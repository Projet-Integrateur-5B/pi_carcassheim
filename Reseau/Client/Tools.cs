namespace Client;

using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

public static class Tools
{
    public static byte[] PacketToByteArray(this Packet? packet)
    {
        var jso = new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
        var jsonString = JsonSerializer.Serialize(packet, jso);
        return Encoding.ASCII.GetBytes(jsonString);
    }

    public static Packet ByteArrayToPacket(this byte[]? byteArray)
    {
        if (byteArray == null)
        {
            return new Packet();
        }
        var packetAsJson = Encoding.ASCII.GetString(byteArray);
        return JsonSerializer.Deserialize<Packet>(packetAsJson) ?? new Packet();
    }

    public static List<Packet> Prepare(this Packet? original)
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
        var jso = new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
        var dataString = JsonSerializer.Serialize(original.Data, jso);
        dataString = dataString.Substring(1, dataString.Length - 2);
        // test = test.Substring(1, test.Length - 2);
        var dataBytes = Encoding.ASCII.GetBytes(dataString);
        var dataBytesTotalLength = dataBytes.Length;

        for (var i = 0; i < dataBytesTotalLength; i += headerBytesMaxLength)
        {
            var packet = headerBytes.ByteArrayToPacket();
            if (i + headerBytesMaxLength > dataBytesTotalLength)
            {
                packet.Data = dataString.Substring(i, dataBytesTotalLength-i);
                Console.WriteLine(dataString.Substring(i,dataBytesTotalLength-i));
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



