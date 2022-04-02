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
            case Errors.Unknown:
                break;
            case Errors.Format:
                break;
            case Errors.Receive:
                break;
            case Errors.Data:
                break;
            default:
                // TODO : handle case : default
                Console.WriteLine("Errors.Unknown");
                break;
        }

        string[] test = { "pseudo", "mdp18" };
        error_value = socket.Communication(ref original, IdMessage.Default, test);
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
            case Errors.Unknown:
                break;
            case Errors.ConfigFile:
                break;
            case Errors.ToBeDetermined:
                break;
            default:
                // TODO : handle case : default
                Console.WriteLine("Errors.Unknown");
                break;
        }

        /*error_value = Communication(socket, ref original, IdMessage.Connection, test);
        error_value = Communication(socket, ref original, IdMessage.Signup, test);
        error_value = Communication(socket, ref original, IdMessage.Statistics, test);
        error_value = Communication(socket, ref original, IdMessage.RoomList, test);
        error_value = Communication(socket, ref original, IdMessage.RoomJoin, test);
        error_value = Communication(socket, ref original, IdMessage.RoomLeave, test);
        error_value = Communication(socket, ref original, IdMessage.RoomReady, test);
        error_value = Communication(socket, ref original, IdMessage.RoomSettings, test);
        error_value = Communication(socket, ref original, IdMessage.RoomStart, test);*/

        error_value = Disconnection(socket);
        switch (error_value)
        {
            case Errors.None:
                break;
            case Errors.Socket:
                // TODO : handle case : connection could not be closed
                Console.WriteLine("Errors.Socket");
                break;
            case Errors.Unknown:
                break;
            case Errors.Format:
                break;
            case Errors.ConfigFile:
                break;
            case Errors.Receive:
                break;
            case Errors.Data:
                break;
            case Errors.ToBeDetermined:
                break;
            default:
                // TODO : handle case : default
                Console.WriteLine("Errors.Unknown");
                break;
        }

        return 0;
    }
}
