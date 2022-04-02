namespace Assets;

public class ReceivedInvalidPacketFormatException : Exception
{
    public ReceivedInvalidPacketFormatException() { }

    public ReceivedInvalidPacketFormatException(string message) : base(message)
    {

    }
}
