namespace Assets;

using System.ComponentModel;
using System.Text;
using Newtonsoft.Json;

public enum Errors
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

    public static byte[] PacketToByteArray(this Packet packet, ref Errors error)
    {
        if (!Enum.IsDefined(typeof(Errors), error))
        {
            throw new InvalidEnumArgumentException(nameof(error), (int)error, typeof(Errors));
        }

        try
        {
            var packetAsJsonString = JsonConvert.SerializeObject(packet);
            var packetAsBytes = Encoding.ASCII.GetBytes(packetAsJsonString);
            error = Errors.None;
            return packetAsBytes;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            error = Errors.Format;
            return Array.Empty<byte>();
        }
    }

    public static Packet ByteArrayToPacket(this byte[] byteArray, ref Errors error)
    {
        if (!Enum.IsDefined(typeof(Errors), error))
        {
            throw new InvalidEnumArgumentException(nameof(error), (int)error, typeof(Errors));
        }

        try
        {
            var packetAsJson = Encoding.ASCII.GetString(byteArray);
            var packet = JsonConvert.DeserializeObject<Packet>(packetAsJson) ?? throw new ArgumentNullException(packetAsJson);
            error = Errors.None;
            return packet;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            error = Errors.Format;
            return new Packet();
        }
    }

    public static List<Packet> Split(this Packet original, ref Errors error)
    {
        var packets = new List<Packet>();

        // within authorized range of size
        var originalBytesLength = original.PacketToByteArray(ref error).Length;
        if (error != Errors.None)
        {
            // TODO : PacketToByteArray => handle error
            return packets; // empty
        }

        if (originalBytesLength < Packet.MaxPacketSize)
        {
            packets.Add(original);
            return packets;
        }
        var dataString = string.Join(string.Empty, original.Data);
        var header = new Packet(original.Type, original.IdRoom, original.IdMessage, original.Final, original.IdPlayer, Array.Empty<string>());
        var packetLength = original.Data.Length;

        var headerBytes = header.PacketToByteArray(ref error); // header to bytes
        if (error != Errors.None)
        {
            // TODO : PacketToByteArray => handle error
            return packets; // empty
        }
        var headerBytesLength = headerBytes.Length; // length
        var headerBytesMaxLength = Packet.MaxPacketSize - headerBytesLength; // max length allowed

        var dataBytes = Encoding.ASCII.GetBytes(dataString);
        var dataBytesTotalLength = dataBytes.Length;

        var packet = headerBytes.ByteArrayToPacket(ref error);
        if (error != Errors.None)
        {
            // TODO : ByteArrayToPacket => handle error
            return new List<Packet>();
        }

        int dataLength;
        while (true)
        {
            try
            {
                dataLength = Encoding.ASCII.GetBytes(original.Data[0]).Length;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

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
                packet = headerBytes.ByteArrayToPacket(ref error);
                if (error != Errors.None)
                {
                    // TODO : ByteArrayToPacket => handle error
                    return new List<Packet>();
                }
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
