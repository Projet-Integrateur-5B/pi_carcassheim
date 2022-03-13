using System.Text;
using NUnit.Framework;
using Reseau;

namespace Reseau_UnitTest;

public class Tests_Packet
{
    private string originalAsJsonString;
    private Packet originalAsPacket;

    [SetUp]
    public void Setup()
    {
        originalAsPacket = new Packet(true, null, 0, 0, 0, true, 1, 1, "test");
        originalAsJsonString = "{\"Type\":true,\"IpAddress\":null,\"Port\":0,\"IdRoom\":0," +
                               "\"IdMessage\":0,\"Status\":true,\"Permission\":1,\"IdPlayer\":1,\"Data\":\"test\"}";
    }

    [Test]
    public void Test_Packet_Serialization_Success()
    {
        var originalAsBytes = originalAsPacket.Serialize();
        var resultAsJsonString = Encoding.Default.GetString(originalAsBytes);

        Assert.AreEqual(originalAsJsonString, resultAsJsonString);
    }

    [Test]
    public void Test_Packet_Deserialization_Success()
    {
        var originalAsBytes = Encoding.ASCII.GetBytes(originalAsJsonString);
        var result = Packet.Deserialize(originalAsBytes);

        Assert.AreEqual(originalAsPacket.Type, result.Type);
        Assert.AreEqual(originalAsPacket.IpAddress, result.IpAddress);
        Assert.AreEqual(originalAsPacket.Port, result.Port);
        Assert.AreEqual(originalAsPacket.IdRoom, result.IdRoom);
        Assert.AreEqual(originalAsPacket.IdMessage, result.IdMessage);
        Assert.AreEqual(originalAsPacket.Status, result.Status);
        Assert.AreEqual(originalAsPacket.Permission, result.Permission);
        Assert.AreEqual(originalAsPacket.IdPlayer, result.IdPlayer);
        Assert.AreEqual(originalAsPacket.Data, result.Data);
    }
}