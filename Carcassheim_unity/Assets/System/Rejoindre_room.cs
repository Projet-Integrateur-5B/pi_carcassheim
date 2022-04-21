using System;
using System.Net.Sockets;
using UnityEngine;

public class Rejoindre_room : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Récupération de l'ID de la partie
        int id_partie;

        // Ouverture du communication avec le serveur (serveur principal/main)
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
        error_value = socket.Communication(ref original, Tools.IdMessage.RoomCreate, test);

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



        // Sauvegarde les informations pour communiquer avec le bon thread de com du serveur
        int portThreadCom = -1;

        if (original.Error == Tools.Errors.Success)
        {
            portThreadCom = Int32.Parse(original.Data[0]);
        }
        else 
        {
            // AFFICHAGE GRAPHIQUE -> fail de connexion (afficher aussi la raison de l'échec)
        }
        
        
        // Déconnection de ce socket là quoi qu'il arrive
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


        // Si la connexion est un succès, on lance "Communication_ingame" et on lui donne le nouveau port (celui du thread de com)
        if(original.Error == Tools.Errors.Success)
        {

        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
