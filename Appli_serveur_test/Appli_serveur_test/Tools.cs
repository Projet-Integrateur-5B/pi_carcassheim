namespace Server;

using Assets;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public static class Tools
{

    public static byte[] PacketToByteArray(this Packet? packet)
    {
        if (packet == null)
        {
            return Array.Empty<byte>();
        }
        var bf = new BinaryFormatter();
        using (var ms = new MemoryStream())
        {
            bf.Serialize(ms, packet);
            return ms.ToArray();
        }
    }

    public static Packet ByteArrayToPacket(this byte[]? byteArray)
    {
        if (byteArray == null)
        {
            return new Packet();
        }
        using (var memStream = new MemoryStream())
        {
            var binForm = new BinaryFormatter();
            memStream.Write(byteArray, 0, byteArray.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            var obj = (Packet)binForm.Deserialize(memStream);
            return obj;
        }
    }
}

