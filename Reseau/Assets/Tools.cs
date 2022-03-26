namespace Assets;

using System.Text;
using Newtonsoft.Json;

public enum IdMessage : byte
{
    Default = 0,
    Connection = 1,
    Disconnection = 2,
    Signup = 3,
    Statistics = 4,
    RoomList = 5,
    RoomJoin = 6,
    RoomLeave = 7,
    RoomReady = 8,
    RoomSettings = 9,
    RoomEdit = 10
}

public static class Tools
{

    public static byte[] PacketToByteArray(this Packet packet)
    {
        var serialized = JsonConvert.SerializeObject(packet);
        return Encoding.ASCII.GetBytes(serialized);
    }

    public static Packet ByteArrayToPacket(this byte[] byteArray)
    {
        var packetAsJson = Encoding.ASCII.GetString(byteArray);
        return JsonConvert.DeserializeObject<Packet>(packetAsJson) ?? new Packet();
    }

    public static List<Packet> Split(this Packet original)
    {
        var packets = new List<Packet>();

        // within authorized range of size
        if (original.PacketToByteArray().Length < Packet.MaxPacketSize)
        {
            packets.Add(original);
            return packets;
        }

        var dataString = original.Data;
        var header = original; // copy the original packet
        header.Data = ""; // empty the data field (= keep the packet's header)

        var headerBytes = header.PacketToByteArray(); // header to bytes
        var headerBytesLength = headerBytes.Length; // length
        var headerBytesMaxLength = Packet.MaxPacketSize - headerBytesLength; // max length allowed

        var dataBytes = Encoding.ASCII.GetBytes(dataString);
        var dataBytesTotalLength = dataBytes.Length;

        for (var i = 0; i < dataBytesTotalLength; i += headerBytesMaxLength - 1)
        {
            var packet = headerBytes.ByteArrayToPacket();
            if (i + headerBytesMaxLength - 1 > dataBytesTotalLength)
            {
                packet.Final = true;
                packet.Data = dataString[i..dataBytesTotalLength];
                Console.WriteLine(dataString[i..dataBytesTotalLength]);
                // dataString.Substring(i, dataBytesTotalLength - i);
            }
            else
            {
                packet.Final = false;
                packet.Data = dataString.Substring(i, headerBytesMaxLength - 1);
                Console.WriteLine(dataString.Substring(i, headerBytesMaxLength - 1));
                // dataString.Substring(i, headerBytesMaxLength - 1);
            }
            packets.Add(packet);
        }
        return packets;
    }

    public static Packet Catenate(this List<Packet> packets)
    {
        var original = packets[0];
        foreach (var packet in packets.Skip(1))
        {
            original.Data += packet.Data;
        }
        return original;
    }
}
