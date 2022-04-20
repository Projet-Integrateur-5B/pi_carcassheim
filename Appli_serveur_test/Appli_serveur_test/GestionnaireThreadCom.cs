using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        // Génère un nouveau thread de communication
        // Retourne la position du thread de com dans la liste _lst_obj_threads_com
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

        // Retourne le port du thread de com gérant la partie dont l'ID est passé en paramètre
        public int GetPortFromPartyID (int partyID)
        {
            int port = -1;

            // Parcours des threads de communication pour trouver celui qui gère la partie cherchée
            foreach (Thread_communication thread_com_iterateur in _instance._lst_obj_threads_com)
            {
                lock (thread_com_iterateur.Get_lock_id_parties_gerees())
                {
                    List<int> lst_id_parties_gerees = thread_com_iterateur.Get_id_parties_gerees();

                    if (lst_id_parties_gerees.Contains(partyID))
                    {

                        // Port du thread de com
                        port = thread_com_iterateur.Get_port();


                        break; // Sortie du foreach

                    }
                }    
            }

            return port;
        }

        // Créer une nouvelle room et retourne le port du thread de com s'en occupant
        // Retour : [port] tout s'est bien passé
        // Retour : -1 la room n'a pas pu être créée 
        public int CreateRoom(int idPlayer)
        {
            int portThreadCom = -1;

            if (_lst_threads_com.Count == 0 && _lst_obj_threads_com.Count == 0)
            { // Aucun thread de comm n'existe

                int positionThreadCom = Creation_thread_com();

                if (positionThreadCom != -1)
                { // Seulement si un nouveau thread de com a pu être créé

                    // Demande de création d'une nouvelle partie dans le bon thread de com
                    int retourAddNewGame = _instance._lst_obj_threads_com[positionThreadCom].AddNewGame(idPlayer);
                    if( retourAddNewGame != -1)
                    {
                        portThreadCom = _instance._lst_obj_threads_com[positionThreadCom].Get_port();
                    }
                }            
            }

            return portThreadCom;
        }
    }
}
