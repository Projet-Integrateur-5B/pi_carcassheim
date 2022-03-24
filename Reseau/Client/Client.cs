namespace Client;

using System.Net;
using System.Net.Sockets;

public class Client
{
    //private static Packet packet = new();
    private const int Port = 19000;

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
            var remoteEP = new IPEndPoint(ipAddress, Port);

            // Create a TCP/IP  socket.
            var sender = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint. Catch any errors.
            try
            {
                sender.Connect(remoteEP);
                Console.WriteLine("Client is connected to {0}", sender.RemoteEndPoint);

                var original = new Packet(false, 0, number, 999, data);
                var packets = original.Prepare();

                foreach (var packet in packets)
                {
                    var packetAsBytes = packet.PacketToByteArray();
                    bytes = new byte[packetAsBytes.Length];

                    // Send the data through the socket.
                    var bytesSent = sender.Send(packetAsBytes);
                    Console.WriteLine("Sent {0} bytes =>\t" + packet, bytesSent);
                }

                // Receive the response from the remote device.
                var bytesRec = sender.Receive(bytes);
                var packetAsBytes2 = new byte[bytesRec];
                Array.Copy(bytes, packetAsBytes2, bytesRec);
                var recv = packetAsBytes2.ByteArrayToPacket();
                if (recv.Status)
                {
                    Console.WriteLine("Read {0} bytes => permission accepted \n", bytesRec);
                }
                else
                {
                    Console.WriteLine("Read {0} bytes => permission denied \n", bytesRec);
                }

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
        //string[] data = { "petit", "test" };
        var data = "petit test";
        StartClient(nb, data);
        return 0;
    }
}
