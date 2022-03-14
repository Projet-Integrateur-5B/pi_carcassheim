using System.Net;
using System.Text;
using NUnit.Framework;
using Reseau;

namespace Reseau_UnitTest;

public class Tests_Packet
{
    private static string _localhost = "127.0.0.1";
    private Packet _originalAsPacket = new Packet();
    private string _originalAsJsonString = new string("");

    [SetUp]
    public void Setup()
    {
        _originalAsPacket = new Packet(true, IPAddress.Parse(_localhost), 0, 0, 0, true, 1, 1, "test");
        _originalAsJsonString = "{\"Type\":true,\"IpAddress\":null,\"Port\":0,\"IdRoom\":0," +
                                "\"IdMessage\":0,\"Status\":true,\"Permission\":1,\"IdPlayer\":1,\"Data\":\"test\"}";
    }

    [Test]
    public void Test_Packet_Serialization_Success()
    {
        var originalAsBytes = _originalAsPacket.Serialize();
        var resultAsJsonString = Encoding.Default.GetString(originalAsBytes);

        Assert.AreEqual(_originalAsJsonString, resultAsJsonString);
    }

    [Test]
    public void Test_Packet_Deserialization_Success()
    {
        var originalAsBytes = Encoding.ASCII.GetBytes(_originalAsJsonString);
        var result = Packet.Deserialize(originalAsBytes);

        Assert.AreEqual(_originalAsPacket.Type, result.Type);
        Assert.AreEqual(_originalAsPacket.IpAddress, result.IpAddress);
        Assert.AreEqual(_originalAsPacket.Port, result.Port);
        Assert.AreEqual(_originalAsPacket.IdRoom, result.IdRoom);
        Assert.AreEqual(_originalAsPacket.IdMessage, result.IdMessage);
        Assert.AreEqual(_originalAsPacket.Status, result.Status);
        Assert.AreEqual(_originalAsPacket.Permission, result.Permission);
        Assert.AreEqual(_originalAsPacket.IdPlayer, result.IdPlayer);
        Assert.AreEqual(_originalAsPacket.Data, result.Data);
    }
}