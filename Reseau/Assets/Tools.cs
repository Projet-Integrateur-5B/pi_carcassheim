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
    RoomEdit = 10
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

        var dataString = original.Data;
        var header = original; // copy the original packet
        header.Data = ""; // empty the data field (= keep the packet's header)

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

        for (var i = 0; i < dataBytesTotalLength; i += headerBytesMaxLength - 1)
        {
            var packet = headerBytes.ByteArrayToPacket(ref error);
            if (error != Errors.None)
            {
                // TODO : ByteArrayToPacket => handle error
                return new List<Packet>();
            }
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
