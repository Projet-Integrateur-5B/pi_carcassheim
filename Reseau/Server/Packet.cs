namespace Server;

public class Packet
{
    private const string DataEof = "<EOF>";
    public const int MaxPacketSize = 512;

    public Packet()
    {
        this.Type = false;
        this.Data = new string("" + DataEof);
    }

    public Packet(bool type, ulong idRoom, byte idMessage,
        bool status, byte permission, ulong idPlayer, string data)
    {
        this.Type = type;
        this.IdRoom = idRoom;
        this.IdMessage = idMessage;
        this.Status = status;
        this.Permission = permission;
        this.IdPlayer = idPlayer;
        this.Data = new string(data + DataEof);
    }

    // type == false (client -> server)
    public Packet(bool type, ulong idRoom, byte idMessage,
        ulong idPlayer, string data)
    {
        this.Type = type;
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
    }

    public bool Type { get; set; } // false (client -> server) - true (server -> client)

    // type == false (client -> server)
    public ulong IdRoom { get; set; } // default : 0 -> not destined to a room
    public byte IdMessage { get; set; } // à définir

    // type == true (server -> client)
    public bool Status { get; set; } // 0 : error ; 1 : success
    public byte Permission { get; set; } // 0 : unused/error ; 1 : permission ; 2 : confirmation

    // common to both
    public ulong IdPlayer { get; set; }
    public string Data { get; set; } // à définir

    public override string ToString() => "Type:" + this.Type + "; "
                                         + "IdRoom:" + this.IdRoom + "; "
                                         + "IdMessage:" + this.IdMessage + "; "
                                         + "Status:" + this.Status + "; "
                                         + "Permission:" + this.Permission + "; "
                                         + "IdPlayer:" + this.IdPlayer + "; "
                                         + "Data:" + this.Data + ";";
}

