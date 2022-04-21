using System;
using System.Collections.Generic;
using ClassLibrary;
using System.Net;
using System.Net.Sockets;

using Server;

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

        private Tools.GameStatus _statut_partie;

        private Tools.Mode _mode; // 0 -> Classique | 1 -> Time-attack | 2 -> Score

        private int _nb_tuiles;
        private int _score_max;

        private bool _privee;
        private Tools.Timer _timer; // En secondes
        private Tools.Timer _timer_max_joueur; // En secondes
        private Tools.Meeple _meeples; // Nombre de meeples par joueur

        // Locks

        private readonly object _lock_settings;

        // Attribut pour le bon fonctionnement du moteur de jeu

        private ulong _id_joueur_actuel;
        private Semaphore _s_id_joueur_actuel;
        private List<ulong> _tuilesGame; // Totalité des tuiles de la game
        private Semaphore _s_tuilesGame;
        private string[] _tuilesEnvoyees; // Stock les 3 tuiles envoyées au client à chaque tour
        private Semaphore _s_tuilesEnvoyees;

        // Sockets des joueurs de la partie
        private Dictionary<ulong, List<Socket?>> _dico_player_sockets;
        private Semaphore _s_dico_player_sockets;

        // Getters et setters

        public int Get_ID()
        {
            return this._id_partie;
        }

        public Tools.Mode Get_Mode()
        {
            return this._mode;
        }

        public ulong Get_Moderateur()
        {
            return this._id_moderateur;
        }

        public Tools.GameStatus Get_Status()
        {
            return this._statut_partie;
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
            get { return this._nombre_joueur_max; }
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

        public ulong Get_NextPlayer()
        {
            // TODO :
            return 0;
        }

        // Constructeur
        public Thread_serveur_jeu(int id_partie, ulong id_joueur_createur, Socket? playerSocket)
        {
            _id_partie = id_partie;

            /* Zone  du Dictionary Score */
            _dico_joueur = new Dictionary<ulong, Player>();
            _s_dico_joueur = new Semaphore(0, 1);
            _nombre_joueur = 1;
            _nombre_joueur_max = 8;
            _dico_joueur.Add(id_joueur_createur, new Player(id_joueur_createur, playerSocket));
            _id_moderateur = id_joueur_createur;

            _statut_partie = Tools.GameStatus.Room;

            // Initialisation des valeurs par défaut
            _mode = Tools.Mode.Default;
            _nb_tuiles = 60;
            _score_max = -1;
            _privee = true; // Une partie est par défaut privée
            _timer = Tools.Timer.Heure; // Une heure par défaut
            _timer_max_joueur = Tools.Timer.Minute;
            _meeples = Tools.Meeple.Huit;

            // Initialisation des attributs moteurs
            _id_joueur_actuel = 0;
            _s_id_joueur_actuel = new Semaphore(0, 1);
            _tuilesGame = new List<ulong>();
            _s_tuilesGame = new Semaphore(0, 1);

            // Initialisation des attributs sockets
            _dico_player_sockets = new Dictionary<ulong, List<Socket?>>();
            _s_dico_player_sockets = new Semaphore(0, 1);

        }

        // Méthodes

        public Tools.PlayerStatus AddJoueur(ulong id_joueur, Socket? playerSocket)
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

            _dico_joueur.Add(id_joueur, new Player(id_joueur, playerSocket));
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
                if (id_joueur == _id_moderateur)
                {
                    if (_dico_joueur.Count != 1)
                    {
                        _id_moderateur = _dico_joueur.First().Key;
                    }
                    else
                    {/* Il n'y a plus personne dans la room */
                        EndGame();
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
            if (player._triche == (uint)Tools.PlayerStatus.Kicked)
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

        public Tools.PlayerStatus SetPlayerPoint(ulong id_joueur, uint score)
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

        // =======================
        // Méthodes moteur de jeu
        // =======================

        public void StartGame()
        {
            _statut_partie = Tools.GameStatus.Running;

            // Récupération de la liste de joueurs
            _s_dico_joueur.WaitOne();
            ulong[] idPlayer_array = _dico_joueur.Keys.ToArray();
            _s_dico_joueur.Release();

            // Indication du joueur actuel
            _s_id_joueur_actuel.WaitOne();
            _id_joueur_actuel = idPlayer_array[_id_joueur_actuel];
            _s_id_joueur_actuel.Release();

            // Génération des tuiles de la game
            _s_tuilesGame.WaitOne();
            _tuilesGame = Random_sort_tuiles(_nb_tuiles);
            _s_tuilesGame.Release();

            // TODO :
            // synchronisation de la methode
            // genere les tuiles
            // envoie 3 tuiles au player 1
            // start timer
            // return valeur d'erreur pour la méthode parent
        }

        /// <summary>
        /// Get three tiles' id from the game's list of tile
        /// </summary>
        /// <returns> A array of 3 tiles' id </returns>
        public string[] GenerateThreeTiles()
        {
            // Génère les 3 tuiles à envoyer
            List<string> tuilesTirees = tirageTroisTuiles(_tuilesGame);

            // Met à jour le stockage des 3 tuiles envoyées
            _s_tuilesEnvoyees.WaitOne();
            _tuilesEnvoyees = tuilesTirees.ToArray();
            _s_tuilesEnvoyees.Release();
            
            return tuilesTirees.ToArray();
        }

        /// <summary>
        /// End the turn
        /// </summary>
        /// <returns> The ID of the next player to play </returns>
        public ulong EndTurn()
        {

            return 0;
        }

        public void EndGame()
        {
            _statut_partie = Tools.GameStatus.Stopped;

            // TODO :
            // synchronisation de la methode
            // close timer
            // return valeur d'erreur pour la méthode parent
        }

        public void Round()
        {
            // TODO :
            // synchronisation de la methode
            // check que c'est bien au tour du joueur
            // calcul des points
            // envoie 3 tuiles au player suivant
            // update timer
            // return valeur d'erreur pour la méthode parent
        }

        public void TimerPlayer(ulong idPlayer)
        {
            // TODO :
            // inc player timer count + check limit ?
            Round();
        }

        /// <summary>
        /// Get 3 tiles id in string format from a tile list
        /// </summary>
        /// <param name="tuiles"> Tiles list </param>
        /// <returns> List of 3 tiles id in string format </returns>
        public static List<string> tirageTroisTuiles(List<ulong> tuiles)
        {
            List<string> list = new List<string>();
            for (int i = tuiles.Count - 3; i < tuiles.Count; i++)
                list.Add(tuiles[i].ToString());
            return list;
        }
        public static List<ulong> suppTuileChoisie(List<ulong> tuiles, ulong idTuile)
        {
            int i = 0;
            for (i = tuiles.Count - 1; i >= 0 && tuiles[i] != idTuile; i--) ;
            tuiles.Remove(tuiles[i]);

            return tuiles;
        }


        public static ulong tuile_a_tirer(ulong id, int x, Dictionary<ulong, ulong> dico)
        {
            ulong sum = 0;
            foreach (var item in dico)          //Parcourir dico:
            {
                if ((int)(sum + item.Value) < x)
                {
                    sum += item.Value;          //chercher l'id correspondant
                                                //Tant que sum+la proba de tuile actuele > id de tuile --> avance
                }

                else
                {
                    id = item.Key;              //id retrouvé
                    break;
                }

            }

            return id;
        }

        /// <summary>
        /// Get a randomized list of tiles 
        /// </summary>
        /// <param name="nbTuiles"> Number of tiles to have in the list </param>
        /// <returns> List of tiles </returns>
        public static List<ulong> Random_sort_tuiles(int nbTuiles)
        {
            List<ulong> list = null;
            list = new List<ulong>();
            System.Random MyRand = new System.Random();
            int x = 0;
            ulong idTuile = 0, sumDesProbas = 0;

            //Recuperer les id des tuiles et leurs probas depuis la bdd

            //Dictionary<int, int> map = new Dictionary<int, int>();
            //La section suivante est à remplacer par une methode de l'équipe BDD qui retourne 
            //un dico des ids de tuile avec leurs probas
            /*************************/
            Dictionary<ulong, ulong> map = new Dictionary<ulong, ulong>();

            var db = new Database();
            db.RemplirTuiles(map);


            //Parcourir le dictionnaire resultat pour calculer la somme des probabilités des tuiles:
            foreach (var item in map)
            {
                sumDesProbas += item.Value;

            }
            int tmp = (int)(sumDesProbas - sumDesProbas % 1.0);
            //Tirage aléatoire des tuiles
            for (int i = 0; i < nbTuiles; i++)
            {
                x = MyRand.Next(tmp);
                idTuile = tuile_a_tirer(idTuile, x, map);
                list.Add(idTuile);

            }
            //Retourner la liste 
            return list;

        }




        public void Lancement_thread_serveur_jeu()
        {



            // A FAIRE - Fin/retour de la fonction pour libérer l'objet associé dans le thread_com à la fin de la partie et donc de ce thread
        }
    }

}
