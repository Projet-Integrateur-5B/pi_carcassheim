using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security;
using UnityEngine;


public static partial class Client
{
    public static Errors Connection(ref Socket socket)
    {
        // TODO : trycatch lors de la récupération des données de config
        /*var port = ConfigurationManager.AppSettings.Get("ServerPort");
        var ip = ConfigurationManager.AppSettings.Get("ServerIP");*/

        try
        {
            Debug.Log(string.Format("Client is setting up..."));

            // Establish the remote endpoint for the socket.
            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());

            // /!\ VERSION TEMPORAIRE (BARBARE)
            var ipAddress = IPAddress.Parse("185.155.93.105");
            var remoteEP = new IPEndPoint(ipAddress, 19000);
            // /!\

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
            var original = new Packet(false, 0, (byte)idMessage, true, 999, data);

            packets = original.Split(ref error_value);
            if (error_value != Errors.None)
            {
                // TODO : Split => handle error
                return Errors.Data;
            }

            var count_errors = 0;
            while (true)
            {
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

                // Receive the response from the remote device.
                bytes = new byte[Packet.MaxPacketSize];
                var bytesRec = socket.Receive(bytes);
                var packetAsBytes = new byte[bytesRec];

                try
                {
                    Array.Copy(bytes, packetAsBytes, bytesRec);
                    received = packetAsBytes.ByteArrayToPacket(ref error_value);
                    if (error_value != Errors.None)
                    {
                        // TODO : ByteArrayToPacket => handle error
                        return Errors.Data;
                    }

                    Debug.Log(string.Format(
                        received.Status
                            ? "Read {0} bytes => \tpermission accepted \n"
                            : "Read {0} bytes => \tpermission denied \n", bytesRec));
                    return Errors.None;
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
                    return Errors.Receive;
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
