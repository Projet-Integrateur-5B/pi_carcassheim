namespace Assets;

using System.Text;
using Newtonsoft.Json;

public enum Errors : int
{
    None = 0,
    Unknown = -1,
    Socket = 1,
    Format = 2,
    ConfigFile = 3,
    Receive = 4,
    Data = 5,
    ToBeDetermined = 999
}
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
    RoomStart = 10
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
        var dataString = string.Join(string.Empty, original.Data);
        var header = new Packet(original.Type, original.IdRoom, original.IdMessage, original.Final, original.IdPlayer, Array.Empty<string>());
        var packetLength = original.Data.Length;

        var headerBytes = header.PacketToByteArray(); // header to bytes
        var headerBytesLength = headerBytes.Length; // length
        var headerBytesMaxLength = Packet.MaxPacketSize - headerBytesLength; // max length allowed

        var dataBytes = Encoding.ASCII.GetBytes(dataString);
        var dataBytesTotalLength = dataBytes.Length;

        var packet = headerBytes.ByteArrayToPacket();

        int dataLength;
        while (true)
        {
            dataLength = Encoding.ASCII.GetBytes(original.Data[0]).Length;
            if (dataLength < headerBytesMaxLength)
            {
                var list = new List<string>(packet.Data.ToList())
                {
                    original.Data[0]
                };
                packet.Data = list.ToArray();
                packetLength--;
                headerBytesMaxLength = headerBytesMaxLength - dataLength - 3;
                original.Data = original.Data.Where((source, index) => index != 0).ToArray();
            }
            else
            {
                packet.Final = false;
                packets.Add(packet);
                packet = headerBytes.ByteArrayToPacket();
                headerBytesMaxLength = Packet.MaxPacketSize - headerBytesLength;
            }
            if (packetLength == 0)
            {
                packet.Final = true;
                packets.Add(packet);
                break;
            }
        }
        return packets;
    }

    public static Packet Catenate(this List<Packet> packets)
    {
        var original = packets[0];
        foreach (var packet in packets.Skip(1))
        {
            original.Data = original.Data.Concat(packet.Data).ToArray();
        }
        return original;
    }
}
