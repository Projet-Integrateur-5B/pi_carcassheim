using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class Recuperation_liste_room : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

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

        string[] test = { };
        error_value = socket.Communication(ref original, Tools.IdMessage.RoomList, test);


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


        // Passer les données dans original.data à l'affichage

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
