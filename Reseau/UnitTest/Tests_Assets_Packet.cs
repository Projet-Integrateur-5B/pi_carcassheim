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
        this.original = new Packet(false, 0, 18, false, 0, 999, "test");
        this.originalAsString = "{\"Type\":false,\"IdRoom\":0,\"IdMessage\":18,\"Status\":false,\"Permission\":0," +
                                "\"IdPlayer\":999,\"Data\":\"test" + DataEof + "\"}";
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
        var resultAsBytes = Encoding.ASCII.GetBytes(this.originalAsString);
        var result = resultAsBytes.ByteArrayToPacket();

        Assert.AreEqual(this.original.Type, result.Type);
        Assert.AreEqual(this.original.IdRoom, result.IdRoom);
        Assert.AreEqual(this.original.IdMessage, result.IdMessage);
        Assert.AreEqual(this.original.Status, result.Status);
        Assert.AreEqual(this.original.Permission, result.Permission);
        Assert.AreEqual(this.original.IdPlayer, result.IdPlayer);
        Assert.AreEqual(this.original.Data, result.Data);
    }

    [Test]
    public void TestPacketToByteArraySuccess()
    {
        var resultAsBytes = this.original.PacketToByteArray();
        var resultAsString = Encoding.ASCII.GetString(resultAsBytes);

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

        var packet = new Packet(type, idRoom, idMessage, status, permission, idPlayer,
            "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz");
        var p0 = new Packet(type, idRoom, idMessage, status, permission, idPlayer, "")
        {
            Data =
            "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijkl"
        };
        var p1 = new Packet(type, idRoom, idMessage, status, permission, idPlayer,
            "mnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz");

        var packets = packet.Prepare();
        Console.WriteLine(p0.Data);
        Console.WriteLine(packets[0].Data);
        Assert.AreEqual(p0.Data, packets[0].Data);
        Assert.AreEqual(p1.Data, packets[1].Data);
    }

    [Test]
    public void TestPacketPrepareSolopacketSuccess()
    {
        var packets = this.original.Prepare();
        Assert.AreEqual(this.original, packets[0]);
    }
}
