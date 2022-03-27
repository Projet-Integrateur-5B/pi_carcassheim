namespace Client;
using Assets;
public partial class Client
{
    public static int Main()
    {
        var sender = Connection();

        try
        {
            var value = Communication(sender, 1, "petit test");
        }
        catch (ReceivedInvalidPacketFormatException e)
        {
            // TODO : handle case where errors is catch
            Console.WriteLine(e);
        }

        Disconnection(sender);
        return 0;
    }
}
