namespace Client;

using Assets;
using System.Net;
using System.Net.Sockets;
using System.Configuration;


public class ClientMain
{
    public static void StartClient(byte idMessage, string data)
    {
        // get config from file
        var Port = ConfigurationManager.AppSettings.Get("ServerPort");
        var IP = ConfigurationManager.AppSettings.Get("ServerIP");

        // Data buffer for incoming data.

        // Connect to a remote device.
        try
        {
            Console.WriteLine("Client is setting up...");

            // Establish the remote endpoint for the socket.
            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = ipHostInfo.AddressList[0];
            var remoteEP = new IPEndPoint(ipAddress, Packet.Port);


            // Create a TCP/IP  socket.
            var sender = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint. Catch any errors.
            try
            {
                sender.Connect(remoteEP);
                Console.WriteLine("Client is connected to {0}", sender.RemoteEndPoint);

                // envoye / reception message
                Client.StartEnvoye(sender, idMessage, data);

                //Release the socket.
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                Console.WriteLine("Closing connection...");
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane);
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
    public static int Main()
    {
        const byte idMessage = 2;
        const string data = "petit test";
        StartClient(idMessage, data);
        return 0;
    }
}
