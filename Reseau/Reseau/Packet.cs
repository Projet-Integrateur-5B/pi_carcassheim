using System.Net;
using System.Text;
using System.Text.Json;

namespace Reseau;

public class Packet
{
    private static string _localhost = "127.0.0.1";
    public Packet()
    {
        Type = false;
        IpAddress = IPAddress.Parse(_localhost);
        Data = new string("");
    }

    public Packet(bool type, IPAddress ipAddress, ushort port, ulong idRoom, byte idMessage, bool status, byte permission, ulong idPlayer, string data)
    {
        Type = type;
        IpAddress = ipAddress;
        Port = port;
        IdRoom = idRoom;
        IdMessage = idMessage;
        Status = status;
        Permission = permission;
        IdPlayer = idPlayer;
        Data = new string(data);
    }

    // type == false (client -> server)
    public Packet(bool type, IPAddress ipAddress, ushort port, ulong idRoom, byte idMessage, ulong idPlayer,
        string data)
    {
        Type = type;
        IpAddress = ipAddress;
        Port = port;
        IdRoom = idRoom;
        IdMessage = idMessage;
        IdPlayer = idPlayer;
        Data = new string(data);
    }

    // type == true (server -> client)
    public Packet(bool type, bool status, byte permission, ulong idPlayer, string data)
    {
        Type = type;
        Status = status;
        Permission = permission;
        IdPlayer = idPlayer;
        Data = new string(data);
        IpAddress = IPAddress.Parse(_localhost); // unused
    }

    public bool Type { get; set; } // false (client -> server) - true (server -> client)

    // type == false (client -> server)
    public IPAddress IpAddress { get; set; }
    public ushort Port { get; set; }
    public ulong IdRoom { get; set; } // default : 0 -> not destined to a room
    public byte IdMessage { get; set; } // à définir

    // type == true (server -> client)
    public bool Status { get; set; } // 0 : error ; 1 : success
    public byte Permission { get; set; } // 0 : unused/error ; 1 : permission ; 2 : confirmation

    // common to both
    public ulong IdPlayer { get; set; }
    public string Data { get; set; } // à définir

    public byte[] Serialize()
    {
        return JsonSerializer.SerializeToUtf8Bytes(this);
    }

    public static Packet? Deserialize(byte[] packetAsBytes)
    {
        var packetAsJson = Encoding.Default.GetString(packetAsBytes);
        return JsonSerializer.Deserialize<Packet>(packetAsJson);
    }

    private static void Main()
    {
    }
}