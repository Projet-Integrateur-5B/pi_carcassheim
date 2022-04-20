using System;
using System.Threading;
using System.Collections.Generic;

namespace system
{j

    public class Thread_communication
    {
        // Attributs

        private int _id_thread_com;
        private int _numero_port;
        private int _nb_parties_gerees;

        private List<int> _id_parties_gerees;
        private List<Thread_serveur_jeu> _lst_serveur_jeu;

        private static int _compteur_id_thread_com;

        // Locks

        private readonly object _lock_nb_parties_gerees;
        private readonly object _lock_id_parties_gerees;

        // Rajouter un objet RESEAU pour les communications ?


        // Constructeur

        public Thread_communication(int num_port, int id)
        {
            _numero_port = num_port;
            _nb_parties_gerees = 0;
            _id_parties_gerees = new List<int>();
            _id_thread_com = id;
            _lock_nb_parties_gerees = new object();
            _lock_id_parties_gerees = new object();
        }

        // Getters et setters

        public int Get_nb_parties_gerees()
        {
            return _nb_parties_gerees;
        }

        public List<int> Get_id_parties_gerees()
        {
            return _id_parties_gerees;
        }

        public object Get_lock_nb_parties_gerees()
        {
            return _lock_nb_parties_gerees;
        }

        public object Get_lock_id_parties_gerees()
        {
            return _lock_id_parties_gerees;
        }

        public int Get_port()
        {
            return _numero_port;
        }

        public List<Thread_serveur_jeu> Get_list_server_thread()
        {
            return this._lst_serveur_jeu;
        }

        // Ajoute une nouvelle partie au thread de communication
        public int AddNewGame(int playerId)
        {

            int id_nouv_partie = -1;

            lock (this._lock_nb_parties_gerees)
            {
                if (_nb_parties_gerees < 5)
                {
                    // Dixaine : numéro de thread, unité : numéro de partie
                    id_nouv_partie = _id_thread_com * 10 + (_nb_parties_gerees+1) ;

                    lock (this._lock_id_parties_gerees)
                    {
                        _id_parties_gerees.Add(id_nouv_partie);
                    }

                    _nb_parties_gerees++;
                }
            }

            if (id_nouv_partie != -1) // Si la partie a pu être crée
            {
                Thread_serveur_jeu thread_serveur_jeu = new Thread_serveur_jeu(id_nouv_partie, playerId);
                Thread nouv_thread = new Thread(new ThreadStart(thread_serveur_jeu.Lancement_thread_serveur_jeu));

                _lst_serveur_jeu.Add(thread_serveur_jeu);

                nouv_thread.Start();

                return 0;

            }
            else // La partie n'a pas pu être créée
            {
                return -1;
            }


        }

        // Création d'un nouveau thread_serveur_jeu

        // Méthodes

        public void Lancement_thread_com()
        {

            Thread.Sleep(2000);

            Console.WriteLine(string.Format("[{0}] Je suis un thread !", _id_thread_com));
            Console.WriteLine(string.Format("[{0}] J'officie sur le port numéro {1} !", _id_thread_com, _numero_port));
            Console.WriteLine(string.Format("[{0}] Je gère actuellement {1} parties!", _id_thread_com, _nb_parties_gerees));
            foreach (int id_ite in _id_parties_gerees)
            {
                Console.WriteLine(string.Format("[{0}] Je gère la partie d'ID {1}", _id_thread_com, id_ite));
            }

            //Debug.Log(string.Format("Compteur d'id de strings : {0}", _compteur_id_thread_com));


            int debug = 1;

            if (debug != 1) // TEMPORAIRE - A retirer plus tard    // A REMPLACER - Par boucle de réception
            {
                // Types de messages qui peuvent être reçus:
                //      • Création de partie
                //      • Arrivée dans la partie
                //      • Pose de tuile
                //      • Pose de pion
                //      • ? Fin de tour ?

                // Types de message à envoyer spontanéement par le thread_com:
                //      • Fin de partie
                //      • Réaction à triche
                //      • Réaction à afk
                //      • Kick (afk ou triche)



                //Thread.Sleep(20000);

            }


        }
    }
}
