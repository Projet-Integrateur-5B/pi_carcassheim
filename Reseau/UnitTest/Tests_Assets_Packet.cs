namespace UnitTest;

using System;
using System.Text;
using Assets;
using NUnit.Framework;

public class TestsAssetsPacket
{
    private const string DataEof = "<EOF>";
    private Packet original = new();
    private string originalAsString = new("");

    [SetUp]
    public void Setup()
    {
        this.original = new Packet(true, 0, 0, true, 1, 1, "test");
        this.originalAsString = "{\"Type\":true," +
                                "\"IdRoom\":0,\"IdMessage\":0,\"Status\":true,\"Permission\":1," +
                                "\"IdPlayer\":1,\"Data\":\"test" + DataEof + "\"}";
    }

    [Test]
    public void TestPacketDataEofSuccess()
    {
        var packet = new Packet();
        Assert.AreEqual(packet.Data, "<EOF>");
        Assert.AreEqual(this.original.Data, "test<EOF>");

        var sb = new StringBuilder();
        sb.Append(packet.Data);
        var content = sb.ToString();

        var result = false || content.IndexOf("<EOF>", StringComparison.Ordinal) > -1;
        Assert.IsTrue(result);
    }

    [Test]
    public void TestPacketDeserializationSuccess()
    {
        var originalAsBytes = Encoding.ASCII.GetBytes(this.originalAsString);
        var result = Packet.Deserialize(originalAsBytes) ?? new Packet();

        Assert.AreEqual(this.original.Type, result.Type);
        Assert.AreEqual(this.original.IdRoom, result.IdRoom);
        Assert.AreEqual(this.original.IdMessage, result.IdMessage);
        Assert.AreEqual(this.original.Status, result.Status);
        Assert.AreEqual(this.original.Permission, result.Permission);
        Assert.AreEqual(this.original.IdPlayer, result.IdPlayer);
        Assert.AreEqual(this.original.Data, result.Data);
    }

    [Test]
    public void TestPacketSerializationSuccess()
    {
        var originalAsBytes = this.original.Serialize();
        var resultAsString = Encoding.ASCII.GetString(originalAsBytes);

        Assert.AreEqual(this.originalAsString, resultAsString);
    }

    [Test]
    public void TestPacketPrepareMultipacketsSuccess()
    {
        var type = this.original.Type;
        var idRoom = this.original.IdRoom;
        var idMessage = this.original.IdMessage;
        var status = this.original.Status;
        var permission = this.original.Permission;
        var idPlayer = this.original.IdPlayer;

        var packet = new Packet(type, idRoom, idMessage, status, permission,
            idPlayer,
            "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz");
        var p0 = new Packet(type, idRoom, idMessage, status, permission, idPlayer,
            "")
        {
            Data =
            "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopq"
        };
        var p1 = new Packet(type, idRoom, idMessage, status, permission, idPlayer,
            "lmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz");

        var packets = packet.Prepare();
        Assert.AreEqual(p0.Data, packets[0].Data);
        Assert.AreEqual(p1.Data, packets[1].Data);
    }

    [Test]
    public void TestPacketPrepareSolopacketSuccess()
    {
        var packets = this.original.Prepare();
        Assert.AreEqual(this.original, packets[0]);
    }

    [Test]
    public void TestPacketConstructorClienttoserverSuccess()
    {
        var type = this.original.Type;
        var idRoom = this.original.IdRoom;
        var idMessage = this.original.IdMessage;
        var idPlayer = this.original.IdPlayer;
        var data = this.original.Data;

        var packet = new Packet(type, idRoom, idMessage, idPlayer, data);
        Assert.AreEqual(type, packet.Type);
        Assert.AreEqual(idRoom, packet.IdRoom);
        Assert.AreEqual(idMessage, packet.IdMessage);
        Assert.AreEqual(idPlayer, packet.IdPlayer);
        Assert.AreEqual(data + DataEof, packet.Data);
    }

    [Test]
    public void TestPacketConstructorServertoclientSuccess()
    {
        var type = this.original.Type;
        var status = this.original.Status;
        var permission = this.original.Permission;
        var idPlayer = this.original.IdPlayer;
        var data = this.original.Data;

        var packet = new Packet(type, status, permission, idPlayer, data);
        Assert.AreEqual(type, packet.Type);
        Assert.AreEqual(status, packet.Status);
        Assert.AreEqual(permission, packet.Permission);
        Assert.AreEqual(idPlayer, packet.IdPlayer);
        Assert.AreEqual(data + DataEof, packet.Data);
    }

    [Test]
    public void TestPacketTostringSuccess()
    {
        var result =
            "Type:True; IdRoom:0; IdMessage:0; Status:True; Permission:1; IdPlayer:1; Data:test<EOF>;";
        Assert.AreEqual(result, this.original.ToString());
    }
}
