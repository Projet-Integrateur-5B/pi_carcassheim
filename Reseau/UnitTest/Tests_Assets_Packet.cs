namespace UnitTest;

using System.Text;
using Assets;
using NUnit.Framework;

public class TestsAssetsPacket
{
    private const string Localhost = "127.0.0.1";
    private Packet original = new();
    private string originalAsString = new("");

    [SetUp]
    public void Setup()
    {
        this.original = new Packet(true, Localhost, 0, 0, 0, true, 1, 1, "test");
        this.originalAsString = "{\"Type\":true,\"IpAddress\":\"" + Localhost + "\",\"Port\":0," +
                                "\"IdRoom\":0,\"IdMessage\":0,\"Status\":true,\"Permission\":1," +
                                "\"IdPlayer\":1,\"Data\":\"test\"}";
    }

    [Test]
    public void TestPacketSerializationSuccess()
    {
        var originalAsBytes = this.original.Serialize();
        var resultAsString = Encoding.Default.GetString(originalAsBytes);

        Assert.AreEqual(this.originalAsString, resultAsString);
    }

    [Test]
    public void TestPacketDeserializationSuccess()
    {
        var originalAsBytes = Encoding.ASCII.GetBytes(this.originalAsString);
        var result = Packet.Deserialize(originalAsBytes);

        if (result is null)
        {
            Assert.Fail();
        }
        else
        {
            Assert.AreEqual(this.original.Type, result.Type);
            Assert.AreEqual(this.original.IpAddress, result.IpAddress);
            Assert.AreEqual(this.original.Port, result.Port);
            Assert.AreEqual(this.original.IdRoom, result.IdRoom);
            Assert.AreEqual(this.original.IdMessage, result.IdMessage);
            Assert.AreEqual(this.original.Status, result.Status);
            Assert.AreEqual(this.original.Permission, result.Permission);
            Assert.AreEqual(this.original.IdPlayer, result.IdPlayer);
            Assert.AreEqual(this.original.Data, result.Data);
        }
    }
}
