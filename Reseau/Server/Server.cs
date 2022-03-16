namespace Server;

using System.Net;
using System.Net.Sockets;
using System.Text;

// State object for reading client data asynchronously
public class StateObject
{
    // Size of receive buffer.
    public const int BufferSize = 1024;

    // Receive buffer.
    public byte[] Buffer { get; } = new byte[BufferSize];

    // Received data string.
    public StringBuilder Sb { get; } = new();

    // Client socket.
    public Socket? WorkSocket { get; set; }
}

public class Server
{
    // Thread signal.
    private static ManualResetEvent AllDone { get; } = new(false);

    public static void StartListening()
    {
        // Establish the local endpoint for the socket.
        // The DNS name of the computer
        // running the listener is "host.contoso.com".
        var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        var ipAddress = ipHostInfo.AddressList[0];
        var localEndPoint = new IPEndPoint(ipAddress, 11000);

        // Create a TCP/IP socket.
        var listener = new Socket(ipAddress.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);

        // Bind the socket to the local endpoint and listen for incoming connections.
        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(100);

            while (true)
            {
                // Set the event to nonsignaled state.
                AllDone.Reset();

                // Start an asynchronous socket to listen for connections.
                Console.WriteLine("Waiting for a connection...");
                listener.BeginAccept(AcceptCallback, listener);

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
        // Signal the main thread to continue.
        AllDone.Set();

        // Get the socket that handles the client request.
        var listener = (Socket?)ar.AsyncState;

        if (listener is not null)
        {
            var handler = listener.EndAccept(ar);

            // Create the state object.
            var state = new StateObject {WorkSocket = handler};
            handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0,
                ReadCallback, state);
        }
        // si c'est null ??
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
                    // There  might be more data, so store the data received so far.
                    state.Sb.Append(Encoding.ASCII.GetString(
                        state.Buffer, 0, bytesRead));

                    // Check for end-of-file tag. If it is not there, read
                    // more data.
                    var content = state.Sb.ToString();
                    if (content.IndexOf("<EOF>", StringComparison.Ordinal) > -1)
                    {
                        // All the data has been read from the
                        // client. Display it on the console.
                        Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                            content.Length, content);
                        // Echo the data back to the client.
                        Send(handler, content);
                    }
                    else
                    {
                        // Not all data received. Get more.
                        handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0,
                            ReadCallback, state);
                    }
                }
            }
            // si c'est null ?
        }
        // si c'est null ?
    }

    private static void Send(Socket handler, string data)
    {
        // Convert the string data to byte data using ASCII encoding.
        var byteData = Encoding.ASCII.GetBytes(data);

        // Begin sending the data to the remote device.
        handler.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, handler);
    }

    private static void SendCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.
            var handler = (Socket?)ar.AsyncState;

            if (handler is not null)
            {
                // Complete sending the data to the remote device.
                var bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            // si c'est null ??
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
