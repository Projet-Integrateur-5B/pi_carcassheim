using system;

namespace Server;

using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using ClassLibrary;
using Microsoft.Extensions.Configuration;

/// <summary>
///     Represents an async server.
/// </summary>
public static partial class Server
{
    /// <summary>
    ///     Thread signal used to have each connected client on a different thread in <see cref="Server" />
    ///     .
    /// </summary>
    private static ManualResetEvent AllDone { get; } = new(false);

    /// <summary>
    ///     Stores the <see cref="ServerParameters" />.
    /// </summary>
    private static ServerParameters Settings { get; set; } = new();

    /// <summary>
    ///     Setup a listening socket for the <see cref="Server" /> from <see cref="Settings" />.
    /// </summary>
    /// <param name="error">Stores the <see cref="Tools.Errors" /> value.</param>
    /// <param name="port">Server's local port.</param>
    /// <returns>The listening socket.</returns>
    /// <exception cref="InvalidEnumArgumentException"></exception>
    public static Socket GetListenerSocket(ref Tools.Errors error, int offset)
    {
        // Check if enum "Errors" do exist.
        if (!Enum.IsDefined(typeof(Tools.Errors), error))
        {
            throw new InvalidEnumArgumentException(nameof(error), (int)error, typeof(Tools.Errors));
        }

        // Initialize a default socket.
        var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // Catch : socket could not be created nor put into listening state.
        try
        {
            // Establish the local endpoint for the socket.
            // IpAddress is local and Port is coming from the config file.
            Console.WriteLine("Server is preparing to listen...");
            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = ipHostInfo.AddressList[0];
            var localEndPoint = new IPEndPoint(ipAddress, Settings.LocalPort + offset);

            // Create a TCP/IP socket and Bind to the local endpoint and listen for incoming connections.
            listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(100);
            Console.WriteLine("Server is listening : " + listener.LocalEndPoint);

            // No error has occured.
            error = Tools.Errors.None;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            // Setting the error value.
            error = Tools.Errors.Socket;
        }

        return listener;
    }

    /// <summary>
    ///     Main server method which accepts incoming connections and starts an asynchronous communication.
    /// </summary>
    /// <param name="port">Server's local port.</param>
    public static void StartListening(int offset)
    {
        var error_value = Tools.Errors.None;
        Console.WriteLine("Server is setting up...");

        // Getting the server settings from the config file.
        Settings = ServerParameters.GetConfig(ref error_value);
        if (error_value != Tools.Errors.None) // Checking for errors.
        {
            // Setting the error value.
            // TODO : GetConfig error
            return;
        }

        if (offset > Settings.MaxNbPorts - 1)
            return;

        // Getting the listening socket from the server settings.
        var listener = GetListenerSocket(ref error_value, offset);
        if (error_value != Tools.Errors.None) // Checking for errors.
        {
            // Setting the error value.
            // TODO : GetSockets error
            return;
        }

        // Catch : Something went wrong during an asynchronous communication.
        try
        {
            while (true) // Receives incoming asynchronous connections indefinitely.
            {
                // Set the event to nonsignaled state.
                AllDone.Reset();

                // Start an asynchronous socket to listen for connections.
                Console.WriteLine("Waiting for a connection...");
                var new_state = new StateObject { Listener = listener, Error = Tools.Errors.None };
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

    /// <summary>
    ///     Asynchronous operation to accept an incoming connection attempt.
    /// </summary>
    /// <param name="ar">Async <see cref="StateObject" />.</param>
    public static void AcceptCallback(IAsyncResult ar)
    {
        Console.WriteLine("New connection is being established...");

        // Signal the main thread to continue.
        AllDone.Set();

        var state = (StateObject?)ar.AsyncState;
        if (state is null) // Checking for errors.
        {
            // Setting the error value.
            // TODO : state is null
            return;
        }

        // Accepts an asynchronous connection with a client.
        state.Listener = state.Listener.EndAccept(ar);
        Console.WriteLine("Connection with client is established : " +
                          state.Listener.RemoteEndPoint + " on : " + state.Listener.LocalEndPoint);

        // Start listening.
        StartReading(ar, state.Listener, false);
    }

    /// <summary>
    /// Start receiving from a client but with a timeout.
    /// </summary>
    /// <param name="ar">Async <see cref="StateObject" />.</param>
    /// <param name="listener"><see cref="Socket"/> on which the <see cref="Server"/> must be listening.</param>
    /// <param name="updateState">Indicate if <see cref="StateObject"/> should be reset or not.</param>
    public static void StartReading(IAsyncResult ar, Socket listener, bool updateState)
    {
        var state = (StateObject?)ar.AsyncState;
        if (state is null) // Checking for errors.
        {
            // Setting the error value.
            // TODO : state is null
            return;
        }

        if (updateState)
        {
            state = new StateObject { Listener = listener, ReceiveDone = state.ReceiveDone };
        }

        state.ReceiveDone = new ManualResetEvent(false);
        state.ReceiveDone.Reset();
        listener.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0,
            ReadCallback, state);
        if (!state.ReceiveDone.WaitOne(Settings.Timeout))
        {
            DisconnectFromClient(ar);
        }
    }

    /// <summary>
    ///     Begins to asynchronously receive data from <see cref="StateObject.Listener" /> in
    ///     <see cref="Server" />.
    /// </summary>
    /// <param name="ar">Async <see cref="StateObject" />.</param>
    public static void ReadCallback(IAsyncResult ar)
    {
        // Retrieve the state object from the asynchronous state object.
        var state = (StateObject?)ar.AsyncState;
        if (state is null) // Checking for errors.
        {
            // Setting the error value.
            // TODO : state is null
            return;
        }

        // Signal the async state thread to continue.
        state.ReceiveDone.Set();

        var listener = state.Listener;
        var bytesRead = 0;

        // Trying read data from the client socket.
        // Catch : the socket has timed out.
        try
        {
            bytesRead = listener.EndReceive(ar);
        }
        catch (Exception)
        {
            Console.WriteLine("Connection with the client has timed out");
            return;
        }

        // Nothing to read here.
        if (bytesRead <= 0)
        {
            return;
        }

        // Get the last received bytes.
        var packetAsBytes = new byte[bytesRead];
        Array.Copy(state.Buffer, packetAsBytes, bytesRead);
        var error_value = Tools.Errors.None;

        // Deserialize the byte array
        state.Packet = packetAsBytes.ByteArrayToPacket(ref error_value);
        if (error_value != Tools.Errors.None) // Checking for errors.
        {
            // Setting the error value.
            // TODO : ByteArrayToPacket => handle error
            return;
        }

        var dataLength = state.Data.Length;
        state.Data = state.Data.Concat(state.Packet.Data).ToArray();
        if (dataLength > 0)
        {
            if (state.Data[dataLength] == "")
            {
                state.Data = state.Data.Where((source, index) => index != dataLength)
                    .ToArray();
                state.Data[dataLength - 1] += state.Data[dataLength];
                state.Data = state.Data.Where((source, index) => index != dataLength)
                    .ToArray();
            }
        }

        var debug = "Reading from : " + listener.RemoteEndPoint + " on : " + state.Listener.LocalEndPoint + 
                    "\n\t Read {0} bytes =>\t" + state.Packet +
                    "\n\t Data buffer =>\t\t" + string.Join(" ", state.Data);

        // The client asked for a disconnection.
        if (state.Packet.IdMessage == Tools.IdMessage.AccountLogout)
        {
            Console.WriteLine(debug + "\n\t => FIN !", bytesRead);
            DisconnectFromClient(ar);
        }
        // Final packet of the series has been received.
        else if (state.Packet.Final)
        {
            Console.WriteLine(debug + "\n\t => Every packet has been received !",
                bytesRead);

            state.Packet.Data = state.Data;

            // TODO: check if packet.IdMessage requires an answer for the client

            // Get required data from the database.
            var packet = GetFromDatabase(ar, listener);
            // Send answer to the client.
            SendBackToClient(ar, packet);

            // Start listening again.
            StartReading(ar, listener, true);
        }
        // More packets to receive in this series.
        else
        {
            Console.WriteLine(debug + "\n\t => Waiting for the rest to be send...",
                bytesRead);
            // Keep listening.
            StartReading(ar, listener, false);
        }
    }

    /// <summary>
    ///     Sends data asynchronously to <see cref="StateObject.Listener" /> in <see cref="Server" />.
    /// </summary>
    /// <param name="ar">Async <see cref="StateObject" />.</param>
    /// <param name="original">Instance of <see cref="Packet" /> to send.</param>
    private static void SendBackToClient(IAsyncResult ar, Packet original)
    {
        byte[]? bytes = null;
        var error_value = Tools.Errors.None; // Default error value.

        // Retrieve the state object from the asynchronous state object.
        var state = (StateObject?)ar.AsyncState;
        if (state is null) // Checking for errors.
        {
            // Setting the error value.
            // TODO : state is null
            return;
        }

        // Original packet may be to big to be send directly : splitting it into smaller ones.
        var packets = original.Split(ref error_value);
        if (error_value != Tools.Errors.None) // Checking for errors.
        {
            // Setting the error value.
            // TODO : Split => handle error
            return;
        }

        // Sending split packets.
        foreach (var packet in packets)
        {
            // Serialize the packet.
            bytes = packet.PacketToByteArray(ref error_value);
            if (error_value != Tools.Errors.None) // Checking for errors.
            {
                // Setting the error value.
                // TODO : PacketToByteArray => handle error
                return;
            }

            var size = bytes.Length;
            Console.WriteLine("Sending back : " + state.Listener.RemoteEndPoint + " on : " + state.Listener.LocalEndPoint + 
                              "\n\t Sent {0} bytes =>\t" + packet, size);
            // Send the packet through the socket.
            state.Listener.BeginSend(bytes, 0, size, 0, null, state);
        }
    }

    /// <summary>
    ///     Ends asynchronous connection with a client in <see cref="Server" />.
    /// </summary>
    /// <param name="ar">Async <see cref="StateObject" />.</param>
    private static void DisconnectFromClient(IAsyncResult ar)
    {

        // Retrieve the state object from the asynchronous state object.
        var state = (StateObject?)ar.AsyncState;
        if (state is null) // Checking for errors.
        {
            // Setting the error value.
            // TODO : state is null
            return;
        }
        
        // Récupération du singleton gestionnaire + Attempt to remove a player from all the rooms.
        GestionnaireThreadCom.GetInstance().LogoutPlayer(state.IdPlayer);

        // Ending client connection.
        Console.WriteLine(state.Listener.RemoteEndPoint + "\t => Closing client connection...");
        state.Listener.EndSend(ar);
        state.Listener.Shutdown(SocketShutdown.Both);
        state.Listener.Close();
    }

    /// <summary>
    ///     State object for storing useful data asynchronously in <see cref="Server" />.
    /// </summary>
    public class StateObject
    {
        public ulong IdPlayer { get; set; }
        
        /// <summary>
        ///     Received data size related to <see cref="Packet" /> in <see cref="StateObject" />.
        /// </summary>
        public const int BufferSize = Packet.MaxPacketSize;

        /// <summary>
        ///     Stores the data from <see cref="Listener" /> of size <see cref="BufferSize" /> in
        ///     <see cref="StateObject" />.
        /// </summary>
        public byte[] Buffer { get; } = new byte[BufferSize];

        /// <summary>
        ///     Stores everything from <see cref="Listener" /> in the <see cref="Packet" /> format in
        ///     <see cref="StateObject" />.
        /// </summary>
        public Packet? Packet { get; set; }

        /// <summary>
        ///     Represents the listening socket used asynchronously for each client in
        ///     <see cref="StateObject" />.
        /// </summary>
        public Socket Listener { get; set; } = new(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);

        public string[] Data { get; set; } = Array.Empty<string>();

        /// <summary>
        ///     Stores an asynchronous <see cref="Tools.Errors" /> value.
        /// </summary>
        public Tools.Errors Error { get; set; } = Tools.Errors.None;

        /// <summary>
        ///     Used to check if <see cref="Listener" /> has timed out in <see cref="StateObject" />.
        /// </summary>
        public ManualResetEvent ReceiveDone { get; set; } = new(false);
    }
}

/// <summary>
///     Object used to store the parameters of the <see cref="Server" />.
/// </summary>
[Serializable]
public class ServerParameters
{
    /// <summary>
    ///     The local port on which the server is running.
    /// </summary>
    public int LocalPort { get; set; }
        
    /// <summary>
    ///     The remote port on which the server is running.
    /// </summary>
    public int RemotePort { get; set; }
        
    /// <summary>
    ///     The number max of ports.
    /// </summary>
    public int MaxNbPorts { get; set; }

    /// <summary>
    ///     Time left (without any communication) to a async listening socket until timeout.
    /// </summary>
    public int Timeout { get; set; }
    
    /// <summary>
    ///     Stores the config file of the <see cref="Server" /> into <see cref="Settings" />.
    /// </summary>
    /// <param name="error">Stores the <see cref="Tools.Errors" /> value.</param>
    /// <exception cref="InvalidEnumArgumentException"></exception>
    public static ServerParameters GetConfig(ref Tools.Errors error)
    {
        var settings = new ServerParameters(); // default

        // Check if enum "Errors" do exist.
        if (!Enum.IsDefined(typeof(Tools.Errors), error))
        {
            throw new InvalidEnumArgumentException(nameof(error), (int)error,
                typeof(Tools.Errors));
        }

        // Catch : config file could not be imported nor serialized into ServerParameters.
        try
        {
            // Import the config file.
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .AddEnvironmentVariables()
                .Build();

            // Config into ServerParameters.
            settings = config.GetRequiredSection("Settings").Get<ServerParameters>() ??
                       new ServerParameters();

            // No error has occured.
            error = Tools.Errors.None;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            // Setting the error value.
            error = Tools.Errors.ConfigFile;
        }

        return settings;
    }
}