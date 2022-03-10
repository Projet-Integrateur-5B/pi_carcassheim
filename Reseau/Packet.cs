using System.Text.Json;
using System.Net;
using System.Text;

public class Packet
{
    public bool Type { get; } // 0 : client -> server ; 1 : server -> client

    // type == 0 : client -> server
    public IPAddress IpAddress { get; set; }
    public ushort Port { get; set; }
    public ulong IdRoom { get; set; } // default : 0 -> not destined to a room
    public byte IdMessage { get; set; } // à définir

    // type == 1 : server -> client
    public bool Status { get; set; } // 0 : error ; 1 : success
    public byte Permission { get; set; } // 0 : unused/error ; 1 : permission ; 2 : confirmation

    // common to both
    public ulong IdPlayer { get; set; }
    public String Data { get; set; } // à définir



    // basic
    public Packet() { }

    // type == 0 : client -> server
    public Packet(bool type, ushort port, IPAddress ipAddress, ulong idPlayer, ulong idRoom, byte idMessage, String data)
    {
        Type = type;
        IpAddress = ipAddress;
        Port = port;
        IdPlayer = idPlayer;
        IdRoom = idRoom;
        IdMessage = idMessage;
        Data = data;
    }

    // type == 1 : server -> client
    public Packet(bool type, bool status, byte permission, ulong idPlayer, String data)
    {
        Type = type;
        Status = status;
        Permission = permission;
        IdPlayer = idPlayer;
        Data = data;
    }

    public static void Main()
    {
        Packet sendPacket = new Packet(true, true, 1, 1, "test");

        // serialize
        byte[] packetAsBytes = sendPacket.Serialize();

        // insert send & recv packet

        // unserialize
        Packet? recvPacket = Deserialize(packetAsBytes);
    }

    public byte[] Serialize()
    {
        /*string jsonString = JsonSerializer.Serialize<Packet>(this);
        byte[] bytes = Encoding.ASCII.GetBytes(jsonString);*/
        return JsonSerializer.SerializeToUtf8Bytes<Packet>(this);
    }

    public static Packet Deserialize(byte[] packetAsBytes)
    {
        String packetAsJson = Encoding.Default.GetString(packetAsBytes);
        return JsonSerializer.Deserialize<Packet>(packetAsJson);
    }
}