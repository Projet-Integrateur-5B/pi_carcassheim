
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using System.Text;


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
        string dataString = UnityEngine.JsonUtility.ToJson(original.Data);
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



