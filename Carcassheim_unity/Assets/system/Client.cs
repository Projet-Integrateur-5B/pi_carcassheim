using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security;
using UnityEngine;
using Newtonsoft.Json;

public class Client
{
    [Serializable]
    public class ServerParameters
    {
        public int serverPort;
        public string serverIP;
    }
    
    public static int Connection(ref Socket? socket)
    {
        // TODO : trycatch lors de la récupération des données de config
        TextAsset contents = Resources.Load<TextAsset>("network/config");
        ServerParameters serverParameters = JsonConvert.DeserializeObject<ServerParameters>(contents.ToString());

        try
        {
            Debug.Log(string.Format("Client is setting up..."));

            // Establish the remote endpoint for the socket.
            IPAddress ipAddress = IPAddress.Parse(serverParameters.serverIP);
            var remoteEP = new IPEndPoint(ipAddress, serverParameters.serverPort);

            // Create a TCP/IP  socket.
            socket = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint. Catch any errors.
            try
            {
                socket.Connect(remoteEP);
                Debug.Log(string.Format("Client is connected to {0}", socket.RemoteEndPoint));
                return (int)Errors.None;
            }
            catch (ArgumentNullException ane)
            {
                // Connect : remoteEP is null
                Debug.Log(string.Format("ArgumentNullException : {0}", ane));
                return (int)Errors.ToBeDetermined; // Unknown ?
            }
            catch (SocketException se)
            {
                // Connect : An error occurred when attempting to access the socket
                Debug.Log(string.Format("SocketException : {0}", se));
                return (int)Errors.Socket;
            }
            catch (ObjectDisposedException ode)
            {
                // Connect : The Socket has been closed
                Debug.Log(string.Format("ObjectDisposedException : {0}", ode));
                return (int)Errors.Socket;
            }
            catch (SecurityException se)
            {
                // Connect : A caller higher in the call stack does not have permission for the requested operation
                Debug.Log(string.Format("SecurityException : {0}", se));
                return (int)Errors.ToBeDetermined; // Unknown ?
            }
            catch (InvalidOperationException se)
            {
                // Connect : The Socket has been placed in a listening state by calling Listen(Int32).
                Debug.Log(string.Format("InvalidOperationException : {0}", se));
                return (int)Errors.Socket;
            }
            catch (Exception e)
            {
                Debug.Log(string.Format("Unexpected exception : {0}", e));
                return (int)Errors.Unknown;
            }
        }
        catch (ArgumentNullException ane)
        {
            // IPAddress.Parse : ipAddress a la valeur null
            // IPEndPoint : address a la valeur null
            Debug.Log(string.Format("ArgumentNullException : {0}", ane));
            return (int)Errors.ConfigFile;
        }
        catch (FormatException fe)
        {
            // IPAddress.Parse : ipAddress n’est pas une adresse IP valide
            Debug.Log(string.Format("FormatException : {0}", fe));
            return (int)Errors.ConfigFile;
        }
        catch (ArgumentOutOfRangeException aoore)
        {
            // IPEndPoint : port est inférieur à MinPort; port est supérieur à MaxPort; address est inférieur à 0 ou supérieur à 0x00000000FFFFFFFF
            Debug.Log(string.Format("ArgumentOutOfRangeException : {0}", aoore));
            return (int)Errors.ConfigFile;
        }
        catch (SocketException se)
        {
            // Socket : La combinaison de addressFamily, socketType et protocolType crée un socket non valide
            Debug.Log(string.Format("SocketException : {0}", se));
            return (int)Errors.ConfigFile;
        }
        catch (Exception e)
        {
            Debug.Log(string.Format(e.ToString()));
            return (int)Errors.Unknown;
        }
    }

    public static int Disconnection(Socket socket)
    {
        var original = new Packet();
        Communication(socket, ref original, (byte)IdMessage.Disconnection, "");
        // if Errors.Format = ignore, not expecting to receive anything from the server
        // if Errors.Socket = ignore, already disconnected from the server

        // Deconnect to a remote device.
        try
        {
            Debug.Log(string.Format("Closing connection..."));
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            return (int)Errors.None;
        }
        catch (SocketException se)
        {
            // Shutdown : An error occurred when attempting to access the socket
            Debug.Log(string.Format("SocketException : {0}", se));
            return (int)Errors.Socket;
        }
        catch (ObjectDisposedException ode)
        {
            // Shutdown : The Socket has been closed
            Debug.Log(string.Format("ObjectDisposedException : {0}", ode));
            return (int)Errors.None;
        }
        catch (Exception e)
        {
            Debug.Log(string.Format("Unexpected exception : {0}", e));
            return (int)Errors.Unknown;
        }
    }
    
    public static int Communication(Socket socket, ref Packet received, byte idMessage, string data)
    {
        try
        {
            byte[]? bytes = null;
            var packets = new List<Packet>();
            var original = new Packet(false, 0, idMessage, true, 999, data);

            try
            {
                packets = original.Split();
            }
            catch (Exception e)
            {
                // PacketToByteArray : issue during serialization
                Debug.Log(string.Format("Exception : {0}", e));
                return (int)Errors.Data;
            }

            var count_errors = 0;
            while (true)
            {
                foreach (var packet in packets)
                {
                    // Send the data through the socket.
                    try
                    {
                        bytes = packet.PacketToByteArray();
                    }
                    catch (Exception e)
                    {
                        // PacketToByteArray : issue during serialization
                        Debug.Log(string.Format("Exception : {0}", e));
                        return (int)Errors.Data;
                    }
                    var bytesSent = socket.Send(bytes);
                    Debug.Log(string.Format("Sent {0} bytes =>\t" + packet, bytesSent));
                }

                // Receive the response from the remote device.
                bytes = new byte[Packet.MaxPacketSize];
                var bytesRec = socket.Receive(bytes);
                var packetAsBytes = new byte[bytesRec];

                try
                {
                    Array.Copy(bytes, packetAsBytes, bytesRec);
                    received = packetAsBytes.ByteArrayToPacket();
                    Debug.Log(string.Format(
                        received.Status
                            ? "Read {0} bytes => \tpermission accepted \n"
                            : "Read {0} bytes => \tpermission denied \n", bytesRec));
                    return (int)Errors.None;
                }
                catch (Exception e)
                {
                    // besoin de tester 3x ? return en premiere instance ?
                    count_errors++;
                    if (count_errors != 3)
                    {
                        continue;
                    }
                    Debug.Log(string.Format("Exception : {0}", e));
                    return (int)Errors.Receive;
                }
            }
        }
        catch (ArgumentNullException ane)
        {
            // Send/Receive : buffer a la valeur null
            Debug.Log(string.Format("ArgumentNullException : {0}", ane));
            return (int)Errors.ToBeDetermined; // Unknown ?
        }
        catch (SocketException se)
        {
            // Send/Receive : Une erreur s’est produite pendant la tentative d’accès au socket
            Debug.Log(string.Format("SocketException : {0}", se));
            return (int)Errors.Socket;
        }
        catch (ObjectDisposedException ode)
        {
            // Send/Receive : Socket a été fermé
            Debug.Log(string.Format("ObjectDisposedException : {0}", ode));
            return (int)Errors.Socket;
        }
        catch (SecurityException se)
        {
            // Receive : Un appelant de la pile des appels ne dispose pas des autorisations requises
            Debug.Log(string.Format("SecurityException : {0}", se));
            return (int)Errors.ToBeDetermined; // Unknown ?
        }
        catch (Exception e)
        {
            Debug.Log(string.Format("Unexpected exception : {0}", e));
            return (int)Errors.Unknown;
        }
    }


    public static void StartClient(byte number, string data)
    {
        // get config from file
        TextAsset contents = Resources.Load<TextAsset>("network/config");
        ServerParameters serverParameters = JsonConvert.DeserializeObject<ServerParameters>(contents.ToString());

        // Data buffer for incoming data.
        var bytes = new byte[Packet.MaxPacketSize];

        // Connect to a remote device.
        try
        {
            Debug.Log(string.Format("Client is setting up..."));

            // Establish the remote endpoint for the socket.
            IPAddress ipAddress = IPAddress.Parse(serverParameters.serverIP);
            var remoteEP = new IPEndPoint(ipAddress, serverParameters.serverPort);
            /*var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = IPAddress.Parse("185.155.93.105");
            var remoteEP = new IPEndPoint(ipAddress, 19000);*/

            // Create a TCP/IP  socket.
            var sender = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint. Catch any errors.
            try
            {
                sender.Connect(remoteEP);
                Debug.Log(string.Format("Client is connected to {0}", sender.RemoteEndPoint));

                var original = new Packet(false, 0, number, true, 999, data);
                var packets = original.Split();

                foreach (var packet in packets)
                {
                    var packetAsBytes = packet.PacketToByteArray();
                    bytes = new byte[packetAsBytes.Length];

                    // Send the data through the socket.
                    var bytesSent = sender.Send(packetAsBytes);
                    Debug.Log(string.Format("Sent {0} bytes =>\t" + packet, bytesSent));
                }

                // Receive the response from the remote device.
                var bytesRec = sender.Receive(bytes);
                var packetAsBytes2 = new byte[bytesRec];
                Array.Copy(bytes, packetAsBytes2, bytesRec);
                var recv = packetAsBytes2.ByteArrayToPacket();
                if (recv.Status)
                {
                    Debug.Log(string.Format("Read {0} bytes => permission accepted \n", bytesRec));
                }
                else
                {
                    Debug.Log(string.Format("Read {0} bytes => permission denied \n", bytesRec));
                }

                //Release the socket.
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                Debug.Log(string.Format("Closing connection..."));
            }
            catch (ArgumentNullException ane)
            {
                Debug.Log(string.Format("ArgumentNullException : {0}", ane));
            }
            catch (SocketException se)
            {
                Debug.Log(string.Format("SocketException : {0}", se));
            }
            catch (Exception e)
            {
                Debug.Log(string.Format("Unexpected exception : {0}", e));
            }
        }
        catch (Exception e)
        {
            Debug.Log(string.Format(e.ToString()));
        }
    }


}
