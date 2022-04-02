namespace Server;

using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using Assets;
using Microsoft.Extensions.Configuration;

public static partial class Server
{

    [Serializable]
    public class Parameters
    {
        public int ServerPort { get; set; }
        public int DatabasePort { get; set; }
        public string DatabaseIp { get; set; } = new("");
    }

    // State object for reading client data asynchronously
    public class StateObject
    {
        // Size of receive buffer.
        public const int BufferSize = Packet.MaxPacketSize;

        // Receive buffer.
        public byte[] Buffer { get; } = new byte[BufferSize];

        public Socket Listener { get; set; } = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);

        public Socket Database { get; set; } = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);

        public Packet? Packet { get; set; }

        public string[] Tableau { get; set; } = Array.Empty<string>();

        public Errors Error { get; set; } = Errors.None;
    }

    // Thread signal.
    private static ManualResetEvent AllDone { get; } = new(false);

    public static Parameters GetConfig(ref Errors error)
    {
        if (!Enum.IsDefined(typeof(Errors), error))
        {
            throw new InvalidEnumArgumentException(nameof(error), (int)error,
                typeof(Errors));
        }

        try
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .AddEnvironmentVariables()
                .Build();

            var serverParameters = config.GetRequiredSection("Settings").Get<Parameters>();
            error = Errors.None;
            return serverParameters ?? new Parameters();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            error = Errors.ConfigFile;
            return new Parameters();
        }
    }

    public static StateObject GetSockets(ref Errors error)
    {
        if (!Enum.IsDefined(typeof(Errors), error))
        {
            throw new InvalidEnumArgumentException(nameof(error), (int)error, typeof(Errors));
        }

        var state = new StateObject();
        try
        {
            var parameters = GetConfig(ref error);
            if (error != Errors.None)
            {
                // TODO : GetConfig error
                return state;
            }

            // Connecting to the database server
            /*Console.WriteLine("Server is connecting to the database...");
            var databaseAddress = IPAddress.Parse(parameters.DatabaseIp);
            var remoteEp = new IPEndPoint(databaseAddress, parameters.DatabasePort);
            state.Database = new Socket(databaseAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            state.Database.Connect(remoteEp);
            Console.WriteLine("Server is connected to the database : {0}", state.Database.RemoteEndPoint);*/

            // Establish the local endpoint for the socket.
            Console.WriteLine("Server is preparing to listen...");
            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = ipHostInfo.AddressList[0];
            var localEndPoint = new IPEndPoint(ipAddress, parameters.ServerPort);

            // Create a TCP/IP socket and Bind to the local endpoint and listen for incoming connections.
            state.Listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            state.Listener.Bind(localEndPoint);
            state.Listener.Listen(100);
            Console.WriteLine("Server is listening : " + state.Listener.LocalEndPoint);

            error = Errors.None;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            error = Errors.Socket;
        }
        return state;
    }

    public static void StartListening()
    {
        var error_value = Errors.None;

        Console.WriteLine("Server is setting up...");
        var state = GetSockets(ref error_value);
        if (error_value != Errors.None)
        {
            // TODO : GetSockets error
            return;
        }

        try
        {
            while (true)
            {
                // Set the event to nonsignaled state.
                AllDone.Reset();

                // Start an asynchronous socket to listen for connections.
                Console.WriteLine("Waiting for a connection...");
                var new_state = new StateObject { Listener = state.Listener, Database = state.Database, Error = Errors.None };
                new_state.Listener.BeginAccept(AcceptCallback, new_state);

                // Wait until a connection is made before continuing.
                AllDone.WaitOne();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        Console.WriteLine("Server is closed");
    }

    public static void AcceptCallback(IAsyncResult ar)
    {
        Console.WriteLine("New connection is being established...");

        // Signal the main thread to continue.
        AllDone.Set();

        var state = (StateObject?)ar.AsyncState;
        if (state is not null)
        {
            state.Listener = state.Listener.EndAccept(ar);
            Console.WriteLine("Connection with client is established : " + state.Listener.RemoteEndPoint);
            state.Listener.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0,
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
            var listener = state.Listener;
            // Read data from the client socket.
            var bytesRead = listener.EndReceive(ar);

            if (bytesRead > 0)
            {
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

                var debug = "Reading from : " + listener.RemoteEndPoint +
                               "\n\t Read {0} bytes =>\t" + state.Packet +
                               "\n\t Data buffer =>\t\t" + string.Join(" ", state.Tableau);

                // Disconnection
                if (state.Packet.IdMessage == IdMessage.Disconnection)
                {
                    Console.WriteLine(debug + "\n\t => FIN !", bytesRead);
                    DisconnectFromClient(ar, state.Packet);
                }
                // Final packet of the series
                else if (state.Packet.Final)
                {
                    Console.WriteLine(debug + "\n\t => Every packet has been received !", bytesRead);

                    // Get answer data from database and answer the client
                    state.Packet.Data = state.Tableau;
                    var packet = GetFromDatabase(ar);
                    SendBackToClient(ar, packet);

                    // Start listening again
                    state = new StateObject { Listener = listener };
                    listener.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0,
                        ReadCallback, state);
                }
                // More packets to receive
                else
                {
                    Console.WriteLine(debug + "\n\t => Waiting for the rest to be send...", bytesRead);
                    listener.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0,
                        ReadCallback, state);
                }
            }
            else
            {
                Console.WriteLine("0 bytes to read from : " + listener.RemoteEndPoint);
            }
        }
        else
        {
            // si c'est null ?
            Console.WriteLine("state is null");
        }
    }

    private static void SendBackToClient(IAsyncResult ar, Packet original)
    {
        byte[]? bytes = null;
        var error_value = Errors.None;

        var state = (StateObject?)ar.AsyncState;
        if (state is null)
        {
            // TODO : state is null
            return;
        }

        var packets = original.Split(ref error_value);
        if (error_value != Errors.None)
        {
            // TODO : Split => handle error
            // return Errors.Data;
        }

        foreach (var packet in packets)
        {
            // Send the data through the socket.
            bytes = packet.PacketToByteArray(ref error_value);
            if (error_value != Errors.None)
            {
                // TODO : PacketToByteArray => handle error
                // return Errors.Data;
            }

            var size = bytes.Length;
            Console.WriteLine("Sending back : " + state.Listener.RemoteEndPoint +
                              "\n\t Sent {0} bytes =>\t" + packet, size);
            state.Listener.BeginSend(bytes, 0, size, 0, null, state);
        }
    }

    private static void DisconnectFromClient(IAsyncResult ar, Packet packet)
    {
        byte[]? bytes = null;
        var error_value = Errors.None;

        var state = (StateObject?)ar.AsyncState;
        if (state is null)
        {
            // TODO : state is null
            return;
        }

        packet.Status = true;
        packet.IdMessage = IdMessage.Disconnection;
        Array.Clear(packet.Data);

        bytes = packet.PacketToByteArray(ref error_value);
        if (error_value != Errors.None)
        {
            // TODO : PacketToByteArray => handle error
            // return Errors.Data;
        }

        var size = bytes.Length;
        state.Listener.BeginSend(bytes, 0, size, 0, null, state);

        // Ending the connection.
        var listener = state.Listener;
        var bytesSent = listener.EndSend(ar);

        Console.WriteLine("Sending back : " + listener.RemoteEndPoint +
                          "\n\t Sent {0} bytes =>\t" + packet +
                          "\n\t => Closing connection...", bytesSent);

        listener.Shutdown(SocketShutdown.Both);
        listener.Close();
    }

    public static int Main()
    {
        StartListening();
        return 0;
    }
}
