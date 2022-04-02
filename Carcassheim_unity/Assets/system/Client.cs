using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security;
using Newtonsoft.Json;
using UnityEngine;

public static class Client
{
    [Serializable]
    public class Parameters
    {
        public int ServerPort { get; set; }
        public string ServerIP { get; set; } = "";
    }

    public static Errors Connection(ref Socket socket)
    {
        // TODO : trycatch lors de la récupération des données de config
        // Version : Unity
        TextAsset contents = Resources.Load<TextAsset>("network/config");
        Parameters parameters = JsonConvert.DeserializeObject<Parameters>(contents.ToString());

        try
        {
            Debug.Log(string.Format("Client is setting up..."));

            // Establish the remote endpoint for the socket.
            // Version : Unity
            IPAddress ipAddress = IPAddress.Parse(parameters.ServerIP);
            var remoteEP = new IPEndPoint(ipAddress, parameters.ServerPort);
            // Version : Local
            /*var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = ipHostInfo.AddressList[0];
            var remoteEP = new IPEndPoint(ipAddress, 10000);*/


            // Create a TCP/IP  socket.
            socket = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint. Catch any errors.
            try
            {
                socket.Connect(remoteEP);
                Debug.Log(string.Format("Client is connected to {0}", socket.RemoteEndPoint));
                return Errors.None;
            }
            catch (ArgumentNullException ane)
            {
                // Connect : remoteEP is null
                Debug.Log(string.Format("ArgumentNullException : {0}", ane));
                return Errors.ToBeDetermined; // Unknown ?
            }
            catch (SocketException se)
            {
                // Connect : An error occurred when attempting to access the socket
                Debug.Log(string.Format("SocketException : {0}", se));
                return Errors.Socket;
            }
            catch (ObjectDisposedException ode)
            {
                // Connect : The Socket has been closed
                Debug.Log(string.Format("ObjectDisposedException : {0}", ode));
                return Errors.Socket;
            }
            catch (SecurityException se)
            {
                // Connect : A caller higher in the call stack does not have permission for the requested operation
                Debug.Log(string.Format("SecurityException : {0}", se));
                return Errors.ToBeDetermined; // Unknown ?
            }
            catch (InvalidOperationException se)
            {
                // Connect : The Socket has been placed in a listening state by calling Listen(Int32).
                Debug.Log(string.Format("InvalidOperationException : {0}", se));
                return Errors.Socket;
            }
            catch (Exception e)
            {
                Debug.Log(string.Format("Unexpected exception : {0}", e));
                return Errors.Unknown;
            }
        }
        catch (ArgumentNullException ane)
        {
            // IPAddress.Parse : ipAddress a la valeur null
            // IPEndPoint : address a la valeur null
            Debug.Log(string.Format("ArgumentNullException : {0}", ane));
            return Errors.ConfigFile;
        }
        catch (FormatException fe)
        {
            // IPAddress.Parse : ipAddress n’est pas une adresse IP valide
            Debug.Log(string.Format("FormatException : {0}", fe));
            return Errors.ConfigFile;
        }
        catch (ArgumentOutOfRangeException aoore)
        {
            // IPEndPoint : port est inférieur à MinPort; port est supérieur à MaxPort; address est inférieur à 0 ou supérieur à 0x00000000FFFFFFFF
            Debug.Log(string.Format("ArgumentOutOfRangeException : {0}", aoore));
            return Errors.ConfigFile;
        }
        catch (SocketException se)
        {
            // Socket : La combinaison de addressFamily, socketType et protocolType crée un socket non valide
            Debug.Log(string.Format("SocketException : {0}", se));
            return Errors.ConfigFile;
        }
        catch (Exception e)
        {
            Debug.Log(string.Format(e.ToString()));
            return Errors.Unknown;
        }
    }

    public static Errors Disconnection(Socket socket)
    {
        var original = new Packet();
        socket.Communication(ref original, IdMessage.Disconnection, Array.Empty<string>());
        // if Errors.Format = ignore, not expecting to receive anything from the server
        // if Errors.Socket = ignore, already disconnected from the server

        // Deconnect to a remote device.
        try
        {
            Debug.Log(string.Format("Closing connection..."));
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            return Errors.None;
        }
        catch (SocketException se)
        {
            // Shutdown : An error occurred when attempting to access the socket
            Debug.Log(string.Format("SocketException : {0}", se));
            return Errors.Socket;
        }
        catch (ObjectDisposedException ode)
        {
            // Shutdown : The Socket has been closed
            Debug.Log(string.Format("ObjectDisposedException : {0}", ode));
            return Errors.None;
        }
        catch (Exception e)
        {
            Debug.Log(string.Format("Unexpected exception : {0}", e));
            return Errors.Unknown;
        }
    }

    public static Errors Communication(this Socket socket, ref Packet received,
        IdMessage idMessage, string[] data)
    {
        try
        {
            byte[]? bytes = null;
            var error_value = Errors.None;
            var packets = new List<Packet>();
            var original = new Packet(false, 0, idMessage, true, 999, data);

            packets = original.Split(ref error_value);
            if (error_value != Errors.None)
            {
                // TODO : Split => handle error
                return Errors.Data;
            }
            
            foreach (var packet in packets)
            {
                // Send the data through the socket.
                bytes = packet.PacketToByteArray(ref error_value);
                if (error_value != Errors.None)
                {
                    // TODO : PacketToByteArray => handle error
                    return Errors.Data;
                }

                var bytesSent = socket.Send(bytes);
                Debug.Log(string.Format("Sent {0} bytes =>\t" + packet, bytesSent));
            }
            
            // check if an answer is needed
            if (idMessage == IdMessage.Disconnection)
            {
                return Errors.None;
            }

            // Receive the response from the remote device.
            bytes = new byte[Packet.MaxPacketSize];
            var bytesRec = 0;
            var packetAsBytes = new byte[bytesRec];
            var part_answer = new Packet();

            while (true)
            {
                bytesRec = socket.Receive(bytes);
                packetAsBytes = new byte[bytesRec];

                Array.Copy(bytes, packetAsBytes, bytesRec);
                part_answer = packetAsBytes.ByteArrayToPacket(ref error_value);
                if (error_value != Errors.None)
                {
                    // TODO : ByteArrayToPacket => handle error
                    return Errors.Data;
                }

                Debug.Log(string.Format("Read {0} bytes => \t" + part_answer + "\n\t\t\tPermission = " +
                                        part_answer.Status, bytesRec));

                received.Data = received.Data.Concat(part_answer.Data).ToArray();
                if (received.Final)
                {
                    Debug.Log(string.Format("Received = " + received));
                    return Errors.None;
                }
            }
        }

        catch (ArgumentNullException ane)
        {
            // Send/Receive : buffer a la valeur null
            Debug.Log(string.Format("ArgumentNullException : {0}", ane));
            return Errors.ToBeDetermined; // Unknown ?
        }
        catch (SocketException se)
        {
            // Send/Receive : Une erreur s’est produite pendant la tentative d’accès au socket
            Debug.Log(string.Format("SocketException : {0}", se));
            return Errors.Socket;
        }
        catch (ObjectDisposedException ode)
        {
            // Send/Receive : Socket a été fermé
            Debug.Log(string.Format("ObjectDisposedException : {0}", ode));
            return Errors.Socket;
        }
        catch (SecurityException se)
        {
            // Receive : Un appelant de la pile des appels ne dispose pas des autorisations requises
            Debug.Log(string.Format("SecurityException : {0}", se));
            return Errors.ToBeDetermined; // Unknown ?
        }
        catch (Exception e)
        {
            Debug.Log(string.Format("Unexpected exception : {0}", e));
            return Errors.Unknown;
        }
    }
}
