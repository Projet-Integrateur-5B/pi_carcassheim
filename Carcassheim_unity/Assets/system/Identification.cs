using System;
using System.Threading;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class Identification : MonoBehaviour
{
    
    void Start()
    {
        
        string login = "pseudo";
        string mdp = "mdp18";

        // Etablissement de la communication réseau
        Socket? socket = null;
        var original = new Packet();

        var error_value = Client.Connection(ref socket, 19000);
        switch (error_value)
        {
            case Tools.Errors.None:
                break;
            case Tools.Errors.ConfigFile:
                // TODO : handle case : config file is bad or issue while extracting the data
                Debug.Log(string.Format("Errors.ConfigFile"));
                break;
            case Tools.Errors.Socket:
                // TODO : handle case : connection could not be established
                Debug.Log(string.Format("Errors.Socket"));
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
            case Tools.Errors.Permission:
                break;
            case Tools.Errors.Success:
                break;
            default:
                // TODO : handle case : default
                Debug.Log(string.Format("Errors.Unknown"));
                break;
        }

        string[] test = { login, mdp };
        error_value = socket.Communication(ref original, Tools.IdMessage.Login, test);
        /*Console.WriteLine("\n {0} \n", original.Data[12]);
        try
        {
            port = int.Parse(original.Data[12], new CultureInfo("en-us"));
        }
        catch (FormatException e)
        {
            Console.WriteLine(e.Message);
        }*/

        switch (error_value)
        {
            case Tools.Errors.None:
                break;
            case Tools.Errors.Format: // == Errors.Receive ?
                // TODO : handle case : wrong format
                Debug.Log(string.Format("Errors.Format"));
                break;
            case Tools.Errors.Socket:
                // TODO : handle case : connection error
                Debug.Log(string.Format("Errors.Socket"));
                break;
            case Tools.Errors.Data:
                // TODO : handle case : error while getting the packet ready
                Debug.Log(string.Format("Errors.Data"));
                break;
            case Tools.Errors.Receive:
                // TODO : handle case : error while receiving an answer
                Debug.Log(string.Format("Errors.Receive"));
                break;
            case Tools.Errors.Unknown:
                break;
            case Tools.Errors.ConfigFile:
                break;
            case Tools.Errors.ToBeDetermined:
                break;
            case Tools.Errors.Permission:
                break;
            case Tools.Errors.Success:
                break;
            default:
                // TODO : handle case : default
                Debug.Log(string.Format("Errors.Unknown"));
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

        error_value = Client.Disconnection(socket);
        switch (error_value)
        {
            case Tools.Errors.None:
                break;
            case Tools.Errors.Socket:
                // TODO : handle case : connection could not be closed
                Debug.Log(string.Format("Errors.Socket"));
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
            case Tools.Errors.Permission:
                break;
            case Tools.Errors.Success:
                break;
            default:
                // TODO : handle case : default
                Debug.Log(string.Format("Errors.Unknown"));
                break;
        }


    }


}