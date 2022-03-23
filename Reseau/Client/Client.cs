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
        var bytes = new byte[Packet.MaxPacketSize];

        // Connect to a remote device.
        try
        {
            Console.WriteLine("Client is setting up...");

            // Establish the remote endpoint for the socket.
            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = ipHostInfo.AddressList[0];
            var remoteEP = new IPEndPoint(ipAddress, Packet.PortPrincipale);

            // Create a TCP/IP  socket.
            var sender = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint. Catch any errors.
            try
            {
                sender.Connect(remoteEP);
                Console.WriteLine("Client is connected to {0}", sender.RemoteEndPoint);

                var original = new Packet(false, ipAddress.ToString(), Packet.PortPrincipale, 0, number, 999, data);
                var packets = original.Prepare();

                foreach (var packet in packets)
                {
                    var packetAsBytes = packet.Serialize();
                    bytes = new byte[packetAsBytes.Length];

                    // Send the data through the socket.
                    var bytesSent = sender.Send(packet.Serialize());
                    Console.WriteLine("Sent {0} bytes =>\t" + packet, bytesSent);
                }

                // Receive the response from the remote device.
                var bytesRec = sender.Receive(bytes);
                var packetAsBytes2 = new byte[bytesRec];
                Array.Copy(bytes, packetAsBytes2, bytesRec);
                var recv = Packet.Deserialize(packetAsBytes2);
                Console.WriteLine("Read {0} bytes => permission : {1} \n", bytesRec,recv.Status);

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
        byte nb = 18;
        var data = "petit test";
        StartClient(nb, data);
        return 0;
    }
}
