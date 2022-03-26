namespace Client;

using System.Net.Sockets;
using Assets;

public class Client
{
    public static void StartEnvoye(Socket sender, byte idMessage, string data)
    {
        var bytes = new byte[Packet.MaxPacketSize];

        var original = new Packet(false, 0, idMessage, 999, data);
        var packets = original.Prepare();

        foreach (var packet in packets)
        {
            Thread.Sleep(1000);
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
            Console.WriteLine("Read {0} bytes => \tpermission accepted \n", bytesRec);
        }
        else
        {
            Console.WriteLine("Read {0} bytes => \tpermission denied \n", bytesRec);
        }

    }
}
