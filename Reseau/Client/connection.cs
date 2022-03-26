namespace Client;

using System.Configuration;
using System.Net;
using System.Net.Sockets;
using Assets;

public class Connect
{
    public static Socket? Connection()
    {
        var port = ConfigurationManager.AppSettings.Get("ServerPort");
        var ip = ConfigurationManager.AppSettings.Get("ServerIP");
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
                return sender;

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
        return null;
    }
}
