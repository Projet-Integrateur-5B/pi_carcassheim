using System.Text.Json;
using System.Net;
using System;

public class Packet
{
    // private bool _type; // 0 : client -> server ; 1 : server -> client
    //
    // // type == 0 : client -> server
    // private IPAddress _ipAddress;
    // private int _port;
    // private int _idRoom; // default : 0 -> not destined to a room
    // private int _idMessage; // à définir
    //
    // // type == 1 : server -> client
    // private bool _status; // 0 : error ; 1 : success
    // private int _permission; // 0 : unused/error ; 1 : permission ; 2 : confirmation
    //
    // // common to both
    // private int _idPlayer;
    // private String data; // à définir
    
    public bool Type { get; } // 0 : client -> server ; 1 : server -> client
    
    // type == 0 : client -> server
    public IPAddress IpAddress { get; set; }
    public ushort Port { get; set; }
    public ulong IdRoom { get; set; }
    public byte IdMessage { get; set; }
    
    // type == 1 : server -> client
    public bool Status { get; set; }
    public byte Permission { get; set; }
    
    // common to both
    public ulong IdPlayer { get; set; }
    public String Data { get; set; }
    
    
    
    // basic
    public Packet() {}

    // type == 0 : client -> server
    public Packet(bool type, int port, IPAddress ipAddress, int idPlayer, int idRoom, int idMessage, String data)
    {
        _type = type;
        _ipAddress = ipAddress;
        _port = port;
        _idPlayer = idPlayer;
        _idRoom = idRoom;
        _idMessage = idMessage;
        _data = data;
    }
    
    // type == 1 : server -> client
    public Packet(bool type, bool status, int permission, int idPlayer, String data)
    {
        _type = type;
        _status = status;
        _permission = permission;
        _idPlayer = idPlayer;
        _data = data;
    }

    public static void Main()
    {
        Packet packet = new Packet(1, 1, 1, 1, "test");
        string jsonString = JsonSerializer.Serialize(packet);
        Console.WriteLine(jsonString);
    }
}