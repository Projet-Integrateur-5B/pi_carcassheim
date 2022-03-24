namespace Assets;

using System.Text;
using Newtonsoft.Json;

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

    public static List<Packet> Prepare(this Packet original)
    {
        var packets = new List<Packet>();
        original ??= new Packet();

        // within authorized range of size
        if (original.PacketToByteArray().Length < Packet.MaxPacketSize)
        {
            packets.Add(original);
            return packets;
        }

        var header = original; // copy the original packet
        header.Data = ""; // empty the data field (= keep the packet's header)

        var headerBytes = header.PacketToByteArray(); // header to bytes
        var headerBytesLength = headerBytes.Length; // length

        var headerBytesMaxLength = Packet.MaxPacketSize - headerBytesLength; // max length allowed
        var dataString = JsonConvert.SerializeObject(original.Data);
        dataString = dataString[1..^1];
        // test = test.Substring(1, test.Length - 2);
        var dataBytes = Encoding.ASCII.GetBytes(dataString);
        var dataBytesTotalLength = dataBytes.Length;

        for (var i = 0; i < dataBytesTotalLength; i += headerBytesMaxLength)
        {
            var packet = headerBytes.ByteArrayToPacket();
            if (i + headerBytesMaxLength > dataBytesTotalLength)
            {
                packet.Data = dataString[i..dataBytesTotalLength];
                Console.WriteLine(dataString[i..dataBytesTotalLength]);
                // packet.Data = dataString.Substring(i, dataBytesTotalLength-i);
            }
            else
            {
                packet.Data = dataString.Substring(i, headerBytesMaxLength);
                Console.WriteLine(dataString.Substring(i, headerBytesMaxLength));
            }
            packets.Add(packet);
        }
        return packets;
    }
}
