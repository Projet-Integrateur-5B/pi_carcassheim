using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class Communication_ingame : MonoBehaviour
{

    // Paramètres de la partie 
    private readonly int _id_partie;

    private Dictionary<int, int> _dico_joueur_score; // Contient les ID's de chaque joueur
    private int _id_moderateur; // Identifiant du joueur modérateur

    private bool _statut_partie; // false -> en cours de création, true -> lancée

    private int _mode; // 0 -> Classique | 1 -> Time-attack | 2 -> Score

    private int _nb_tuiles;
    private int _score_max;

    private bool _privee;
    private int _timer; // En secondes
    private int _timer_max_joueur; // En secondes
    private int _meeples; // Nombre de meeples par joueur


    // Start is called before the first frame update
    void Start()
    {
        // ======================
        // ==== DANS LE MENU ====
        // ======================

        // Boucle d'écoute du serveur

        // === Première com : demande les settings de la room ===

        // Etablissement de la communication réseau avec le thread com (synchrone)
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

        string[] data = { };
        error_value = socket.Communication(ref original, Tools.IdMessage.RoomSettings, data);

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

        


        // A FAIRE - Mise à jour des paramètres de la partie

        int idClient = -1;


        while (!_statut_partie) // Boucle tant que la partie n'est pas lancée
        {

            // Attente d'une action si modérateur, puis envoie de l'action au serveur
            if (_id_moderateur == idClient)
            {

                // Attente d'une action du client


                /*
                string[] data = { };
                error_value = socket.Communication(ref original, Tools.IdMessage.SetRoomSettings, data);
                */
            }

            // Lancement d'une écoute synchrone si non modérateur
            else
            {

            }

        }


        // Déconnexion du socket
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



        // Type de réceptions : mise à jour des paramètres de partie (changement par le gérant, ou qqun rejoint)

        // Type d'envois :
        //          - mise à jour des paramètres de la partie (si le joueur est le gérant, à vérifier côté client ET serv)
        //                  -> si modification envoyée de la part d'un non gérant, on répond un WarningCheat avec le serveur



        // ======================
        // ==== DANS LA GAME ====
        // ======================

        // Boucle d'écoute du serveur

        // Type de réceptions :
        //          - mise à jour affichage (coup d'un autre joueur, même non validé)
        //          - réception d'un WarningCheat
        //          - indication de la part du serveur que c'est au client de jouer
        //                  -> début de la phase d'interactions entres placements du joueur et le serveur
        //                  -> se base sur des fonctions d'attente personnalisées, où le script attend que le joueur place ses tuiles/pions


        // Description de la phase d'interaction  :
        //      - Réception des 3 tuiles 
        //      - Vérification qu'une des 3 est posable, si ce n'est pas le cas on prévient le serveur et on demande d'autres tuiles
        //      - Affichage de la tuile ainsi choisie par le client (la première à être posable)
        //          -> MaJ graphique (tuile choisie)
        //      - (Attente d'une action du joueur : pose d'une tuile)
        //      - Envoie la pose de tuile au serveur et observe sa réponse (si le coup est illégal par exemple)
        //          -> MaJ graphique (tuile posée)
        //      - (Attente d'une action du joueur : pose d'un pion)
        //      - Envoie la pose de pion au serveur et observe sa réponse (si le coup est illégal par exemple)
        //          -> MaJ graphique (pion posé)
        //      - (Attente d'une action du joueur : validation de son tour)
        //      - Envoie la validation du tour au serveur et observe sa réponse (si le coup est illégal par exemple)
        //          -> MaJ graphique (tour terminé)



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
