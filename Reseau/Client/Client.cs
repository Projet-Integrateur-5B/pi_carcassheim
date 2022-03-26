namespace Client;

public partial class Client
{
    public static int Main()
    {
        var sender = Connect.Connection();
        Communication(sender, 1, "petit test");
        Communication(sender, 5, "test deux");
        Communication(sender, 9, "test trois");
        Communication(sender, 2, "test trois");
        Disconect.Deconnection(sender);
        return 0;
    }
}
