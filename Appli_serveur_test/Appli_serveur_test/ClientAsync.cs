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
    public Packet? Packet { get; set; }
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

    public delegate void OnPacketReceivedHandler(object sender, Packet packet);
    public static event OnPacketReceivedHandler OnPacketReceived;


    public static void Connection(Socket client_socket,Parameters parameters)
    {
        //Version : Unity
        IPAddress ipAddress = IPAddress.Parse(parameters.ServerIP);
        var remoteEP = new IPEndPoint(ipAddress, parameters.ServerPort);

        // Create a TCP/IP socket.
        client_socket = new Socket(ipAddress.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);

        // Connect to the remote endpoint.
        client_socket.BeginConnect(remoteEP,
            new AsyncCallback(ConnectCallback), client_socket);
    }

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
            {
                return;
            }

            // Get the last received bytes.
            var packetAsBytes = new byte[bytesRead];
            Array.Copy(state.buffer, packetAsBytes, bytesRead);
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

            var debug = "Reading from : " + client.RemoteEndPoint +
                        "\n\t Read {0} bytes =>\t" + state.Packet +
                        "\n\t Data buffer =>\t\t" + string.Join(" ", state.Data);

            if (state.Packet.Final)
            {
                Console.WriteLine(debug + "\n\t => Every packet has been received !",
                    bytesRead);

                state.Packet.Data = state.Data;

                OnPacketReceived?.Invoke(typeof(ClientAsync), state.Packet);
                Receive(client);
                // TODO: check if packet.IdMessage requires an answer for the client

                // Start listening again.
                // StartReading(ar, listener, true);

            }
            // More packets to receive in this series.
            else
            {
                // Get the rest of the data.
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public static void Send(Socket client, Packet original)
    {

        byte[]? bytes = null;
        var error_value = Tools.Errors.None;
        var packets = new List<Packet>();

        packets = original.Split(ref error_value);
        if (error_value != Tools.Errors.None)
        {
            // TODO : Split => handle error
            // return Tools.Errors.Data;
        }

        foreach (var packet in packets)
        {
            // Send the data through the socket.
            bytes = packet.PacketToByteArray(ref error_value);
            if (error_value != Tools.Errors.None)
            {
                // TODO : PacketToByteArray => handle error
                // return Tools.Errors.Data;
            }

            // Begin sending the data to the remote device.
            var size = bytes.Length;
            Console.WriteLine("Sent {0} bytes =>\t" + packet, size);
            client.BeginSend(bytes, 0, size, 0,
                null, client);
        }
    }

    private static void SendCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.
            Socket client = (Socket)ar.AsyncState;

            // Complete sending the data to the remote device.
            int bytesSent = client.EndSend(ar);
            Console.WriteLine("Sent total {0} bytes to server.", bytesSent);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public static void StopListening(Socket client)
    {
        client.EndReceive(null);
    }
}
