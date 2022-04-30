using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using ClassLibrary;
using UnityEngine;
using Assets.System;
using System.Threading.Tasks;

// State object for receiving data from remote device.
public class StateObject
{
    // Client socket.
    public Socket workSocket = null;
    // Size of receive buffer.
    public const int BufferSize = Packet.MaxPacketSize;
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
    //public static Socket clientSocket { get; private set; }

    // ManualResetEvent instances signal completion.
    public static ManualResetEvent connectDone = new ManualResetEvent(false);
    private static ManualResetEvent receiveDone = new ManualResetEvent(false);


    public delegate void OnPacketReceivedHandler(object sender, Packet packet);
    public static event OnPacketReceivedHandler OnPacketReceived;

    public static void Connection(Parameters parameters)
    {
        connectDone.Reset();

        //Version : Unity
        IPAddress ipAddress = IPAddress.Parse(parameters.ServerIP);
        var remoteEP = new IPEndPoint(ipAddress, parameters.ServerPort);

        // Create a TCP/IP socket.
        Socket clientSocket = new Socket(ipAddress.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);

        Communication.Instance.SetSocket(clientSocket);

        // Connect to the remote endpoint.
        clientSocket.BeginConnect(remoteEP,
            new AsyncCallback(ConnectCallback), clientSocket);
    }

    private static void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.
            Socket client = (Socket)ar.AsyncState;

            // Complete the connection.
            client.EndConnect(ar);

            Debug.Log("Client is connected to {0} " + client.RemoteEndPoint);

            // Signal that the connection has been made.
            connectDone.Set();
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public static void Disconnection(Socket clientSocket)
    {
        clientSocket.BeginDisconnect(true,
            new AsyncCallback(DisconnectCallback), clientSocket);
    }

    public static void DisconnectCallback(IAsyncResult ar)
    {
        try
        {
            // Complete the disconnect request.
            Socket client = (Socket)ar.AsyncState;
            client.EndDisconnect(ar);

            Debug.Log("Client is disconnected to {0} " + client.RemoteEndPoint);

        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public static void Receive(Socket clientSocket)
    {
        try
        {
            Debug.Log("------------------------- Lancement Ecoute -------------------------");

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = clientSocket;

            Communication.Instance.isListening = true;

            // Begin receiving the data from the remote device.
            clientSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReceiveCallback), state);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            Communication.Instance.isListening = false;
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
            foreach (var packet in state.Packets)
            {
                var debug = "Reading from : " + client.RemoteEndPoint +
                        "\n\t Read {"+ client.RemoteEndPoint +"} bytes =>\t" + packet +
                        "\n\t Data buffer =>\t\t" + string.Join(" ", state.Data);

                Debug.Log(debug);

                tasks.Add(Task.Run(() => OnPacketReceived?.Invoke(typeof(ClientAsync), packet)));
            }

            Task.WhenAll(tasks).Wait();
            state.Packets.Clear();

            Communication.Instance.isListening = false;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            Communication.Instance.isListening = false;
        }
    }

    public static void ReceiveLoop(Socket clientSocket)
    {
        try
        {
            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = clientSocket;

            while (true)
            {
                Debug.Log("------------------------- Lancement Ecoute Infini -------------------------");
                receiveDone.Reset();
                // Begin receiving the data from the remote device.
                clientSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveLoopCallback), state);
                receiveDone.WaitOne();
            }

        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private static void ReceiveLoopCallback(IAsyncResult ar)
    {
        receiveDone.Set();
        ReceiveCallback(ar);
    }

    public static void Send(Socket clientSocket, Packet original)
    {
        Debug.Log("/////////////////////////////////////// original.IdMessage : " + original.IdMessage + " ////////////////////////////////////////");
        byte[]? bytes = null;
        var error_value = Tools.Errors.None;

        // Send the data through the socket.
        bytes = original.PacketToByteArray(ref error_value);
        if (error_value != Tools.Errors.None)
        {
            // TODO : PacketToByteArray => handle error
            // return Tools.Errors.Data;
        }

        // Begin sending the data to the remote device.
        var size = bytes.Length;

        var debug = "Sent total {"+ size +"} bytes to server."+
                        "\n\t bytes =>\t" + original;

        Debug.Log(debug);

        clientSocket.BeginSend(bytes, 0, size, 0,
            SendCallback, clientSocket);
    }

    private static void SendCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.
            Socket client = (Socket)ar.AsyncState;

            // Complete sending the data to the remote device.
            int bytesSent = client.EndSend(ar);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public static void StopListening(Socket client)
    {
        //Todo
    }
}
