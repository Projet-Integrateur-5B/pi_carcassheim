namespace Client;

using System.Net;
using System.Net.Sockets;
using System.Security;
using Assets;

public static partial class Client
{
    /// <summary>
    ///     connction du client au serveur
    /// </summary>
    /// <param name="socket"> socket où enregistrer le connection</param>
    /// <param name="port"> numéro de port</param>
    /// <returns></returns>
    private static Tools.Errors Connection(ref Socket socket, int port)
    {
        // TODO : trycatch lors de la récupération des données de config
        // Version : Unity
        /*TextAsset contents = Resources.Load<TextAsset>("network/config");
        Parameters parameters = JsonConvert.DeserializeObject<Parameters>(contents.ToString());*/

        try
        {
            Console.WriteLine("Client is setting up...");

            // Establish the remote endpoint for the socket.
            // Version : Unity
            /*IPAddress ipAddress = IPAddress.Parse(parameters.ServerIP);
            var remoteEP = new IPEndPoint(ipAddress, parameters.ServerPort);*/
            // Version : Local
            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = ipHostInfo.AddressList[0];
            var remoteEP = new IPEndPoint(ipAddress, port);


            // Create a TCP/IP  socket.
            socket = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint. Catch any errors.
            try
            {
                socket.Connect(remoteEP);
                socket.ReceiveTimeout = 15000;
                Console.WriteLine("Client is connected to {0}", socket.RemoteEndPoint);
                return Tools.Errors.None;
            }
            catch (ArgumentNullException ane)
            {
                // Connect : remoteEP is null
                Console.WriteLine("ArgumentNullException : {0}", ane);
                return Tools.Errors.ToBeDetermined; // Unknown ?
            }
            catch (SocketException se)
            {
                // Connect : An error occurred when attempting to access the socket
                Console.WriteLine("SocketException : {0}", se);
                return Tools.Errors.Socket;
            }
            catch (ObjectDisposedException ode)
            {
                // Connect : The Socket has been closed
                Console.WriteLine("ObjectDisposedException : {0}", ode);
                return Tools.Errors.Socket;
            }
            catch (SecurityException se)
            {
                // Connect : A caller higher in the call stack does not have permission for the requested operation
                Console.WriteLine("SecurityException : {0}", se);
                return Tools.Errors.ToBeDetermined; // Unknown ?
            }
            catch (InvalidOperationException se)
            {
                // Connect : The Socket has been placed in a listening state by calling Listen(Int32).
                Console.WriteLine("InvalidOperationException : {0}", se);
                return Tools.Errors.Socket;
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e);
                return Tools.Errors.Unknown;
            }
        }
        catch (ArgumentNullException ane)
        {
            // IPAddress.Parse : ipAddress a la valeur null
            // IPEndPoint : address a la valeur null
            Console.WriteLine("ArgumentNullException : {0}", ane);
            return Tools.Errors.ConfigFile;
        }
        catch (FormatException fe)
        {
            // IPAddress.Parse : ipAddress n’est pas une adresse IP valide
            Console.WriteLine("FormatException : {0}", fe);
            return Tools.Errors.ConfigFile;
        }
        catch (ArgumentOutOfRangeException aoore)
        {
            // IPEndPoint : port est inférieur à MinPort; port est supérieur à MaxPort; address est inférieur à 0 ou supérieur à 0x00000000FFFFFFFF
            Console.WriteLine("ArgumentOutOfRangeException : {0}", aoore);
            return Tools.Errors.ConfigFile;
        }
        catch (SocketException se)
        {
            // Socket : La combinaison de addressFamily, socketType et protocolType crée un socket non valide
            Console.WriteLine("SocketException : {0}", se);
            return Tools.Errors.ConfigFile;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            return Tools.Errors.Unknown;
        }
    }

    /// <summary>
    ///     déconnexion du client avec le serveur
    /// </summary>
    /// <param name="socket">socket à utilisé</param>
    /// <returns></returns>
    private static Tools.Errors Disconnection(Socket socket)
    {
        var original = new Packet();
        socket.Communication(ref original, Tools.IdMessage.Logout, Array.Empty<string>());
        // if Errors.Format = ignore, not expecting to receive anything from the server
        // if Errors.Socket = ignore, already disconnected from the server

        // Deconnect to a remote device.
        try
        {
            Console.WriteLine("Closing connection...");
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            return Tools.Errors.None;
        }
        catch (SocketException se)
        {
            // Shutdown : An error occurred when attempting to access the socket
            Console.WriteLine("SocketException : {0}", se);
            return Tools.Errors.Socket;
        }
        catch (ObjectDisposedException ode)
        {
            // Shutdown : The Socket has been closed
            Console.WriteLine("ObjectDisposedException : {0}", ode);
            return Tools.Errors.None;
        }
        catch (Exception e)
        {
            Console.WriteLine("Unexpected exception : {0}", e);
            return Tools.Errors.Unknown;
        }
    }

    /// <summary>
    ///     envoye des message et réception
    /// </summary>
    /// <param name="socket"> socket à utilisé</param>
    /// <param name="received"> variable où enregistré le packet recu </param>
    /// <param name="idMessage"> numéro du message </param>
    /// <param name="data"> data à envoyée </param>
    /// <returns></returns>
    private static Tools.Errors Communication(this Socket socket, ref Packet received,
        Tools.IdMessage idMessage, string[] data)
    {
        try
        {
            byte[]? bytes = null;
            var error_value = Tools.Errors.None;
            var original = new Packet(idMessage, Tools.Errors.None, 999, 0, data);

            // Send the data through the socket.
            bytes = original.PacketToByteArray(ref error_value);
            if (error_value != Tools.Errors.None)
            {
                // TODO : PacketToByteArray => handle error
                return Tools.Errors.Data;
            }

            var bytesSent = socket.Send(bytes);
            Console.WriteLine("Sent {0} bytes =>\t" + original, bytesSent);

            // check if an answer is needed
            if (idMessage == Tools.IdMessage.Logout)
            {
                return Tools.Errors.None;
            }

            // Receive the response from the remote device.
            bytes = new byte[Packet.MaxPacketSize];
            var bytesRec = socket.Receive(bytes);
            var packetAsBytes = new byte[bytesRec];

            Array.Copy(bytes, packetAsBytes, bytesRec);
            var temp = packetAsBytes.ByteArrayToPacket(ref error_value);
            received = temp[0];
            if (error_value != Tools.Errors.None)
            {
                // TODO : ByteArrayToPacket => handle error
                return Tools.Errors.Data;
            }

            Console.WriteLine("Read {0} bytes => \t" + received, bytesRec);
            return Tools.Errors.None;
        }
        catch (ArgumentNullException ane)
        {
            // Send/Receive : buffer a la valeur null
            Console.WriteLine("ArgumentNullException : {0}", ane);
            return Tools.Errors.ToBeDetermined; // Unknown ?
        }
        catch (SocketException se)
        {
            // Send/Receive : Une erreur s’est produite pendant la tentative d’accès au socket
            Console.WriteLine("SocketException : {0}", se);
            return Tools.Errors.Socket;
        }
        catch (ObjectDisposedException ode)
        {
            // Send/Receive : Socket a été fermé
            Console.WriteLine("ObjectDisposedException : {0}", ode);
            return Tools.Errors.Socket;
        }
        catch (SecurityException se)
        {
            // Receive : Un appelant de la pile des appels ne dispose pas des autorisations requises
            Console.WriteLine("SecurityException : {0}", se);
            return Tools.Errors.ToBeDetermined; // Unknown ?
        }
        catch (Exception e)
        {
            Console.WriteLine("Unexpected exception : {0}", e);
            return Tools.Errors.Unknown;
        }
    }

    [Serializable]
    public class Parameters
    {
        public int ServerPort { get; set; }
        public string ServerIP { get; set; } = "";
    }
}
