namespace Client;

using System.Net;
using System.Net.Sockets;
using Assets;

public class Client
{
    //private static Packet packet = new();
    public static void StartClient(byte number, string data)
    {
        // Data buffer for incoming data.
        var bytes = new byte[1024];

        // Connect to a remote device.
        try
        {
            // Establish the remote endpoint for the socket.
            // This example uses port 11000 on the local computer.
            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = ipHostInfo.AddressList[0];
            var remoteEP = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP  socket.
            var sender = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint. Catch any errors.
            try
            {
                sender.Connect(remoteEP);

                Console.WriteLine("Socket connected to {0}", sender.RemoteEndPoint);
                var packet = new Packet(false, ipAddress.ToString(), 11000, 0, number, 999, data);
                Console.WriteLine(packet.ToString());

                var packetAsBytes = packet.Serialize();
                bytes = new byte[packetAsBytes.Length];

                // Send the data through the socket.
                var bytesSent = sender.Send(packet.Serialize());

                // Receive the response from the remote device.
                var bytesRec = sender.Receive(bytes);
                packet = Packet.Deserialize(bytes);
                Console.WriteLine(
                    "Type : {0} \nStatus : {1} \nPermission : {2} \n IdPlayer : {3} \nData : {4} \nIpAdress : {5}",
                    packet.Type, packet.Status, packet.Permission, packet.IdPlayer, packet.Data,
                    packet.IpAddress);

                //Release the socket.
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
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
        byte nb = 18;
        var data = "petit test des familles";
        StartClient(nb, data);
        return 0;
    }
}
