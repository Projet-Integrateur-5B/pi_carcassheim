namespace Client;
using System.Net.Sockets;
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
            // TODO : handle case : wrong format
            Console.WriteLine("ReceivedInvalidPacketFormatException : {0}", e);
        }
        catch (ArgumentNullException ane)
        {
            Console.WriteLine("ArgumentNullException : {0}", ane);
        }
        catch (SocketException se)
        {
            // TODO : handle case : connection is already closed on the server side
            Console.WriteLine("SocketException : {0}", se);
        }
        catch (Exception e) when (e is not ReceivedInvalidPacketFormatException)
        {
            Console.WriteLine("Unexpected exception : {0}", e);
        }

        Disconnection(sender);
        return 0;
    }
}
