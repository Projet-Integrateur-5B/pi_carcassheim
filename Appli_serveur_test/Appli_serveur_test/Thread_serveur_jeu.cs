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
        private Dictionary<ulong, Player> _dico_joueur; // Contient les ID's de chaque joueur
        private Semaphore _s_dico_joueur;
        private uint _nombre_joueur;
        private uint _nombre_joueur_max;
        private ulong _id_moderateur; // Identifiant du joueur modérateur

        private string _statut_partie;

        private Tools.Mode _mode; // 0 -> Classique | 1 -> Time-attack | 2 -> Score

        private int _nb_tuiles;
        private int _score_max;

        private bool _privee;
        private Tools.Timer _timer; // En secondes
        private Tools.Timer _timer_max_joueur; // En secondes
        private Tools.Meeple _meeples; // Nombre de meeples par joueur

        // Locks

        private readonly object _lock_settings;

        // Champs nécessaires pour le bon fonctionnement du programme

        // Getters et setters

        public int Get_ID()
        {
            return this._id_partie;
        }
        
        public ulong Get_Moderateur()
        {
            return this._id_moderateur;
        }

        public bool Is_Private()
        {
            return this._privee;
        }

        public uint NbJoueurs
        {
            get { return this._nombre_joueur; }
        }

        public uint NbJoueursMax
        {
            get { return this._nombre_joueur_max;  }
        }

        /// <summary>
        /// Settings of that game
        /// </summary>
        /// <returns> 
        /// Returns a string[] in that order: { id_moderator, player_number, player_number_max,
        /// private, mode, number_tiles, number_pawn, timer, timer_max_player }.
        /// </returns>
        public string[] Get_Settings()
        {
            List<string> settingsList = new List<string>();

            settingsList.Add(_id_moderateur.ToString());
            settingsList.Add(_nombre_joueur.ToString());
            settingsList.Add(_nombre_joueur_max.ToString());
            settingsList.Add(_privee.ToString());
            settingsList.Add(_mode.ToString());
            settingsList.Add(_nb_tuiles.ToString());
            settingsList.Add(_meeples.ToString());
            settingsList.Add(_timer.ToString());
            settingsList.Add(_timer_max_joueur.ToString());
            settingsList.Add(_score_max.ToString());
           
            return settingsList.ToArray();
        }

        /// <summary>
        /// Sets the settings of the game
        /// </summary>
        /// <param name="idPlayer"> Id of the player making the request </param>
        /// <param name="settings"> The new set of settings (in the same order than Get_settings, without the 2 firsts) </param>
        public void Set_Settings(ulong idPlayer, string[] settings)
        {
            if (idPlayer == Get_Moderateur())
            {
                lock (_lock_settings)
                {
                    try
                    {
                        _nombre_joueur_max = Convert.ToUInt32(settings[0]);
                        _privee = bool.Parse(settings[1]);

                        switch (Int32.Parse(settings[2]))
                        {
                            case 0:
                                _mode = Tools.Mode.Default;
                                break;
                            case 1:
                                _mode = Tools.Mode.TimeAttack;
                                break;
                            case 2:
                                _mode = Tools.Mode.Score;
                                break;
                            default:
                                _mode = Tools.Mode.Default;
                                break;
                        }

                        _nb_tuiles = int.Parse(settings[3]);

                        switch (int.Parse(settings[4]))
                        {
                            case 4:
                                _meeples = Tools.Meeple.Quatre;
                                break;
                            case 8:
                                _meeples = Tools.Meeple.Huit;
                                break;
                            case 10:
                                _meeples = Tools.Meeple.Dix;
                                break;
                            default:
                                _meeples = Tools.Meeple.Huit;
                                break;
                        }

                        switch (int.Parse(settings[5]))
                        {
                            case 60:
                                _timer = Tools.Timer.Minute;
                                break;
                            case 1800:
                                _timer = Tools.Timer.DemiHeure;
                                break;
                            case 3600:
                                _timer = Tools.Timer.Heure;
                                break;
                            default:
                                _timer = Tools.Timer.DemiHeure;
                                break;

                        }

                        switch (int.Parse(settings[6]))
                        {
                            case 10:
                                _timer = Tools.Timer.DixSecondes;
                                break;
                            case 30:
                                _timer = Tools.Timer.DemiMinute;
                                break;
                            case 60:
                                _timer = Tools.Timer.Minute;
                                break;
                            default:
                                _timer = Tools.Timer.DemiMinute;
                                break;

                        }

                        _score_max = int.Parse(settings[7]);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERROR: Set_settings : " + ex);
                        return;
                    }
                }
                
            }
        }

        // Constructeur
        public Thread_serveur_jeu(int id_partie, ulong id_joueur_createur)
        {
            _id_partie = id_partie;

            /* Zone  du Dictionary Score */
            _dico_joueur = new Dictionary<ulong, Player>();
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

        public Tools.PlayerStatus AddJoueur(ulong id_joueur)
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

        public Tools.PlayerStatus RemoveJoueur(ulong id_joueur)
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


        public Tools.PlayerStatus SetPlayerTriche(ulong id_joueur)
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

        public Tools.PlayerStatus SetPlayerStatus(ulong id_joueur)
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

        public Tools.PlayerStatus SetPlayerPoint(ulong id_joueur,uint score)
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
