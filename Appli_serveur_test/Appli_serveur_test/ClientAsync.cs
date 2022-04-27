using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using ClassLibrary;

// State object for receiving data from remote device.
public class StateObject
{
    // Client socket.
    public Socket workSocket = null;
    // Size of receive buffer.
    public const int BufferSize = 256;
    // Receive buffer.
    public byte[] buffer = new byte[BufferSize];
    // Received data string.
    public StringBuilder sb = new StringBuilder();
    public List<Packet>? Packets { get; set; }
    public string[] Data { get; set; } = Array.Empty<string>();
}

[Serializable]
public class Parameters
{
    public int ServerPort { get; set; }
    public string ServerIP { get; set; } = "";
}

public class ClientAsync
{

    // The port number for the remote device.
    private const int port = 10000;

    // ManualResetEvent instances signal completion.
    public static ManualResetEvent connectDone =
        new ManualResetEvent(false);

    public delegate void OnPacketReceivedHandler(object sender, Packet packet, Socket? socket);
    public static event OnPacketReceivedHandler OnPacketReceived;

    private static void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.
            Socket client = (Socket)ar.AsyncState;

            // Complete the connection.
            client.EndConnect(ar);

            Console.WriteLine("Client is connected to {0}", client.RemoteEndPoint);

            // Signal that the connection has been made.
            connectDone.Set();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public static void Receive(Socket client)
    {
        try
        {
            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = client;

            // Begin receiving the data from the remote device.
            client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReceiveCallback), state);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private static void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the state object and the client socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket client = state.workSocket;

            // Read data from the remote device.
            int bytesRead = client.EndReceive(ar);

            // Nothing to read here.
            if (bytesRead <= 0)
                return;

            // Get the last received bytes.
            var packetAsBytes = new byte[bytesRead];
            Array.Copy(state.buffer, packetAsBytes, bytesRead);
            var error_value = Tools.Errors.None;

            // Deserialize the byte array
            state.Packets = packetAsBytes.ByteArrayToPacket(ref error_value);
            if (error_value != Tools.Errors.None) // Checking for errors.
            {
                // Setting the error value.
                // TODO : ByteArrayToPacket => handle error
                return;
            }

            var tasks = new List<Task>();
            foreach(var packet in state.Packets)
            {
                var debug = "Reading from : " + client.RemoteEndPoint +
                            "\n\t Read {0} bytes =>\t" + packet +
                            "\n\t Data buffer =>\t\t" + string.Join(" ", state.Data);
                Console.WriteLine(debug);

                tasks.Add(Task.Run(() => OnPacketReceived?.Invoke(typeof(ClientAsync), packet, client)));
            }

            Task.WhenAll(tasks).Wait();
            state.Packets.Clear();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    /// <summary>
    ///     Sends a packet to a specific <see cref="Socket" />.
    /// </summary>
    /// <param name="client">Instance of <see cref="Socket" /> to send to.</param>
    /// <param name="original">Instance of <see cref="Packet" /> to send.</param>
    public static void Send(Socket client, Packet original)
    {
        byte[]? bytes = null;
        var error_value = Tools.Errors.None; // Default error value.

        // Serialize the packet.
        bytes = original.PacketToByteArray(ref error_value);
        if (error_value != Tools.Errors.None) // Checking for errors.
        {
            // Setting the error value.
            // TODO : PacketToByteArray => handle error
            return;
        }

        var size = bytes.Length;
        Console.WriteLine("Sending to : " + client.RemoteEndPoint + " on : " + client.LocalEndPoint + 
                          "\n\t Sent {0} bytes =>\t" + original, size);
        // Send the packet through the socket.
        client.BeginSend(bytes, 0, size, 0, null, client);
    }
}
