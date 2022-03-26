namespace Client;

using System.Net.Sockets;

public class Disconect
{
    public static void Deconnection(Socket sender)
    {
        // Deconnect to a remote device.
        try
        {
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
            Console.WriteLine("Closing connection...");
        }
        catch (ArgumentNullException ane)
        {
            Console.WriteLine("ArgumentNullException : {0}", ane);
        }
        catch (SocketException se)
        {
            Console.WriteLine("SocketException : {0}", se);
        }
        catch (Exception e)
        {
            Console.WriteLine("Unexpected exception : {0}", e);
        }
    }
}
