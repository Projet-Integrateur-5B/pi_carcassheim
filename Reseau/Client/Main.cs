namespace Client;
using System.Net.Sockets;
using Assets;
public static partial class Client
{
    public static int Main()
    {
        Socket? socket = null;
        var original = new Packet();

        var error_value = Connection(ref socket);
        switch (error_value)
        {
            case Errors.None:
                break;
            case Errors.ConfigFile:
                // TODO : handle case : config file is bad or issue while extracting the data
                Console.WriteLine("Errors.ConfigFile");
                break;
            case Errors.Socket:
                // TODO : handle case : connection could not be established
                Console.WriteLine("Errors.Socket");
                break;
            case Errors.ToBeDetermined:
                break;
            default:
                // TODO : handle case : default
                Console.WriteLine("Errors.Unknown");
                break;
        }

        error_value = socket.Communication(ref original, IdMessage.Default, "petit test");
        switch (error_value)
        {
            case Errors.None:
                break;
            case Errors.Format: // == Errors.Receive ?
                // TODO : handle case : wrong format
                Console.WriteLine("Errors.Format");
                break;
            case Errors.Socket:
                // TODO : handle case : connection error
                Console.WriteLine("Errors.Socket");
                break;
            case Errors.Data:
                // TODO : handle case : error while getting the packet ready
                Console.WriteLine("Errors.Data");
                break;
            case Errors.Receive:
                // TODO : handle case : error while receiving an answer
                Console.WriteLine("Errors.Receive");
                break;
            default:
                // TODO : handle case : default
                Console.WriteLine("Errors.Unknown");
                break;
        }

        error_value = Disconnection(socket);
        switch (error_value)
        {
            case Errors.None:
                break;
            case Errors.Socket:
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
