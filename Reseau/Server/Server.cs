namespace Server;

using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using Assets;
using Microsoft.Extensions.Configuration;

public static partial class Server
{
    // Thread signal.
    private static ManualResetEvent AllDone { get; } = new(false);
    private static Parameters Settings { get; set; } = new();

    public static void GetConfig(ref Errors error)
    {
        Settings = new Parameters();
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

            Settings = config.GetRequiredSection("Settings").Get<Parameters>() ?? new Parameters();
            error = Errors.None;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            error = Errors.ConfigFile;
        }
    }

    public static Socket GetListenerSocket(ref Errors error)
    {
        if (!Enum.IsDefined(typeof(Errors), error))
        {
            throw new InvalidEnumArgumentException(nameof(error), (int)error, typeof(Errors));
        }

        var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            // Establish the local endpoint for the socket.
            Console.WriteLine("Server is preparing to listen...");
            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = ipHostInfo.AddressList[0];
            var localEndPoint = new IPEndPoint(ipAddress, Settings.ServerPort);

            // Create a TCP/IP socket and Bind to the local endpoint and listen for incoming connections.
            listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(100);
            Console.WriteLine("Server is listening : " + listener.LocalEndPoint);

            error = Errors.None;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            error = Errors.Socket;
        }

        return listener;
    }

    public static Socket GetDatabaseSocket()
    {
        // Connecting to the database server
        Console.WriteLine("Server is connecting to the database...");
        var databaseAddress = IPAddress.Parse(Settings.DatabaseIp);
        var remoteEp = new IPEndPoint(databaseAddress, Settings.DatabasePort);
        var database = new Socket(databaseAddress.AddressFamily, SocketType.Stream,
            ProtocolType.Tcp);
        // database.Connect(remoteEp);
        Console.WriteLine("Server is connected to the database : {0}", database.RemoteEndPoint);
        return database;
    }

    public static void DisconnectFromDatabase(IAsyncResult ar, ref Errors error)
    {
        var state = (StateObject?)ar.AsyncState;
        if (state is null)
        {
            // TODO : state is null
            return;
        }

        Console.WriteLine(state.Database.RemoteEndPoint + "\t => Closing database connection...");
        /*state.Database.EndSend(ar);
        state.Database.Shutdown(SocketShutdown.Both);
        state.Database.Close();*/
        error = Errors.None;
    }

    public static void StartListening()
    {
        var error_value = Errors.None;

        Console.WriteLine("Server is setting up...");

        GetConfig(ref error_value);
        if (error_value != Errors.None)
        {
            // TODO : GetConfig error
            return;
        }

        var listener = GetListenerSocket(ref error_value);
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
                var new_state = new StateObject { Listener = listener, Error = Errors.None };
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
            // Connecting the thread to the database
            try
            {
                state.Database = Retry.Do(GetDatabaseSocket, TimeSpan.FromSeconds(0));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }

            state.Listener = state.Listener.EndAccept(ar);
            Console.WriteLine("Connection with client is established : " +
                              state.Listener.RemoteEndPoint);

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

                var tailletab = state.Tableau.Length;
                state.Tableau = state.Tableau.Concat(state.Packet.Data).ToArray();
                if (tailletab > 0)
                {
                    var fusion = state.Tableau[tailletab - 1][..4];
                    if (fusion == "<FS>")
                    {
                        state.Tableau[tailletab - 1] = state.Tableau[tailletab - 1][4..] +
                                                       state.Tableau[tailletab];
                        state.Tableau = state.Tableau.Where((source, index) => index != tailletab)
                            .ToArray();
                    }
                }

                var debug = "Reading from : " + listener.RemoteEndPoint +
                            "\n\t Read {0} bytes =>\t" + state.Packet +
                            "\n\t Data buffer =>\t\t" + string.Join(" ", state.Tableau);

                // Disconnection
                if (state.Packet.IdMessage == IdMessage.Disconnection)
                {
                    Console.WriteLine(debug + "\n\t => FIN !", bytesRead);
                    DisconnectFromClient(ar);
                }
                // Final packet of the series
                else if (state.Packet.Final)
                {
                    Console.WriteLine(debug + "\n\t => Every packet has been received !",
                        bytesRead);

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
                    Console.WriteLine(debug + "\n\t => Waiting for the rest to be send...",
                        bytesRead);
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

    private static void DisconnectFromClient(IAsyncResult ar)
    {
        var error_value = Errors.None;

        var state = (StateObject?)ar.AsyncState;
        if (state is null)
        {
            // TODO : state is null
            return;
        }

        // Ending database connection
        DisconnectFromDatabase(ar, ref error_value);
        if (error_value != Errors.None)
        {
            // TODO : DisconnectFromDatabase => handle error
            // return Errors.Data;
        }

        // Ending client connection
        Console.WriteLine(state.Listener.RemoteEndPoint + "\t => Closing client connection...");
        state.Listener.EndSend(ar);
        state.Listener.Shutdown(SocketShutdown.Both);
        state.Listener.Close();
    }

    public static int Main()
    {
        StartListening();
        return 0;
    }

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

        public Socket Listener { get; set; } = new(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);

        public Socket Database { get; set; } = new(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);

        public Packet? Packet { get; set; }

        public string[] Tableau { get; set; } = Array.Empty<string>();

        public Errors Error { get; set; } = Errors.None;
    }
}
