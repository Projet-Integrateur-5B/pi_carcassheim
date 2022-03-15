using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

namespace system
{

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

        // Augmente le nombre de parties gérées de 1
        public void Add_partie_geree(int id_partie_ajoutee)
        {
            _id_parties_gerees.Add(id_partie_ajoutee);
            _nb_parties_gerees++;
        }

        // Création d'un nouveau thread_serveur_jeu

        // Méthodes

        public void Lancement_thread_com()
        {

            Thread.Sleep(2000);

            Debug.Log(string.Format("[{0}] Je suis un thread !", _id_thread_com));
            Debug.Log(string.Format("[{0}] J'officie sur le port numéro {1} !", _id_thread_com, _numero_port));
            Debug.Log(string.Format("[{0}] Je gère actuellement {1} parties!", _id_thread_com, _nb_parties_gerees));
            foreach (int id_ite in _id_parties_gerees)
            {
                Debug.Log(string.Format("[{0}] Je gère la partie d'ID {1}", _id_thread_com, id_ite));
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


                int typeMsg = 0; // (Dépend du RESEAU) Dépendra du type : création de partie, connexion à la partie, etc
                int portPartie = 0; // (Dépend du RESEAU) Port de la partie en question
                int id_joueur_client = 0; // (Dépend du RESEAU)

                if (typeMsg == 1)    // Création de partie
                {
                    int id_nouv_partie = -1;

                    lock (this.Get_lock_nb_parties_gerees())
                    {
                        if (_nb_parties_gerees < 5)
                        {
                            id_nouv_partie = 1; // TEMPORAIRE     //  BDD - Fonction de récupération d'ID libre

                            this.Add_partie_geree(id_nouv_partie);
                        }
                    }

                    if (id_nouv_partie != -1) // Si la partie a pu être crée
                    {
                        Thread_serveur_jeu thread_serveur_jeu = new Thread_serveur_jeu(id_nouv_partie, id_joueur_client);
                        Thread nouv_thread = new Thread(new ThreadStart(thread_serveur_jeu.Lancement_thread_serveur_jeu));

                        _lst_serveur_jeu.Add(thread_serveur_jeu);

                        nouv_thread.Start();

                        // A FAIRE - Rajouter ce joueur dans la partie
                    }
                    else
                    {
                        // RESEAU - Fonction qui indique au client que la partie n'a pas pu être créée 
                    }

                }


            }


        }
    }
}
