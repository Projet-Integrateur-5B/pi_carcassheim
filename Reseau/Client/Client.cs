namespace Client;

public partial class Client
{
    public static int Main()
    {
        var sender = Connect.Connection();

        // TODO : handle case where value == -1 (error has occured)
        var value = Communication(sender, 1, "petit test");
        value = Communication(sender, 5, "test deux");
        value = Communication(sender, 9, "test trois");
        value = Communication(sender, 2, "test trois");

        Disconect.Deconnection(sender);
        return 0;
    }
}
