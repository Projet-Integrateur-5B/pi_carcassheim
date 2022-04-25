using System;
using System.Collections.Generic;
using ClassLibrary;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Timers;

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
        private Tools.Meeple _meeples; // Nombre de meeples par joueur
        
        // Timer
        private DateTime _DateTime_game;
        private System.Timers.Timer _timer_game;
        private Tools.Timer _timer_game_value; // En secondes
        private DateTime _DateTime_player;
        private System.Timers.Timer _timer_player;
        private Tools.Timer _timer_player_value; // En secondes

        private ulong _idTuileInit; // Id de la tuile initiale : soit le chemin soit la rivière suivant si le dlc est choisi

        // Locks

        private readonly object _lock_settings;

        // Attribut pour le bon fonctionnement du moteur de jeu

        private Plateau _plateau;
        private uint _offsetActualPlayer; // The offset of the actual player in the _dico_joueur 
        private List<ulong> _tuilesGame; // Totalité des tuiles de la game       
        private string[] _tuilesEnvoyees; // Stock les 3 tuiles envoyées au client à chaque tour
        private ulong _idTuileChoisie; // L'id de la tuile choisie par le client parmis les 3 envoyées
        private Position _posTuileTourActu; // Position temporaire de la tuile de ce tour
        private string[] _posPionTourActu; // Position temporaire du pion de ce tour (à cast) {idPlayer, idTuile, idSlot}

        // Attributs anticheat
        private bool _AC_drawedTilesValid;
        private Barrier _AC_barrierAllVerifDone;

        // Semaphores moteur
        private Semaphore _s_nombre_joueur;
        private Semaphore _s_plateau;
        private Semaphore _s_offsetActualPlayer;
        private Semaphore _s_tuilesGame;
        private Semaphore _s_tuilesEnvoyees;
        private Semaphore _s_posTuileTourActu; // Position temporaire de la tuile de ce tour
        private Semaphore _s_posPionTourActu; // Position temporaire du pion de ce tour

        // Getters et setters

        public Dictionary<ulong, Player> Get_Dico_Joueurs()
        {
            return this._dico_joueur;
        }
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
        
        public ulong Get_ActualPlayerId()
        {
            _s_dico_joueur.WaitOne();
            ulong[] idPlayer_array = _dico_joueur.Keys.ToArray();
            _s_dico_joueur.Release();
            return idPlayer_array[_offsetActualPlayer];
        }

        public Tools.GameStatus Get_Status()
        {
            return this._statut_partie;
        }

        public bool Is_Private()
        {
            return this._privee;
        }
        public bool Get_AC_drawedTilesValid()
        {
            return _AC_drawedTilesValid;
        }
        public void SetValid_AC_drawedTilesValid()
        {
            if(_AC_drawedTilesValid != true)
            {
                _AC_drawedTilesValid = true;
            }
        }

        public Position Get_posTuileTourActu()
        {
            return _posTuileTourActu;
        }

        public string[] Get_posPionTourActu()
        {
            return _posPionTourActu;
        }

        public void Set_idTuileChoisie(ulong idTuile)
        {
            this._idTuileChoisie = idTuile;
        }

        public ulong Get_idTuileChoisie()
        {
            return _idTuileChoisie;
        }

        public ulong GetWinner()
        {
            uint scoreMax = 0;
            ulong winner = _id_moderateur;

            _s_dico_joueur.WaitOne();
            foreach(var player in _dico_joueur)
            {
                if(player.Value._score > scoreMax)
                {
                    winner = player.Key;
                    scoreMax = player.Value._score;
                }
            }
            _s_dico_joueur.Release();

            return winner;
        }

        public ulong Get_idTuileInit()
        {
            return this._idTuileInit;
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
            settingsList.Add(_timer_game_value.ToString());
            settingsList.Add(_timer_player_value.ToString());
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
                                _timer_game_value = Tools.Timer.Minute;
                                break;
                            case 1800:
                                _timer_game_value = Tools.Timer.DemiHeure;
                                break;
                            case 3600:
                                _timer_game_value = Tools.Timer.Heure;
                                break;
                            default:
                                _timer_game_value = Tools.Timer.DemiHeure;
                                break;

                        }

                        switch (int.Parse(settings[6]))
                        {
                            case 10:
                                _timer_player_value = Tools.Timer.DixSecondes;
                                break;
                            case 30:
                                _timer_player_value = Tools.Timer.DemiMinute;
                                break;
                            case 60:
                                _timer_player_value = Tools.Timer.Minute;
                                break;
                            default:
                                _timer_player_value = Tools.Timer.DemiMinute;
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

        /// <summary>
        /// Pass to the next player
        /// </summary>
        /// <returns> Returns the new actual player's id </returns>
        public ulong Get_NextPlayer()
        {
            // Transforme le dico en array pour récupérer le n-ième joueur
            _s_dico_joueur.WaitOne();
            ulong[] idPlayer_array = _dico_joueur.Keys.ToArray();
            _s_dico_joueur.Release();

            // Incrémente le numéro du joueur actuel (modulo nb_joueurs) et récupère l'idPlayer du nouveau joueur
            _s_offsetActualPlayer.WaitOne();
            _s_nombre_joueur.WaitOne();
            _offsetActualPlayer = (_offsetActualPlayer + 1) % _nombre_joueur;
            _s_nombre_joueur.Release();
            ulong nextPlayer = idPlayer_array[_offsetActualPlayer];
            _s_offsetActualPlayer.Release();

            return nextPlayer;
        }

        // Constructeur
        public Thread_serveur_jeu(int id_partie, ulong id_joueur_createur, Socket? playerSocket)
        {
            _id_partie = id_partie;

            /* Zone  du Dictionary Score */
            _dico_joueur = new Dictionary<ulong, Player>();
            _s_dico_joueur = new Semaphore(1, 1);
            _nombre_joueur = 0;
            _s_nombre_joueur = new Semaphore(1, 1);
            _nombre_joueur_max = 8;
            _id_moderateur = id_joueur_createur;

            _statut_partie = Tools.GameStatus.Room;

            // Initialisation des valeurs par défaut
            _mode = Tools.Mode.Default;
            _nb_tuiles = 60;
            _score_max = -1;
            _privee = false; // Une partie est par défaut privée
            _timer_game_value = Tools.Timer.Heure; // Une heure par défaut
            _timer_player_value = Tools.Timer.Minute;
            _meeples = Tools.Meeple.Huit;
            _idTuileInit = 22; // Initialise sans dlc rivière

            // Initialisation des semaphores d'attributs moteurs
            _s_offsetActualPlayer = new Semaphore(1, 1);
            _tuilesGame = new List<ulong>();
            _s_tuilesGame = new Semaphore(1, 1);
            _s_tuilesEnvoyees = new Semaphore(1, 1);

            _s_plateau = new Semaphore(1, 1);
            _s_posPionTourActu = new Semaphore(1, 1);
            _s_posTuileTourActu = new Semaphore(1, 1);

        }

        // ===================
        // Méthodes settings
        // ===================

        public Tools.PlayerStatus AddJoueur(ulong id_joueur, Socket? playerSocket)
        {
            _s_dico_joueur.WaitOne();

            if (_dico_joueur.ContainsKey(id_joueur))
            {
                _s_dico_joueur.Release();
                return Tools.PlayerStatus.Found;
            }

            _s_nombre_joueur.WaitOne();
            if (_nombre_joueur >= _nombre_joueur_max)
            {
                _s_dico_joueur.Release();
                return Tools.PlayerStatus.Full;
            }
            _s_nombre_joueur.Release();

            

            _dico_joueur.Add(id_joueur, new Player(id_joueur, playerSocket));
            _s_nombre_joueur.WaitOne();
            _nombre_joueur++;
            _s_nombre_joueur.Release();
            _s_dico_joueur.Release();

            return Tools.PlayerStatus.Success;
        }

        public Tools.PlayerStatus RemoveJoueur(ulong id_joueur)
        {

            _s_dico_joueur.WaitOne();
            bool res = _dico_joueur.Remove(id_joueur);
            if (res)
            {
                _s_nombre_joueur.WaitOne();
                _nombre_joueur--;
                _s_nombre_joueur.Release();
                if (id_joueur == _id_moderateur)
                {
                    if (_dico_joueur.Count != 0)
                    {
                        _id_moderateur = _dico_joueur.First().Key;
                    }
                    else
                    {/* Il n'y a plus personne dans la room */
                        _statut_partie = Tools.GameStatus.Stopped;
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

        public void SetACBarrier()
        {
            _s_nombre_joueur.WaitOne();
            _AC_barrierAllVerifDone = new Barrier((int)_nombre_joueur);
            _s_nombre_joueur.Release();
        }
        public void WaitACBarrier()
        {
            _AC_barrierAllVerifDone.SignalAndWait(2000);
        }
        public void DisposeACBarrier()
        {
            _AC_barrierAllVerifDone.Dispose();
        }

        public bool EveryoneIsReady()
        {
            bool result = true;

            _s_dico_joueur.WaitOne();
            foreach(var player in _dico_joueur)
            {
                // Vérifie que tous les joueurs
                if(player.Value._is_ready == false)
                {
                    result = false;
                    break;
                }
            }
            _s_dico_joueur.Release();

            return result;
        }

        // =======================
        // Méthodes moteur de jeu
        // =======================

        public void StartGame()
        {
            _statut_partie = Tools.GameStatus.Running;

            // Génération du plateau
            _plateau = new Plateau();

            // Récupération de la liste de joueurs
            _s_dico_joueur.WaitOne();
            ulong[] idPlayer_array = _dico_joueur.Keys.ToArray();
            _s_dico_joueur.Release();

            // Indication du joueur actuel (commence toujours par le modérateur)
            _s_offsetActualPlayer.WaitOne();
            _offsetActualPlayer = 0;
            _s_offsetActualPlayer.Release();

            // Génération des tuiles de la game
            _s_tuilesGame.WaitOne();
            _tuilesGame = Random_sort_tuiles(_nb_tuiles);
            _s_tuilesGame.Release();

            // Génération des attributs d'anti cheat
            _AC_drawedTilesValid = false;

            // Génération du dicoTuile de la classe tuile
            Tuile.DicoTuiles = LireXML2.Read("system/config_back.xml");

            // Initialise la tuile placée de ce tour inexistante
            _posTuileTourActu = new Position();
            _posTuileTourActu.SetNonExistent();

            // Pose la première tuile de la partie
            _s_plateau.WaitOne();
            _plateau.PoserTuile(_idTuileInit, new Position(0, 0, 0));
            _s_plateau.Release();

            _timer_game = new System.Timers.Timer();
            _timer_game.Interval = 1000;
            _timer_game.Elapsed += OnTimedEventGame;
            _DateTime_game = DateTime.Now;
            _timer_game.AutoReset = true;
            _timer_game.Enabled = true;
            
            _timer_player = new System.Timers.Timer();
            _timer_player.Interval = 1000;
            _timer_player.Elapsed += OnTimedEventPlayer;
            _DateTime_player = DateTime.Now;
            _timer_player.AutoReset = true;
            _timer_player.Enabled = true;
            
            // TODO :
            // synchronisation de la methode
            // genere les tuiles
            // envoie 3 tuiles au player 1
            // start timer
            // return valeur d'erreur pour la méthode parent
        }
        
        private void OnTimedEventGame(Object source, System.Timers.ElapsedEventArgs e)
        {
            var diff = DateTime.Now.Subtract(_DateTime_game).Seconds;
            if (diff <= (int) _timer_game_value) return;
            Console.WriteLine("Game was raised at {0}. EndGame() is called", e.SignalTime);
            EndGame();
        }
        
        private void OnTimedEventPlayer(Object source, System.Timers.ElapsedEventArgs e)
        {
            var diff = DateTime.Now.Subtract(_DateTime_player).Seconds;
            if (diff <= (int) _timer_player_value) return;
            var idPlayer = Get_ActualPlayerId();
            Console.WriteLine("Player was raised at {0}. EndTurn({1}) is called", e.SignalTime, idPlayer);
            EndTurn(idPlayer);
        }

        /// <summary>
        /// Get three tiles' id from the game's list of tile
        /// </summary>
        /// <returns> A array of 3 tiles' id </returns>
        public string[] GetThreeLastTiles()
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
        /// Shuffles the list of game's tiles
        /// </summary>
        public void ShuffleTilesGame()
        {
            _s_tuilesGame.WaitOne();

            List<ulong> tuilesGame_resultat = new List<ulong>();
            var rnd = new System.Random();
            var randomedList = _tuilesGame.OrderBy(item => rnd.Next());
            foreach (var value in randomedList)
            {
                tuilesGame_resultat.Add(value);
            }
            
            _tuilesGame = tuilesGame_resultat;
            _s_tuilesGame.Release();

        }


        public Tools.Errors TilePlacement(ulong idPlayer, ulong idTuile, int posX, int posY, int rotat)
        {
            _s_plateau.WaitOne();
            // Si placement légal, on le sauvegarde
            if (isTilePlacementLegal(idTuile, posX, posY, rotat)){

                _s_posTuileTourActu.WaitOne();
                _posTuileTourActu = new Position(posX, posY, rotat);
                _s_posTuileTourActu.Release();

                return Tools.Errors.None;
            }
            else // Si non, renvoie l'erreur illegalPlay + envoi message cheat
            {
                // Le thread de com s'occupera d'appeller le setplayerstatus pour indiquer la triche

                return Tools.Errors.IllegalPlay;
            }
            _s_plateau.Release();
        }

        public Tools.Errors PionPlacement(ulong idPlayer, ulong idTuile, int idMeeple, int slotPos)
        {
            _s_plateau.WaitOne();
            // Si placement légal, on le sauvegarde
            if (isPionPlacementLegal(idTuile, slotPos, idPlayer))
            {
                string[] posPion = new string[] { idPlayer.ToString(), idTuile.ToString(), slotPos.ToString() };
                _s_posPionTourActu.WaitOne();
                _posPionTourActu = posPion;
                _s_posPionTourActu.Release();

                return Tools.Errors.None;
            }
            else // Si non, renvoie l'erreur illegalPlay + envoi message cheat
            {
                // Le thread de com s'occupera d'appeller le setplayerstatus pour indiquer la triche

                return Tools.Errors.IllegalPlay;
            }
            _s_plateau.Release();
        }


        public bool isTilePlacementLegal(ulong idTuile, int posX, int posY, int rotat)
        {
            return _plateau.PlacementLegal(idTuile, posX, posY, rotat);
        }

        public bool isPionPlacementLegal(ulong idTuile, int idSlot, ulong idPlayer)
        {
            return _plateau.PionPosable(idTuile, (ulong)idSlot, idPlayer);
        }

        public void RetirerTuileTourActu()
        {
            _s_posTuileTourActu.WaitOne();
            _posTuileTourActu.SetNonExistent();
            _s_posTuileTourActu.Release();
        }

        public void RetirerPionTourActu()
        {
            _s_posPionTourActu.WaitOne();
            _posPionTourActu = new string[] { };
            _s_posPionTourActu.Release();
        }

        public Tools.GameStatus UpdateGameStatus()
        {
            Tools.GameStatus statutGame = _statut_partie;

            // Update suivant le mode de jeu
            switch (_mode)
            {
                case Tools.Mode.Default:
                    if(_tuilesGame.Count == 0)
                    {
                        statutGame = Tools.GameStatus.Stopped;
                    }
                    break;
                case Tools.Mode.TimeAttack:

                    //TODO

                    break;
                case Tools.Mode.Score:

                    //TODO

                    break;
            }

            _statut_partie = statutGame;

            return statutGame;
        }

        public void RetirerTuileGame(ulong idTuile)
        {
            _s_tuilesGame.WaitOne();
            int indexOfTile = -1;
            for(int i = 0; i<_tuilesGame.Count; i++)
            {
                if(_tuilesGame[i] == idTuile)
                {
                    indexOfTile = i;
                }
            }
            _tuilesGame.RemoveAt(indexOfTile);
            _s_tuilesGame.Release();
        }

        public Socket? CancelTurn(string idRoom)
        {
            // Remise à inexistant la tuilePosActu et pionPosActu
            _s_posTuileTourActu.WaitOne();
            _posTuileTourActu.SetNonExistent();
            _s_posTuileTourActu.Release();

            _s_posPionTourActu.WaitOne();
            _posPionTourActu = new string[] { };
            _s_posPionTourActu.Release();

            // Passe au joueur suivant
            ulong nextPlayer = Get_NextPlayer();

            _s_dico_joueur.WaitOne();
            Socket? nextPlayerSocket = _dico_joueur[nextPlayer]._socket_of_player;
            _s_dico_joueur.Release();


            return nextPlayerSocket;
        }

        /// <summary>
        /// End the turn
        /// </summary>
        /// <returns> The socket of the next player to play </returns>
        public Socket? EndTurn(ulong idPlayer)
        {

            // Prise en compte du placement de la tuile et du pion (mise à jour structure de données)
            _s_plateau.WaitOne();
            _plateau.PoserTuile(_idTuileChoisie, _posTuileTourActu);
            try
            {
                _plateau.PoserPion(idPlayer, _idTuileChoisie, UInt64.Parse(_posPionTourActu[2]));
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: On PoserTuile : " + ex);
            }          
            _s_plateau.Release();


            // Vérification des fermetures de chemins (mise à jour des points)
            int pointsGagnes = CompteurPoints.CompterZoneFerme(_idTuileChoisie, Int32.Parse(_posPionTourActu[2]), idPlayer);
            _s_dico_joueur.WaitOne();
            _dico_joueur[idPlayer].AddPoints((uint)pointsGagnes);
            _s_dico_joueur.Release();


            RetirerTuileGame(_idTuileChoisie);


            // Remise à inexistant la tuilePosActu et pionPosActu
            _s_posTuileTourActu.WaitOne();
            _posTuileTourActu.SetNonExistent();
            _s_posTuileTourActu.Release();

            _s_posPionTourActu.WaitOne();
            _posPionTourActu = new string[] { };
            _s_posPionTourActu.Release();

            // Passe au joueur suivant
            ulong nextPlayer = Get_NextPlayer();

            _s_dico_joueur.WaitOne();
            Socket? nextPlayerSocket = _dico_joueur[nextPlayer]._socket_of_player;
            _s_dico_joueur.Release();
            
            // reset player timer
            _DateTime_player = DateTime.Now;

            return nextPlayerSocket;
        }

        public void EndGame()
        {
            _statut_partie = Tools.GameStatus.Stopped;

            // TODO :
            // synchronisation de la methode
            // close timer
            // return valeur d'erreur pour la méthode parent
        }

        public void TimerPlayer(ulong idPlayer)
        {
            // TODO :
            // inc player timer count + check limit ?
        }

        // =================================
        // Méthodes supplémentaires moteur
        // =================================

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

    }

}
