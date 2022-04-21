using System;
using System.Net.Sockets;
using UnityEngine;
using ClassLibrary;
using Newtonsoft.Json;

namespace system
{
    public class Communication_inGame : MonoBehaviour
    {

        // Param�tres de la partie 
        private readonly ulong _id_partie;

        private ulong _mon_id;

        private int _mode; // 0 -> Classique | 1 -> Time-attack | 2 -> Score

        private int _nb_tuiles;
        private int _score_max;

        private int _nb_joueur_max;
        private int _timer; // En secondes
        private int _timer_max_joueur; // En secondes
        private int _meeples; // Nombre de meeples par joueur

        private Socket? socket = null;

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
            // ==== DANS LA GAME ====
            // ======================

            ClientAsync.OnPacketReceived += OnPacketReceived;
            ClientAsync.Receive(socket);

            /*
            Action listening = () =>
            {
                ClientAsync.StartClient(socket);
            };
            Task.Run(listening);
            */



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
        public void Update()
        {
        }

        public void Disconnection(Socket socket)
        {
            ClientAsync.StopListening(socket);
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();

            //TODO reload l'ancien socket
        }

        public void OnPacketReceived(object sender, Packet packet)
        {

            if (packet.IdMessage == Tools.IdMessage.TuileDraw)
            {
                //ReceiveRoomSettings(packet);
            }
        }
    }
}