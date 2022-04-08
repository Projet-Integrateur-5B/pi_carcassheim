namespace Client;

using System.Globalization;
using System.Net.Sockets;
using Assets;

public static partial class Client
{
    public static int StartConnection(int port)
    {
        Socket? socket = null;
        var original = new Packet();

        var error_value = Connection(ref socket, port);
        switch (error_value)
        {
            case Tools.Errors.None:
                break;
            case Tools.Errors.ConfigFile:
                // TODO : handle case : config file is bad or issue while extracting the data
                Console.WriteLine("Errors.ConfigFile");
                break;
            case Tools.Errors.Socket:
                // TODO : handle case : connection could not be established
                Console.WriteLine("Errors.Socket");
                break;
            case Tools.Errors.ToBeDetermined:
                break;
            case Tools.Errors.Unknown:
                break;
            case Tools.Errors.Format:
                break;
            case Tools.Errors.Receive:
                break;
            case Tools.Errors.Data:
                break;
            default:
                // TODO : handle case : default
                Console.WriteLine("Errors.Unknown");
                break;
        }

        string[] test = { "pseudo", "mdp18" };
        error_value = socket.Communication(ref original, Tools.IdMessage.RoomJoin, test);
        Console.WriteLine("\n {0} \n", original.Data[12]);
        try
        {
            port = int.Parse(original.Data[12], new CultureInfo("en-us"));
        }
        catch (FormatException e)
        {
            Console.WriteLine(e.Message);
        }

        switch (error_value)
        {
            case Tools.Errors.None:
                break;
            case Tools.Errors.Format: // == Errors.Receive ?
                // TODO : handle case : wrong format
                Console.WriteLine("Errors.Format");
                break;
            case Tools.Errors.Socket:
                // TODO : handle case : connection error
                Console.WriteLine("Errors.Socket");
                break;
            case Tools.Errors.Data:
                // TODO : handle case : error while getting the packet ready
                Console.WriteLine("Errors.Data");
                break;
            case Tools.Errors.Receive:
                // TODO : handle case : error while receiving an answer
                Console.WriteLine("Errors.Receive");
                break;
            case Tools.Errors.Unknown:
                break;
            case Tools.Errors.ConfigFile:
                break;
            case Tools.Errors.ToBeDetermined:
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
            case Tools.Errors.None:
                break;
            case Tools.Errors.Socket:
                // TODO : handle case : connection could not be closed
                Console.WriteLine("Errors.Socket");
                break;
            case Tools.Errors.Unknown:
                break;
            case Tools.Errors.Format:
                break;
            case Tools.Errors.ConfigFile:
                break;
            case Tools.Errors.Receive:
                break;
            case Tools.Errors.Data:
                break;
            case Tools.Errors.ToBeDetermined:
                break;
            default:
                // TODO : handle case : default
                Console.WriteLine("Errors.Unknown");
                break;
        }

        return port;
    }

    public static int Main()
    {
        var port = 10000;
        int newPort;
        newPort = StartConnection(port);
        if (newPort != port)
        {
            port = newPort;
            StartConnection(port);
        }

        return 0;
    }
}
