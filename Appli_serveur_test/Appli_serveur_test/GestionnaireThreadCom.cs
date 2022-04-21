using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary;

namespace system
{
    internal class GestionnaireThreadCom
    {

        public List<Thread_communication> _lst_obj_threads_com { get; set; }
        public List<Thread> _lst_threads_com { get; set; }
        private List<int> _lst_port_dispo;
        private int _compteur_id_thread_com;

        private GestionnaireThreadCom() { }

        private static GestionnaireThreadCom _instance;

        // Lock pour l'accès en multithreaded
        private static readonly object _lock = new object();

        // ==========================
        // Récupération du singleton
        // ==========================


        /// <summary>
        /// Get an instance of the singleton
        /// </summary>
        /// <returns> An instance of the singleton </returns>
        public static GestionnaireThreadCom GetInstance()
        {

            if (_instance == null)
            {

                lock (_lock)
                {

                    if (_instance == null)
                    {
                        _instance = new GestionnaireThreadCom();
                        _instance._lst_obj_threads_com = new List<Thread_communication>();
                        _instance._lst_threads_com = new List<Thread>();
                        int[] portsDispos = { 19001, 19002, 19003, 19004, 19005, 19006, 19007 };
                        _instance._lst_port_dispo = new List<int>(portsDispos);
                        _instance._compteur_id_thread_com = 0;
                    }
                }
            }
            return _instance;
        }

        // ==========================================
        // Méthodes privées, pour utilisation interne
        // =========================================

        /// <summary>
        /// Generate a new communication thread manager
        /// </summary>
        /// <returns> The position of the thread manager in the list _lst_obj_threads_com </returns>
        private static int Creation_thread_com()
        {

            lock (_lock)
            {
                if (_instance._lst_port_dispo.Count != 0)
                {
                    int port_choisi = _instance._lst_port_dispo[0];

                    Thread_communication thread_com = new Thread_communication(port_choisi, _instance._compteur_id_thread_com);
                    _instance._compteur_id_thread_com++;
                    Thread nouv_thread = new Thread(new ThreadStart(thread_com.Lancement_thread_com));

                    _instance._lst_obj_threads_com.Add(thread_com);
                    _instance._lst_threads_com.Add(nouv_thread);

                    _instance._lst_port_dispo.RemoveAt(0);

                    nouv_thread.Start();

                    return _instance._lst_obj_threads_com.Count()-1;
                }
                else
                {
                    // Erreur : plus de ports disponibles pour un nouveau thread de communication
                    return -1;
                }
            }

            

        }

        // ====================
        // Méthodes publiques
        // ====================

        /// <summary>
        /// List all the rooms that exists
        /// </summary>
        /// <returns> A list of all rooms : first value is ID, then number of player and number of player max </returns>
        public string[] GetRoomList()
        {
            List<string> room_list = new List<string>();

            // Parcours des threads de communication pour lister ses rooms
            foreach (Thread_communication thread_com_iterateur in _instance._lst_obj_threads_com)
            {
                foreach (Thread_serveur_jeu thread_serv_ite in thread_com_iterateur.Get_list_server_thread())
                {
                    room_list.Add(thread_serv_ite.Get_ID().ToString());
                    room_list.Add(thread_serv_ite.NbJoueurs.ToString());
                    room_list.Add(thread_serv_ite.NbJoueursMax.ToString());
                }
            }

            return room_list.ToArray();
        }

        /// <summary>
        /// Get the port with wich communicate for a given room
        /// </summary>
        /// <param name="roomID"> ID of the room </param>
        /// <returns> The port of the communication thread that manage the room </returns>
        public int GetPortFromPartyID (int roomID)
        {
            int port = -1;

            // Parcours des threads de communication pour trouver celui qui gère la partie cherchée
            foreach (Thread_communication thread_com_iterateur in _instance._lst_obj_threads_com)
            {
                lock (thread_com_iterateur.Get_lock_id_parties_gerees())
                {
                    List<int> lst_id_parties_gerees = thread_com_iterateur.Get_id_parties_gerees();

                    if (lst_id_parties_gerees.Contains(roomID))
                    {

                        // Port du thread de com
                        port = thread_com_iterateur.Get_port();


                        break; // Sortie du foreach

                    }
                }    
            }

            return port;
        }


        /// <summary>
        /// Create a new room and return the port of the manager thread
        /// </summary>
        /// <param name="idPlayer"> Id of the player making this request </param>
        /// <returns> Returns a list containing the id then the port if all goes well, -1 otherwise </returns>
        public List<int> CreateRoom(ulong idPlayer)
        {
            int portThreadCom = -1;
            int idNewRoom = -1;

            if (_lst_threads_com.Count == 0 && _lst_obj_threads_com.Count == 0)
            { // Aucun thread de comm n'existe

                int positionThreadCom = Creation_thread_com();

                if (positionThreadCom != -1)
                { // Seulement si un nouveau thread de com a pu être créé

                    // Demande de création d'une nouvelle partie dans le bon thread de com
                    idNewRoom = _instance._lst_obj_threads_com[positionThreadCom].AddNewGame(idPlayer);
                    if(idNewRoom != -1)
                    {
                        portThreadCom = _instance._lst_obj_threads_com[positionThreadCom].Get_port();
                    }
                }            
            }
            else
            {
                bool thread_com_libre_trouve = false;

                // Parcours des différents threads de communication pour trouver un qui gère < 5 parties
                foreach (Thread_communication thread_com_iterateur in _lst_obj_threads_com)
                {
                    lock (thread_com_iterateur.Get_lock_nb_parties_gerees())
                    {
                        if (thread_com_iterateur.Get_nb_parties_gerees() < 5)
                        {
                            thread_com_libre_trouve = true;
                        }
                    }

                    if (thread_com_libre_trouve)
                    {
                        idNewRoom = thread_com_iterateur.AddNewGame(idPlayer);
                        if (idNewRoom != -1)
                        {
                            portThreadCom = thread_com_iterateur.Get_port();
                        }

                        break; // Sort du foreach
                    }

                }

                // Si aucun des threads n'est libre pour héberger une partie de plus
                if (thread_com_libre_trouve == false)
                {

                    int positionThreadCom = Creation_thread_com();

                    if (positionThreadCom != -1)
                    { // Seulement si un nouveau thread de com a pu être créé

                        // Demande de création d'une nouvelle partie dans le bon thread de com
                        idNewRoom = _instance._lst_obj_threads_com[positionThreadCom].AddNewGame(idPlayer);
                        if (idNewRoom != -1)
                        {
                            portThreadCom = _instance._lst_obj_threads_com[positionThreadCom].Get_port();
                        }
                    }

                }

            }

            List<int> listReturn = new List<int>{ idNewRoom, portThreadCom };

            return listReturn;
        }

        
        
        public void UpdateRoom(string idRoom, ulong idPlayer, string[] settings)
        {
            // Parcours des threads de communication pour trouver celui qui gère la partie cherchée
            foreach (Thread_communication thread_com_iterateur in _instance._lst_obj_threads_com)
            {
                foreach (Thread_serveur_jeu thread_serv_ite in thread_com_iterateur.Get_list_server_thread())
                {
                    if (idRoom != thread_serv_ite.Get_ID().ToString()) continue;
                    thread_serv_ite.Set_Settings(idPlayer, settings);
                    return;
                }
            }
        }
        
        public string[] SettingsRoom(string idRoom)
        {
            // Parcours des threads de communication pour trouver celui qui gère la partie cherchée
            foreach (Thread_communication thread_com_iterateur in _instance._lst_obj_threads_com)
            {
                foreach (Thread_serveur_jeu thread_serv_ite in thread_com_iterateur.Get_list_server_thread())
                {
                    if (idRoom == thread_serv_ite.Get_ID().ToString())
                        return thread_serv_ite.Get_Settings();
                }
            }

            return Array.Empty<string>();
        }

        public int JoinPlayer(string idRoom, ulong idPlayer)
        {
            // Parcours des threads de communication pour trouver celui qui gère la partie cherchée
            foreach (Thread_communication thread_com_iterateur in _instance._lst_obj_threads_com)
            {
                foreach (Thread_serveur_jeu thread_serv_ite in thread_com_iterateur.Get_list_server_thread())
                {
                    if (idRoom != thread_serv_ite.Get_ID().ToString()) continue;
                    var playerStatus = thread_serv_ite.AddJoueur(idPlayer);
                    if (playerStatus != Tools.PlayerStatus.Success)
                        return -1;
                    return thread_com_iterateur.Get_port();
                }
            }

            return -1;
        }

        public Tools.PlayerStatus RemovePlayer(string idRoom, ulong idPlayer)
        {
            // Parcours des threads de communication pour trouver celui qui gère la partie cherchée
            foreach (Thread_communication thread_com_iterateur in _instance._lst_obj_threads_com)
            {
                foreach (Thread_serveur_jeu thread_serv_ite in thread_com_iterateur.Get_list_server_thread())
                {
                    if (idRoom != thread_serv_ite.Get_ID().ToString()) continue;
                    return thread_serv_ite.RemoveJoueur(idPlayer);
                }
            }

            return Tools.PlayerStatus.NotFound;
        }
        
        public Tools.PlayerStatus KickPlayer(string idRoom, ulong idModo, ulong idPlayer)
        {
            // Parcours des threads de communication pour trouver celui qui gère la partie cherchée
            foreach (Thread_communication thread_com_iterateur in _instance._lst_obj_threads_com)
            {
                foreach (Thread_serveur_jeu thread_serv_ite in thread_com_iterateur.Get_list_server_thread())
                {
                    if (idRoom != thread_serv_ite.Get_ID().ToString()) continue;
                    if (idModo != thread_serv_ite.Get_Moderateur())
                        return Tools.PlayerStatus.Permissions;
                    return thread_serv_ite.RemoveJoueur(idPlayer);
                }
            }

            return Tools.PlayerStatus.NotFound;
        }
        
        public Tools.PlayerStatus ReadyPlayer(string idRoom, ulong idPlayer)
        {
            // Parcours des threads de communication pour trouver celui qui gère la partie cherchée
            foreach (Thread_communication thread_com_iterateur in _instance._lst_obj_threads_com)
            {
                foreach (Thread_serveur_jeu thread_serv_ite in thread_com_iterateur.Get_list_server_thread())
                {
                    if (idRoom != thread_serv_ite.Get_ID().ToString()) continue;
                    return thread_serv_ite.SetPlayerStatus(idPlayer);
                }
            }

            return Tools.PlayerStatus.NotFound;
        }
        
        public Tools.PlayerStatus CheatPlayer(string idRoom, ulong idPlayer)
        {
            // Parcours des threads de communication pour trouver celui qui gère la partie cherchée
            foreach (Thread_communication thread_com_iterateur in _instance._lst_obj_threads_com)
            {
                foreach (Thread_serveur_jeu thread_serv_ite in thread_com_iterateur.Get_list_server_thread())
                {
                    if (idRoom != thread_serv_ite.Get_ID().ToString()) continue;
                    return thread_serv_ite.SetPlayerTriche(idPlayer);
                }
            }

            return Tools.PlayerStatus.NotFound;
        }
    }
}
