namespace Server;

using System.Configuration;
using System.Net;
using System.Net.Sockets;
using Assets;

// State object for reading client data asynchronously
public class StateObject
{
    // Size of receive buffer.
    public const int BufferSize = Packet.MaxPacketSize;

    // Receive buffer.
    public byte[] Buffer { get; } = new byte[BufferSize];

    // Client socket.
    public Socket? WorkSocket { get; set; }

    public Packet? Packet { get; set; }

    public string[] Tableau { get; set; } = Array.Empty<string>();
}

public partial class Server
{
    // Thread signal.
    private static ManualResetEvent AllDone { get; } = new(false);

    public static void StartListening()
    {
        // get config from file
        var port = ConfigurationManager.AppSettings.Get("ServerPort");

        Console.WriteLine("Server is setting up...");

        // Establish the local endpoint for the socket.
        // The DNS name of the computer
        // running the listener is "host.contoso.com".
        var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        var ipAddress = ipHostInfo.AddressList[0];
        var localEndPoint = new IPEndPoint(ipAddress, Packet.Port);

        // Create a TCP/IP socket.
        var listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        // Bind the socket to the local endpoint and listen for incoming connections.
        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(100);
            Console.WriteLine("Server is ready : " + listener.LocalEndPoint);

            while (true)
            {
                // Set the event to nonsignaled state.
                AllDone.Reset();

                // Start an asynchronous socket to listen for connections.
                listener.BeginAccept(AcceptCallback, listener);
                Console.WriteLine("Waiting for a connection...");

                // Wait until a connection is made before continuing.
                AllDone.WaitOne();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        Console.WriteLine("\nPress ENTER to continue...");
        Console.Read();
    }

    public static void AcceptCallback(IAsyncResult ar)
    {
        Console.WriteLine("New connection is being established...");

        // Signal the main thread to continue.
        AllDone.Set();

        // Get the socket that handles the client request.
        var listener = (Socket?)ar.AsyncState;

        if (listener is not null)
        {
            var handler = listener.EndAccept(ar);
            Console.WriteLine("Connection with client is established : " + handler.RemoteEndPoint);

            // Create the state object.
            var state = new StateObject { WorkSocket = handler };
            handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0,
                ReadCallback, state);
        }
        else
        {
            // si c'est null ??
            Console.WriteLine("Connection with client failed !");
        }
    }

    public static void ReadCallback(IAsyncResult ar)
    {
        // Retrieve the state object and the handler socket
        // from the asynchronous state object.
        var state = (StateObject?)ar.AsyncState;

        if (state is not null)
        {
            var handler = state.WorkSocket;
            if (handler is not null)
            {
                // Read data from the client socket.
                var bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    Console.WriteLine(bytesRead);
                    var packetAsBytes = new byte[bytesRead];
                    Array.Copy(state.Buffer, packetAsBytes, bytesRead);
                    var error_value = Errors.None;

                    state.Packet = packetAsBytes.ByteArrayToPacket(ref error_value);
                    if (error_value != Errors.None)
                    {
                        // TODO : ByteArrayToPacket => handle error
                    }

                    // There  might be more data, so store the data received so far.
                    state.Tableau = state.Tableau.Concat(state.Packet.Data).ToArray();

                    // Buffer with all the data
                    var content = state.Tableau.ToString();

                    // Disconnection
                    if ((IdMessage)state.Packet.IdMessage == IdMessage.Disconnection)
                    {
                        Console.WriteLine("Reading from : " + handler.RemoteEndPoint +
                                          "\n\t Read {0} bytes =>\t" + state.Packet +
                                          "\n\t Data buffer =>\t\t" + state.Tableau +
                                          "\n\t => FIN !", bytesRead);
                        state.Packet.Status = true;
                        Array.Clear(state.Packet.Data);
                        Send(ar, true);
                    }
                    // Final packet of the series
                    else if (state.Packet.Final)
                    {
                        state.Packet.Data = state.Tableau;

                        Console.WriteLine("Reading from : " + handler.RemoteEndPoint +
                                          "\n\t Read {0} bytes =>\t" + state.Packet +
                                          "\n\t Data buffer =>\t\t" + state.Tableau +
                                          "\n\t => Every packet has been received !", bytesRead);
                        switch ((IdMessage)state.Packet.IdMessage)
                        {
                            case IdMessage.Connection:
                                state.Packet.Status = Connection(state.Packet);
                                Array.Clear(state.Packet.Data);
                                break;
                            case IdMessage.Signup:
                                state.Packet.Status = Signup(state.Packet);
                                Array.Clear(state.Packet.Data);
                                break;
                            case IdMessage.Statistics:
                                state.Packet = Statistics(state.Packet);
                                break;
                            case IdMessage.RoomList:
                                state.Packet = RoomList(state.Packet);
                                break;
                            case IdMessage.RoomJoin:
                                state.Packet = RoomJoin(state.Packet);
                                break;
                            case IdMessage.RoomLeave:
                                state.Packet.Status = RoomLeave(state.Packet);
                                Array.Clear(state.Packet.Data);
                                break;
                            case IdMessage.RoomReady:
                                state.Packet.Status = RoomReady(state.Packet);
                                Array.Clear(state.Packet.Data);
                                break;
                            case IdMessage.RoomSettings:
                                state.Packet = RoomSettings(state.Packet);
                                break;
                            case IdMessage.RoomStart:
                                state.Packet = RoomStart(state.Packet);
                                break;
                            case IdMessage.Disconnection: // impossible
                                Array.Clear(state.Packet.Data);
                                break;
                            case IdMessage.Default:
                            default:
                                state.Packet.Status = false;
                                break;
                        }

                        // Echo the data back to the client.
                        Send(ar, false);
                        state = new StateObject { WorkSocket = state.WorkSocket };
                        handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0,
                            ReadCallback, state);
                    }
                    // More packets to receive
                    else
                    {
                        Console.WriteLine("Reading from : " + handler.RemoteEndPoint +
                                          "\n\t Read {0} bytes =>\t" + state.Packet +
                                          "\n\t Data buffer =>\t" + state.Tableau +
                                          "\n\t => Waiting for the rest to be send...", bytesRead);

                        // Not all data received. Get more.
                        handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0,
                            ReadCallback, state);
                    }
                }
                else
                {
                    Console.WriteLine("0 bytes to read from : " + handler.RemoteEndPoint);
                }
            }
            else
            {
                // si c'est null ?
                Console.WriteLine("handler is null");
            }
        }
        else
        {
            // si c'est null ?
            Console.WriteLine("state is null");
        }
    }

    private static void Send(IAsyncResult ar, bool end)
    {
        var error_value = Errors.None;
        var state = (StateObject?)ar.AsyncState;
        // TODO : get and put into Data what the client asked for
        var packetAsBytes = state.Packet.PacketToByteArray(ref error_value);
        if (error_value != Errors.None)
        {
            // TODO : PacketToByteArray => handle error
        }

        var size = packetAsBytes.Length;

        // Begin sending the data to the remote device.
        var handler = state.WorkSocket;
        if (end) // disconnection
        {
            handler.BeginSend(packetAsBytes, 0, size, 0, SendCallback, state);
        }
        else
        {
            handler.BeginSend(packetAsBytes, 0, size, 0, null, state);
        }
    }

    private static void SendCallback(IAsyncResult ar)
    {
        try
        {
            var state = (StateObject?)ar.AsyncState;

            if (state is not null)
            {
                // Retrieve the socket from the state object.
                var handler = state.WorkSocket;

                if (handler is not null)
                {
                    // Complete sending the data to the remote device.
                    var bytesSent = handler.EndSend(ar);

                    Console.WriteLine("Sending back : " + handler.RemoteEndPoint +
                                      "\n\t Sent {0} bytes =>\t" + state.Packet +
                                      "\n\t => Closing connection...", bytesSent);

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
                else
                {
                    // si c'est null ?
                    Console.WriteLine("handler is null");
                }
            }
            else
            {
                // si c'est null ?
                Console.WriteLine("state is null");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public static int Main()
    {
        StartListening();
        return 0;
    }
}
