namespace UnitTest;

using System.Collections.Generic;
using System.Text;
using Assets;
using NUnit.Framework;

public class TestsAssetsPacket
{
    private Errors errorValue;
    private Packet original = new();
    private string originalAsString = new("");

    [SetUp]
    public void Setup()
    {
        this.errorValue = Errors.None;
        this.original = new Packet(false, 0, 1, false, 0, true, 999, new[] { "test", "deux" });
        this.originalAsString =
            "{\"Type\":false,\"IdRoom\":0,\"IdMessage\":1,\"Status\":false,\"Permission\":0," +
            "\"Final\":true,\"IdPlayer\":999,\"Data\":[\"test\",\"deux\"]}";
    }

    [Test]
    public void TestPacketByteArrayToPacketSuccess()
    {
        var resultAsBytes = Encoding.ASCII.GetBytes(this.originalAsString);
        var result = resultAsBytes.ByteArrayToPacket(ref this.errorValue);

        if (this.errorValue != Errors.None)
        {
            Assert.Fail();
        }

        Assert.AreEqual(this.original.Type, result.Type);
        Assert.AreEqual(this.original.IdRoom, result.IdRoom);
        Assert.AreEqual(this.original.IdMessage, result.IdMessage);
        Assert.AreEqual(this.original.Status, result.Status);
        Assert.AreEqual(this.original.Permission, result.Permission);
        Assert.AreEqual(this.original.Final, result.Final);
        Assert.AreEqual(this.original.IdPlayer, result.IdPlayer);
        Assert.AreEqual(this.original.Data[0], result.Data[0]);
        Assert.AreEqual(this.original.Data[1], result.Data[1]);
    }

    [Test]
    public void TestPacketToByteArraySuccess()
    {
        var resultAsBytes = this.original.PacketToByteArray(ref this.errorValue);
        if (this.errorValue != Errors.None)
        {
            Assert.Fail();
        }

        var resultAsString = Encoding.ASCII.GetString(resultAsBytes);

        Assert.AreEqual(this.originalAsString, resultAsString);
    }

    [Test]
    public void TestPacketSplitSolopacketSuccess()
    {
        var packets = this.original.Split(ref this.errorValue);
        if (this.errorValue != Errors.None)
        {
            Assert.Fail();
        }

        Assert.AreEqual(this.original, packets[0]);
    }

    [Test]
    public void TestPacketSplitMultipacketsSuccess()
    {
        var type = this.original.Type;
        var idRoom = this.original.IdRoom;
        var idMessage = this.original.IdMessage;
        var status = this.original.Status;
        var permission = this.original.Permission;
        var final = this.original.Final;
        var idPlayer = this.original.IdPlayer;

        var packet = new Packet(type, idRoom, idMessage, status, permission, final, idPlayer,
            new[]
            {
                "abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz"
            });
        var p0 = new Packet(type, idRoom, idMessage, status, permission, false, idPlayer,
            new[]
            {
                "abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz"
            });
        var p1 = new Packet(type, idRoom, idMessage, status, permission, true, idPlayer,
            new[]
            {
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz"
            });

        var packets = packet.Split(ref this.errorValue);
        if (this.errorValue != Errors.None)
        {
            Assert.Fail();
        }

        Assert.AreEqual(p0.Data, packets[0].Data);
        Assert.AreEqual(p1.Data, packets[1].Data);
    }

    [Test]
    public void TestPacketCatenateSolopacketSuccess()
    {
        var packets = new List<Packet> { this.original };
        var packet = packets.Catenate(ref this.errorValue);
        if (this.errorValue != Errors.None)
        {
            Assert.Fail();
        }
        Assert.AreEqual(this.original, packet);
    }

    [Test]
    public void TestPacketCatenateMultipacketSuccess()
    {
        var type = this.original.Type;
        var idRoom = this.original.IdRoom;
        var idMessage = this.original.IdMessage;
        var status = this.original.Status;
        var permission = this.original.Permission;
        var final = this.original.Final;
        var idPlayer = this.original.IdPlayer;

        var original = new Packet(type, idRoom, idMessage, status, permission, final, idPlayer,
            new[]
            {
                "abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz"
            });
        var p0 = new Packet(type, idRoom, idMessage, status, permission, false, idPlayer,
            new[]
            {
                "abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz"
            });
        var p1 = new Packet(type, idRoom, idMessage, status, permission, true, idPlayer,
            new[]
            {
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz"
            });

        var packets = new List<Packet> { p0, p1 };
        var packet = packets.Catenate(ref this.errorValue);
        if (this.errorValue != Errors.None)
        {
            Assert.Fail();
        }
        Assert.AreEqual(original.Data, packet.Data);
    }
}
