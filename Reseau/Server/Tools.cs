namespace Server;

using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Assets;

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
}
