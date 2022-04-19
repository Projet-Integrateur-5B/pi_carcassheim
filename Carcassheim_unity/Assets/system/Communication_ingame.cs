using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class Communication_ingame : MonoBehaviour
{

    // Param�tres de la partie 
    private readonly int _id_partie;

    private Dictionary<int, int> _dico_joueur_score; // Contient les ID's de chaque joueur
    private int _id_moderateur; // Identifiant du joueur mod�rateur

    private bool _statut_partie; // false -> en cours de cr�ation, true -> lanc�e

    private int _mode; // 0 -> Classique | 1 -> Time-attack | 2 -> Score

    private int _nb_tuiles;
    private int _score_max;

    private int _nb_joueur_max;
    private bool _privee;
    private int _timer; // En secondes
    private int _timer_max_joueur; // En secondes
    private int _meeples; // Nombre de meeples par joueur


    private Tools.Errors ReceiveRoomSettings(Socket socket)
    {
        var packet = new Packet();

        string[] nothing = new string[0];
        var result = socket.Communication(ref packet, Tools.IdMessage.RoomSettingsGet, nothing);
        // === Premi�re com : demande les settings de la room ===

        string[] dataReceived = packet.Data;
        _nb_joueur_max = int.Parse(dataReceived[0]);

        switch (dataReceived[1])
        {
            case "true":
                _privee = true;
                break;
            case "false":
                _privee = false;
                break;
            default:
                break;
        }

        _mode = int.Parse(dataReceived[2]);
        _nb_tuiles = int.Parse(dataReceived[3]);
        _meeples = int.Parse(dataReceived[4]);
        _timer = int.Parse(dataReceived[5]);
        _timer_max_joueur = int.Parse(dataReceived[6]);

        return result;
    }

    private void CheckErrorSocketConnect(Tools.Errors error_value)
    {
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
    }

    private void CheckErrorSocketRS(Tools.Errors error_value)
    {
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
    }

    // Start is called before the first frame update
    void Start()
    {
        // ======================
        // ==== DANS LE MENU ====
        // ======================

        // Boucle d'�coute du serveur
        Socket? socket = null;

        // Etablissement de la communication r�seau avec le thread com (synchrone)

        var errorValue = Client.Connection(ref socket, 19000);
        CheckErrorSocketConnect(errorValue);


        errorValue = ReceiveRoomSettings(socket);

        CheckErrorSocketRS(errorValue);

        // A FAIRE - Mise � jour des param�tres de la partie


        int idClient = -1;


        Action waitingForStart = () =>
        {
            var pack = new Packet();
            string[] nothing = { };
            Tools.Errors error;
            do
            {
                error = socket.Communication(ref pack, Tools.IdMessage.RoomStart, nothing);
            }
            while ( error == Tools.Errors.Success || error == Tools.Errors.Permission || error == Tools.Errors.None);
            _statut_partie = true;
        };

        Task.Run(waitingForStart);

        while (!_statut_partie) // Boucle tant que la partie n'est pas lanc�e
        {

            // Attente d'une action si mod�rateur, puis envoie de l'action au serveur
            if (_id_moderateur == idClient)
            {

                // Attente d'une action du client

                /*
                string[] data = { };
                errorValue = socket.Communication(ref original, Tools.IdMessage.RoomSettingsSet, data);
                */
                string[] data = new string[]
                {
                    _nb_joueur_max.ToString(),
                    _privee.ToString(),
                    _mode.ToString(),
                    _nb_tuiles.ToString(),
                    _meeples.ToString(),
                    _timer.ToString(),
                    _timer_max_joueur.ToString(),
                    _score_max.ToString()
                };

                var reception = new Packet();
                errorValue = socket.Communication(ref reception, Tools.IdMessage.RoomSettingsSet, data);

                CheckErrorSocketRS(errorValue);
                // OU r�ception du serveur : quelqu'un a rejoint la game


            }

            // Lancement d'une �coute synchrone si non mod�rateur (dans une task)  + prendre en compte le cas o� le client part de la partie
            else
            {

            }

        }


        // D�connexion du socket
        Disconnect(socket);



        // Type de r�ceptions : mise � jour des param�tres de partie (changement par le g�rant, ou qqun rejoint)

        // Type d'envois :
        //          - mise � jour des param�tres de la partie (si le joueur est le g�rant, � v�rifier c�t� client ET serv)
        //                  -> si modification envoy�e de la part d'un non g�rant, on r�pond un WarningCheat avec le serveur



        // ======================
        // ==== DANS LA GAME ====
        // ======================

        // Boucle d'�coute du serveur

        // Type de r�ceptions :
        //          - mise � jour affichage (coup d'un autre joueur, m�me non valid�)
        //          - r�ception d'un WarningCheat
        //          - indication de la part du serveur que c'est au client de jouer
        //                  -> d�but de la phase d'interactions entres placements du joueur et le serveur
        //                  -> se base sur des fonctions d'attente personnalis�es, o� le script attend que le joueur place ses tuiles/pions


        // Description de la phase d'interaction  :
        //      - R�ception des 3 tuiles 
        //      - V�rification qu'une des 3 est posable, si ce n'est pas le cas on pr�vient le serveur et on demande d'autres tuiles
        //      - Affichage de la tuile ainsi choisie par le client (la premi�re � �tre posable)
        //          -> MaJ graphique (tuile choisie)
        //      - (Attente d'une action du joueur : pose d'une tuile)
        //      - Envoie la pose de tuile au serveur et observe sa r�ponse (si le coup est ill�gal par exemple)
        //          -> MaJ graphique (tuile pos�e)
        //      - (Attente d'une action du joueur : pose d'un pion)
        //      - Envoie la pose de pion au serveur et observe sa r�ponse (si le coup est ill�gal par exemple)
        //          -> MaJ graphique (pion pos�)
        //      - (Attente d'une action du joueur : validation de son tour)
        //      - Envoie la validation du tour au serveur et observe sa r�ponse (si le coup est ill�gal par exemple)
        //          -> MaJ graphique (tour termin�)



    }

    // Update is called once per frame
    void Update()
    {
    }
    /*
    private void WaitForStart(Socket socket)
    {
        var pack = new Packet();
        string[] nothing = { };
        var error = socket.Communication(ref pack, Tools.IdMessage.RoomStart, nothing);

        if (error == Tools.Errors.Success || error == Tools.Errors.Permission || error == Tools.Errors.None)
            _statut_partie = true;
    }*/

    private void Disconnect(Socket socket)
    {
        Tools.Errors errorValue = Client.Disconnection(socket);
        switch (errorValue)
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
