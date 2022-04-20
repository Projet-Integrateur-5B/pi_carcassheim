using System;
using System.Collections.Generic;
using ClassLibrary;

namespace system
{

    public partial class Thread_serveur_jeu
    {

        // Attributs

        // Champs qui paramètrent la partie

        private readonly int _id_partie;

        /* Attributs en lien avec le Dictionary Player */
        private Dictionary<long, Player> _dico_joueur; // Contient les ID's de chaque joueur
        private Semaphore _s_dico_joueur;
        private uint _nombre_joueur;
        private uint _nombre_joueur_max;
        private long _id_moderateur; // Identifiant du joueur modérateur

        private string _statut_partie;

        private Tools.Mode _mode; // 0 -> Classique | 1 -> Time-attack | 2 -> Score

        private int _nb_tuiles;
        private int _score_max;

        private bool _privee;
        private Tools.Timer _timer; // En secondes
        private Tools.Timer _timer_max_joueur; // En secondes
        private Tools.Meeple _meeples; // Nombre de meeples par joueur


        // Champs nécessaires pour le bon fonctionnement du programme

        // Getters et setters

        public int Get_ID()
        {
            return this._id_partie;
        }

        public uint NbJoueurs
        {
            get { return this._nombre_joueur; }
        }

        public uint NbJoueursMax
        {
            get { return this._nombre_joueur_max;  }
        }

        // Constructeur
        public Thread_serveur_jeu(int id_partie, int id_joueur_createur)
        {
            _id_partie = id_partie;

            /* Zone  du Dictionary Score */
            _dico_joueur = new Dictionary<long, Player>();
            _s_dico_joueur = new Semaphore(1, 1);
            _nombre_joueur = 1;
            _nombre_joueur_max = 8;
            _dico_joueur.Add(id_joueur_createur, new Player(id_joueur_createur));
            _id_moderateur = id_joueur_createur;

            _statut_partie = "ACCUEIL";

            // Initialisation des valeurs par défaut
            _mode = Tools.Mode.Default;
            _nb_tuiles = 60;
            _score_max = -1;
            _privee = true; // Une partie est par défaut privée
            _timer = Tools.Timer.Heure; // Une heure par défaut
            _timer_max_joueur = Tools.Timer.Minute;
            _meeples = Tools.Meeple.Huit;
        }

        // Méthodes

        public Tools.PlayerStatus AddJoueur(long id_joueur)
        {
            _s_dico_joueur.WaitOne();
            if (_nombre_joueur >= _nombre_joueur_max)
            {
                _s_dico_joueur.Release();
                return Tools.PlayerStatus.Full;
            }

            if (_dico_joueur.ContainsKey(id_joueur))
            {
                _s_dico_joueur.Release();
                return Tools.PlayerStatus.Found;
            }

            _dico_joueur.Add(id_joueur, new Player(id_joueur));
            _nombre_joueur++;
            _s_dico_joueur.Release();

            return Tools.PlayerStatus.Success;
        }

        public Tools.PlayerStatus RemoveJoueur(long id_joueur)
        {

            _s_dico_joueur.WaitOne();
            bool res = _dico_joueur.Remove(id_joueur);
            if (res)
            {
                _nombre_joueur--;
                if(id_joueur == _id_moderateur)
                {
                    if(_dico_joueur.Count != 1)
                    {
                        _id_moderateur = _dico_joueur.First().Key;
                    }
                    else
                    {/* Il n'y a plus personne dans la room */
                        Close();
                    }
                }
                    
                _s_dico_joueur.Release();
                
                return Tools.PlayerStatus.Success;
            }
                
            _s_dico_joueur.Release();
            return Tools.PlayerStatus.NotFound;
        }


        public Tools.PlayerStatus SetPlayerTriche(long id_joueur)
        {
            _s_dico_joueur.WaitOne();
            if (!_dico_joueur.ContainsKey(id_joueur))
            {
                _s_dico_joueur.Release();
                return Tools.PlayerStatus.NotFound;
            }

            Player player = _dico_joueur[id_joueur];
            player._s_player.WaitOne();
            player._triche++;
            if(player._triche == (uint)Tools.PlayerStatus.Kicked)
            {
                player._s_player.Release();
                _s_dico_joueur.Release();
                RemoveJoueur(id_joueur);
                return Tools.PlayerStatus.Kicked;
            }
            player._s_player.Release();
            _s_dico_joueur.Release();

            return Tools.PlayerStatus.Success;
        }

        public Tools.PlayerStatus SetPlayerStatus(long id_joueur)
        {
            _s_dico_joueur.WaitOne();
            if (!_dico_joueur.ContainsKey(id_joueur))
            {
                _s_dico_joueur.Release();
                return Tools.PlayerStatus.NotFound;
            }

            Player player = _dico_joueur[id_joueur];
            player._s_player.WaitOne();
            player._is_ready = !player._is_ready;
            player._s_player.Release();
            _s_dico_joueur.Release();

            return Tools.PlayerStatus.Success;
        }

        public Tools.PlayerStatus SetPlayerPoint(long id_joueur,uint score)
        {
            _s_dico_joueur.WaitOne();
            if (!_dico_joueur.ContainsKey(id_joueur))
            {
                _s_dico_joueur.Release();
                return Tools.PlayerStatus.NotFound;
            }

            Player player = _dico_joueur[id_joueur];
            player._s_player.WaitOne();
            player._score += score;
            player._s_player.Release();
            _s_dico_joueur.Release();

            return Tools.PlayerStatus.Success;
        }

        public void Close()
        {
            //TODO
        }

        public void Lancement_thread_serveur_jeu()
        {



            // A FAIRE - Fin/retour de la fonction pour libérer l'objet associé dans le thread_com à la fin de la partie et donc de ce thread
        }
    }

}
