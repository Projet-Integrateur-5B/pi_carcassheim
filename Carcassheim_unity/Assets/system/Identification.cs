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
        var server_answer = new Packet();

        var error_value = Client.Connection(ref socket);
        switch (error_value)
        {
            case Errors.None:
                break;
            case Errors.ConfigFile:
                // TODO : handle case : config file is bad or issue while extracting the data
                Debug.Log("Errors.ConfigFile");
                break;
            case Errors.Socket:
                // TODO : handle case : connection could not be established
                Debug.Log("Errors.Socket");
                break;
            case Errors.ToBeDetermined:
                break;
            default:
                // TODO : handle case : default
                Debug.Log("Errors.Unknown");
                break;
        }

        // Tentative d'identification 

        string[] data_to_send = { login, mdp };
        error_value = socket.Communication(ref server_answer, IdMessage.Connection, data_to_send);
        switch (error_value)
        {
            case Errors.None:
                break;
            case Errors.Format: // == Errors.Receive ?
                // TODO : handle case : wrong format
                Debug.Log("Errors.Format");
                break;
            case Errors.Socket:
                // TODO : handle case : connection error
                Debug.Log("Errors.Socket");
                break;
            case Errors.Data:
                // TODO : handle case : error while getting the packet ready
                Debug.Log("Errors.Data");
                break;
            case Errors.Receive:
                // TODO : handle case : error while receiving an answer
                Debug.Log("Errors.Receive");
                break;
            default:
                // TODO : handle case : default
                Debug.Log("Errors.Unknown");
                break;
        }

        if (server_answer.Status == true) // Identification réussie
        {
            Debug.Log("Réussite de l'identification !");
        }
        else // Identification échouée
        {
            if(server_answer.Data[0] == "login")
            {
                Debug.Log("Echec de l'identification: login inconnu");
            }
            else if(server_answer.Data[0] == "password")
            {
                Debug.Log("Echec de l'identification: password erroné");
            }



        }

        error_value = Client.Disconnection(socket);
        switch (error_value)
        {
            case Errors.None:
                break;
            case Errors.Socket:
                // TODO : handle case : connection could not be closed
                Debug.Log("Errors.Socket");
                break;
            default:
                // TODO : handle case : default
                Debug.Log("Errors.Unknown");
                break;
        }


    }


}