namespace UnitTest;
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
        this.original = new Packet(Tools.IdMessage.Default, Tools.Errors.None, 999, 0,
            new[] { "test", "deux" });
        this.originalAsString =
            "{\"IdMessage\":0,\"Error\":0,\"IdPlayer\":999,\"IdRoom\":0,\"Data\":[\"test\",\"deux\"]}";
    }

    [Test]
    // Checking if conversion from byte array to Packet is successful.
    public void TestPacketByteArrayToPacketSuccess()
    {
        var resultAsBytes = Encoding.ASCII.GetBytes(this.originalAsString);
        var result = resultAsBytes.ByteArrayToPacket(ref this.errorValue);

        if (this.errorValue != Tools.Errors.None || result is null || result.Count == 0)
        {
            Assert.Fail();
            return;
        }

        Assert.AreEqual(this.original.IdMessage, result[0].IdMessage);
        Assert.AreEqual(this.original.Error, result[0].Error);
        Assert.AreEqual(this.original.IdPlayer, result[0].IdPlayer);
        Assert.AreEqual(this.original.IdRoom, result[0].IdRoom);
        Assert.AreEqual(this.original.Data[0], result[0].Data[0]);
        Assert.AreEqual(this.original.Data[1], result[0].Data[1]);
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
}
