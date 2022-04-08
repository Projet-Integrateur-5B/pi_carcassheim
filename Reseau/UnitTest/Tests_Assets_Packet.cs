namespace UnitTest;

using System.Collections.Generic;
using System.Text;
using Assets;
using NUnit.Framework;

public class TestsAssetsPacket
{
    private Tools.Errors errorValue;
    private Packet original = new();
    private string originalAsString = new("");

    [SetUp]
    public void Setup()
    {
        this.errorValue = Tools.Errors.None;
        this.original = new Packet(false, Tools.IdMessage.Default, Tools.Errors.None, true, 999,
            new[] { "test", "deux" });
        this.originalAsString =
            "{\"Type\":false,\"IdMessage\":0,\"Error\":0," +
            "\"Final\":true,\"IdPlayer\":999,\"Data\":[\"test\",\"deux\"]}";
    }

    [Test]
    // Checking if conversion from byte array to Packet is successful.
    public void TestPacketByteArrayToPacketSuccess()
    {
        var resultAsBytes = Encoding.ASCII.GetBytes(this.originalAsString);
        var result = resultAsBytes.ByteArrayToPacket(ref this.errorValue);

        if (this.errorValue != Tools.Errors.None)
        {
            Assert.Fail();
        }

        Assert.AreEqual(this.original.Type, result.Type);
        Assert.AreEqual(this.original.IdMessage, result.IdMessage);
        Assert.AreEqual(this.original.Error, result.Error);
        Assert.AreEqual(this.original.Final, result.Final);
        Assert.AreEqual(this.original.IdPlayer, result.IdPlayer);
        Assert.AreEqual(this.original.Data[0], result.Data[0]);
        Assert.AreEqual(this.original.Data[1], result.Data[1]);
    }

    [Test]
    // Checking if conversion from Packet to byte array is successful.
    public void TestPacketToByteArraySuccess()
    {
        var resultAsBytes = this.original.PacketToByteArray(ref this.errorValue);
        if (this.errorValue != Tools.Errors.None)
        {
            Assert.Fail();
        }

        var resultAsString = Encoding.ASCII.GetString(resultAsBytes);

        Assert.AreEqual(this.originalAsString, resultAsString);
    }

    [Test]
    // Checking if splitting a small (single) Packet is successful.
    public void TestPacketSplitSoloPacketSuccess()
    {
        var packets = this.original.Split(ref this.errorValue);
        if (this.errorValue != Tools.Errors.None)
        {
            Assert.Fail();
        }

        Assert.AreEqual(this.original, packets[0]);
    }

    [Test]
    // Checking if splitting a big (multi) Packet is successful.
    public void TestPacketSplitMultiPacketsSuccess()
    {
        var type = this.original.Type;
        var idMessage = this.original.IdMessage;
        var error = this.original.Error;
        var final = this.original.Final;
        var idPlayer = this.original.IdPlayer;

        var packet = new Packet(type, idMessage, error, final, idPlayer,
            new[]
            {
                "abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz"
            });
        var p0 = new Packet(type, idMessage, error, false, idPlayer,
            new[]
            {
                "abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmno"
            });
        var p1 = new Packet(type, idMessage, error, true, idPlayer,
            new[]
            {
                "",
                "pqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz"
            });

        var packets = packet.Split(ref this.errorValue);
        if (this.errorValue != Tools.Errors.None)
        {
            Assert.Fail();
        }

        Assert.AreEqual(p0.Data, packets[0].Data);
        Assert.AreEqual(p1.Data, packets[1].Data);
    }

    [Test]
    // Checking if the concatenation of list of multiple packets is successful.
    public void TestPacketConcatenateSoloPacketSuccess()
    {
        var packets = new List<Packet> { this.original };
        var packet = packets.Concatenate(ref this.errorValue);
        if (this.errorValue != Tools.Errors.None)
        {
            Assert.Fail();
        }

        Assert.AreEqual(this.original, packet);
    }

    [Test]
    // Checking if the concatenation of list of a single packet is successful.
    public void TestPacketConcatenateMultiPacketSuccess()
    {
        var type = this.original.Type;
        var idMessage = this.original.IdMessage;
        var error = this.original.Error;
        var final = this.original.Final;
        var idPlayer = this.original.IdPlayer;

        var original = new Packet(type, idMessage, error, final, idPlayer,
            new[]
            {
                "abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz"
            });
        var p0 = new Packet(type, idMessage, error, false, idPlayer,
            new[]
            {
                "abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz"
            });
        var p1 = new Packet(type, idMessage, error, true, idPlayer,
            new[]
            {
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz-abcdefghijklmnopqrstuvwxyz"
            });

        var packets = new List<Packet> { p0, p1 };
        var packet = packets.Concatenate(ref this.errorValue);
        if (this.errorValue != Tools.Errors.None)
        {
            Assert.Fail();
        }

        Assert.AreEqual(original.Data, packet.Data);
    }
}
