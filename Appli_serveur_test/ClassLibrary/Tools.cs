namespace ClassLibrary;

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

}



