namespace Client;
using System.Net.Sockets;
using Assets;
public partial class Client
{
    public static int Main()
    {
        var return_value = 0;
        Socket? socket = null;
        var original = new Packet();

        return_value = Connection(ref socket);
        switch (return_value)
        {
            case (int)Errors.None:
                break;
            case (int)Errors.ConfigFile:
                // TODO : handle case : config file is bad or issue while extracting the data
                Console.WriteLine("Errors.ConfigFile");
                break;
            case (int)Errors.Socket:
                // TODO : handle case : connection could not be established
                Console.WriteLine("Errors.Socket");
                break;
            case (int)Errors.ToBeDetermined:
                break;
            default:
                // TODO : handle case : default
                Console.WriteLine("Errors.Unknown");
                break;
        }

        return_value = Communication(socket, ref original, (byte)IdMessage.Default, "petit test");
        switch (return_value)
        {
            case (int)Errors.None:
                break;
            case (int)Errors.Format: // == Errors.Receive ?
                // TODO : handle case : wrong format
                Console.WriteLine("Errors.Format");
                break;
            case (int)Errors.Socket:
                // TODO : handle case : connection error
                Console.WriteLine("Errors.Socket");
                break;
            case (int)Errors.Data:
                // TODO : handle case : error while getting the packet ready
                Console.WriteLine("Errors.Data");
                break;
            case (int)Errors.Receive:
                // TODO : handle case : error while receiving an answer
                Console.WriteLine("Errors.Receive");
                break;
            default:
                // TODO : handle case : default
                Console.WriteLine("Errors.Unknown");
                break;
        }

        return_value = Disconnection(socket);
        switch (return_value)
        {
            case (int)Errors.None:
                break;
            case (int)Errors.Socket:
                // TODO : handle case : connection could not be closed
                Console.WriteLine("Errors.Socket");
                break;
            default:
                // TODO : handle case : default
                Console.WriteLine("Errors.Unknown");
                break;
        }
        return 0;
    }
}
