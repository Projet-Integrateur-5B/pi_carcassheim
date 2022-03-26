namespace UnitTest;

using System.Text;
using Assets;
using NUnit.Framework;

public class TestsAssetsPacket
{
    private Packet original = new();
    private string originalAsString = new("");

    [SetUp]
    public void Setup()
    {
        this.original = new Packet(false, 0, 1, false, 0, true, 999, "test");
        this.originalAsString = "{\"Type\":false,\"IdRoom\":0,\"IdMessage\":1,\"Status\":false,\"Permission\":0," +
                                "\"Final\":true,\"IdPlayer\":999,\"Data\":\"test\"}";
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
        Assert.AreEqual(this.original.Final, result.Final);
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
    public void TestPacketPrepareSolopacketSuccess()
    {
        var packets = this.original.Prepare();
        Assert.AreEqual(this.original, packets[0]);
    }

    [Test]
    public void TestPacketPrepareMultipacketsSuccess()
    {
        var type = this.original.Type;
        var idRoom = this.original.IdRoom;
        var idMessage = this.original.IdMessage;
        var status = this.original.Status;
        var permission = this.original.Permission;
        var final = this.original.Final;
        var idPlayer = this.original.IdPlayer;

        var packet = new Packet(type, idRoom, idMessage, status, permission, final, idPlayer,
            "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz");
        var p0 = new Packet(type, idRoom, idMessage, status, permission, false, idPlayer, "")
        {
            Data =
            "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz"
        };
        var p1 = new Packet(type, idRoom, idMessage, status, permission, true, idPlayer,
            "-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz");

        var packets = packet.Prepare();
        Assert.AreEqual(p0.Data, packets[0].Data);
        Assert.AreEqual(p1.Data, packets[1].Data);
    }
}
