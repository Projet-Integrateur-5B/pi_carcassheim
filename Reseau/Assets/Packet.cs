namespace Assets;

using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

public class Packet
{
    private const string Localhost = "127.0.0.1";
    private const string DataEof = "<EOF>";

    public Packet()
    {
        this.Type = false;
        this.IpAddress = new string(Localhost);
        this.Data = new string("" + DataEof);
    }

    public Packet(bool type, string ipAddress, ushort port, ulong idRoom, byte idMessage,
        bool status, byte permission, ulong idPlayer, string data)
    {
        this.Type = type;
        this.IpAddress = new string(ipAddress);
        this.Port = port;
        this.IdRoom = idRoom;
        this.IdMessage = idMessage;
        this.Status = status;
        this.Permission = permission;
        this.IdPlayer = idPlayer;
        this.Data = new string(data + DataEof);
    }

    // type == false (client -> server)
    public Packet(bool type, string ipAddress, ushort port, ulong idRoom, byte idMessage,
        ulong idPlayer, string data)
    {
        this.Type = type;
        this.IpAddress = new string(ipAddress);
        this.Port = port;
        this.IdRoom = idRoom;
        this.IdMessage = idMessage;
        this.IdPlayer = idPlayer;
        this.Data = new string(data + DataEof);
    }

    // type == true (server -> client)
    public Packet(bool type, bool status, byte permission, ulong idPlayer,
        string data)
    {
        this.Type = type;
        this.Status = status;
        this.Permission = permission;
        this.IdPlayer = idPlayer;
        this.Data = new string(data + DataEof);
        this.IpAddress = new string(Localhost); // unused, escape errors
    }

    public bool Type { get; set; } // false (client -> server) - true (server -> client)

    // type == false (client -> server)
    /* public IPAddress IpAddress { get; set; } */
    public string IpAddress { get; set; }
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
        var jso = new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
        var jsonString = JsonSerializer.Serialize(this, jso);
        return Encoding.ASCII.GetBytes(jsonString);
    }

    public static Packet? Deserialize(byte[] packetAsBytes)
    {
        var packetAsJson = Encoding.ASCII.GetString(packetAsBytes);
        return JsonSerializer.Deserialize<Packet>(packetAsJson);
    }

    public override string ToString() => "Type:" + this.Type + "; "
                                         + "IpAddress:" + this.IpAddress + "; "
                                         + "Port:" + this.Port + "; "
                                         + "IdRoom:" + this.IdRoom + "; "
                                         + "IdMessage:" + this.IdMessage + "; "
                                         + "Status:" + this.Status + "; "
                                         + "Permission:" + this.Permission + "; "
                                         + "IdPlayer:" + this.IdPlayer + "; "
                                         + "Data:" + this.Data + "; ";
}
