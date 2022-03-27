namespace Client;
using System.Net;
using System.Net.Sockets;
using Assets;
using Newtonsoft.Json;

public partial class Client
{
    private static Socket? Connection()
    {
        /*var port = ConfigurationManager.AppSettings.Get("ServerPort");
        var ip = ConfigurationManager.AppSettings.Get("ServerIP");*/

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

    private static void Disconnection(Socket sender)
    {
        try
        {
            Communication(sender, (byte)IdMessage.Disconnection, "");
        }
        catch (ReceivedInvalidPacketFormatException e)
        {
            Console.WriteLine("ReceivedInvalidPacketFormatException : {0}", e);
        }
        catch (ArgumentNullException ane)
        {
            Console.WriteLine("ArgumentNullException : {0}", ane);
        }
        catch (SocketException se)
        {
            // TODO : handle case : connection is already closed on the server side
            Console.WriteLine("SocketException : {0}", se);
        }
        catch (Exception e) when (e is not ReceivedInvalidPacketFormatException)
        {
            Console.WriteLine("Unexpected exception : {0}", e);
        }

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

    private static Packet Communication(Socket sender, byte idMessage, string data)
    {
        var original = new Packet(false, 0, idMessage, true, 999, data);
        var packets = original.Split();

        var count_errors = 0;
        while (true)
        {
            foreach (var packet in packets)
            {
                // Send the data through the socket.
                var bytesSent = sender.Send(packet.PacketToByteArray());
                Console.WriteLine("Sent {0} bytes =>\t" + packet, bytesSent);
            }

            // Receive the response from the remote device.
            var bytes = new byte[Packet.MaxPacketSize];
            var bytesRec = sender.Receive(bytes);
            var packetAsBytes = new byte[bytesRec];
            Array.Copy(bytes, packetAsBytes, bytesRec);

            try
            {
                var received = packetAsBytes.ByteArrayToPacket();
                Console.WriteLine(
                    received.Status
                        ? "Read {0} bytes => \tpermission accepted \n"
                        : "Read {0} bytes => \tpermission denied \n", bytesRec);
                return received;
            }
            catch (JsonReaderException jre) // received wrong format, deserialize failed
            {
                count_errors++;
                if (count_errors != 3)
                {
                    continue;
                }
                throw new ReceivedInvalidPacketFormatException("" + jre);
            }
        }
    }
}
