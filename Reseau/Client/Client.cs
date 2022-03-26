namespace Client;

using System.Configuration;
using System.Net;
using System.Net.Sockets;
using Assets;

public partial class Client
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

    public static void Disconnection(Socket sender)
    {
        // Deconnect to a remote device.
        try
        {
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

    public static int Communication(Socket sender, byte idMessage, string data)
    {
        var bytes = new byte[Packet.MaxPacketSize];
        var original = new Packet(false, 0, idMessage, true, 999, data);
        var packets = original.Split();

        var count_errors = 0;
        for (var i = 0; i < 1; i++)
        {
            foreach (var packet in packets)
            {
                var packetAsBytes = packet.PacketToByteArray();

                // Send the data through the socket.
                var bytesSent = sender.Send(packetAsBytes);
                Console.WriteLine("Sent {0} bytes =>\t" + packet, bytesSent);
            }

            // Receive the response from the remote device.
            bytes = new byte[Packet.MaxPacketSize];
            var bytesRec = sender.Receive(bytes);
            var packetAsBytes2 = new byte[bytesRec];
            Array.Copy(bytes, packetAsBytes2, bytesRec);

            try
            {
                var recv = packetAsBytes2.ByteArrayToPacket();
                if (recv.Status)
                {
                    Console.WriteLine("Read {0} bytes => \tpermission accepted \n", bytesRec);
                }
                else
                {
                    Console.WriteLine("Read {0} bytes => \tpermission denied \n", bytesRec);
                }
            }
            catch (Exception) // received wrong format, deserialize failed
            {
                count_errors++;
                i = -1; // try sending again, except if :
                if (count_errors == 3) // has failed 3 times, return code error -1
                {
                    Console.WriteLine("An errors has occured, please try again !");
                    return -1;
                }
            }
        }
        return 0;
    }
}
