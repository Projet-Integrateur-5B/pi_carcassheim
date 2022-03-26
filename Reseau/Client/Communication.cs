namespace Client;

using System.Net.Sockets;
using Assets;

public partial class Client
{
    public static int Communication(Socket sender, byte idMessage, string data)
    {
        var bytes = new byte[Packet.MaxPacketSize];
        var original = new Packet(false, 0, idMessage, true, 999, data);
        var packets = original.Prepare();

        int count_errors = 0;
        for (int i = 0; i<1; i++)
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
            catch (Exception)
            {
                count_errors++;
                i = -1;
                if (count_errors == 3)
                {
                    Console.WriteLine("An errors has occured, please try again !");
                    return -1;
                }
            }
        }
        return 0;
    }
}
