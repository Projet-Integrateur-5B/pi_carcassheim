using System;
using System.Threading;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using ClassLibrary;


namespace system
{

    public class Thread_communication
    {
        // Attributs

        private int _id_thread_com;
        private int _numero_localPort;
        private int _numero_remotePort;
        private int _nb_parties_gerees;

        private List<int> _id_parties_gerees;
        private List<Thread_serveur_jeu> _lst_serveur_jeu;

        // Locks

        private readonly object _lock_nb_parties_gerees;
        private readonly object _lock_id_parties_gerees;

        // =============
        // Constructeur
        // =============

        public Thread_communication(int num_localPort, int num_remotePort, int id)
        {
            _numero_localPort = num_localPort;
            _numero_remotePort = num_remotePort;
            _nb_parties_gerees = 0;
            _id_parties_gerees = new List<int>();
            _id_thread_com = id;
            _lst_serveur_jeu = new List<Thread_serveur_jeu>();
            _lock_nb_parties_gerees = new object();
            _lock_id_parties_gerees = new object();
        }

        // ==================
        // Getters et setters
        // ==================

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

        public int Get_localPort()
        {
            return _numero_localPort;
        }
        
        public int Get_remotePort()
        {
            return _numero_remotePort;
        }

        public List<Thread_serveur_jeu> Get_list_server_thread()
        {
            return this._lst_serveur_jeu;
        }

        // ========================
        // Méthodes communication
        // ========================

        /// <summary>
        /// Broadcasts a message to all except the player initiating the request
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="idMessage"></param>
        /// <param name="idPlayer"> Player originating the broadcast </param>
        /// <param name="data"></param>
        public void SendBroadcast(string roomId, Tools.IdMessage idMessage, ulong idPlayer, string[] data)
        {
            // Generate packet
            Packet packet = new Packet();
            packet.IdMessage = idMessage;

            packet.IdPlayer = idPlayer;

            packet.Data = data;

            foreach (Thread_serveur_jeu thread_serv_ite in Get_list_server_thread())
            {
                if (thread_serv_ite.Get_ID() == Int32.Parse(roomId))
                {
                    // Envoi à chaque joueur
                    foreach (var joueur in thread_serv_ite.Get_Dico_Joueurs())
                    {
                        // On envoie le display à tous sauf au joueur dont c'est l'action (si tuileDrawn on envoit à tous)
                        if (joueur.Key != idPlayer || idMessage == Tools.IdMessage.TuileDraw) 
                        {
                            ClientAsync.Send(joueur.Value._socket_of_player, packet);

                            // Lancement de l'écoute de la réponse du joueur 
                            ClientAsync.OnPacketReceived += OnPacketReceived;
                            ClientAsync.Receive(joueur.Value._socket_of_player);

                        }
                    }
                }
            }
        }

        // -----------------------------------------
        // Surcharges de la méthode de communication
        // -----------------------------------------

        public void SendBroadcast(string roomId, Tools.IdMessage idMessage)
        {
            SendBroadcast(roomId, idMessage, 0, Array.Empty<string>());
        }

        public void SendBroadcast(string roomId, Tools.IdMessage idMessage, ulong idPlayer)
        {
            SendBroadcast(roomId, idMessage, idPlayer, Array.Empty<string>());
        }

        public void SendBroadcast(string roomId, Tools.IdMessage idMessage, string[] data)
        {
            SendBroadcast(roomId, idMessage, 0, data);
        }

        // =================
        // Méthodes moteur
        // =================

        /// <summary>
        /// Add a new game (room)
        /// </summary>
        /// <param name="playerId"> Id of the moderator </param>
        /// <param name="socket"> Socket of the moderator (first player) </param>
        /// <returns> The id of the game (-1 if error occurs) </returns>
        public int AddNewGame(ulong playerId, Socket? playerSocket)
        {

            int id_nouv_partie = -1;

            lock (this._lock_nb_parties_gerees)
            {
                if (_nb_parties_gerees < 5)
                {
                    // Dixaine : numéro de thread, unité : numéro de partie
                    id_nouv_partie = _id_thread_com * 10 + (_nb_parties_gerees + 1);

                    lock (this._lock_id_parties_gerees)
                    {
                        _id_parties_gerees.Add(id_nouv_partie);
                    }

                    _nb_parties_gerees++;
                }
            }

            if (id_nouv_partie != -1) // Si la partie a pu être crée
            {
                Thread_serveur_jeu thread_serveur_jeu = new Thread_serveur_jeu(id_nouv_partie, playerId, playerSocket);

                _lst_serveur_jeu.Add(thread_serveur_jeu);

                return id_nouv_partie;

            }
            else // La partie n'a pas pu être créée
            {
                return id_nouv_partie;
            }


        }

        public void DeleteGame(string roomId)
        {
            int indexOfRoom = 0;
            foreach (Thread_serveur_jeu thread_serv_ite in Get_list_server_thread())
            {
                if (thread_serv_ite.Get_ID() == Int32.Parse(roomId))
                {
                    lock (_lock_nb_parties_gerees)
                    {
                        _nb_parties_gerees--;
                    }
                    lock (_lock_id_parties_gerees)
                    {
                        var idToRemove = _id_parties_gerees.Single(id => id.Equals(roomId));
                        _id_parties_gerees.Remove(idToRemove);
                    }
                    break;
                }

                indexOfRoom++;
            }

            _lst_serveur_jeu.RemoveAt(indexOfRoom);
        }

        /// <summary>
        /// Get the parameters needed to communicate with a client
        /// </summary>
        /// <param name="socket"> The listening socket that has received communications from player </param>
        /// <param name="idPlayer"> Id of the player </param>
        /// <returns> The parameters of the player's socket </returns>
        public Parameters GetPlayerSocketParameters(Socket? socket, ulong idPlayer)
        {
            Parameters playerParameters = new Parameters();

            IPAddress playerIpAddress = ((IPEndPoint)socket.RemoteEndPoint).Address;
            int playerPort = ((IPEndPoint)socket.RemoteEndPoint).Port;

            playerParameters.ServerIP = playerIpAddress.ToString();
            playerParameters.ServerPort = playerPort;

            return playerParameters;
        }


        // La fonction du joueur dont c'est le tour
        public void DrawAntiCheatPlayer(string idRoom, ulong idPlayer, Socket? playerSocket, string[] tuilesEnvoyees)
        {
            foreach (Thread_serveur_jeu threadJeu in Get_list_server_thread())
            {
                if (threadJeu.Get_ID() == Int32.Parse(idRoom))
                {
                    threadJeu.SetACBarrier();

                    // On attend que tous les autres joueurs aient exécuté leur rôle d'arbitre
                    threadJeu.WaitACBarrier();

                    // Vérification de la validité des tuiles piochées
                    if (threadJeu.Get_AC_drawedTilesValid())
                    {
                        // Les tuiles s'avèrent valides, on a affaire à un tricheur
                        PlayerCheated(idPlayer, playerSocket, idRoom);
                        // On renvoie les 3 mêmes tuiles
                        SendBroadcast(idRoom, Tools.IdMessage.TuileDraw, threadJeu.GetThreeLastTiles());
                    }
                    else
                    {
                        // En effet aucune tuile n'est valide, nous renvoyons trois nouvelles tuiles
                        threadJeu.ShuffleTilesGame();
                        SendBroadcast(idRoom, Tools.IdMessage.TuileDraw, threadJeu.GetThreeLastTiles());

                    }

                    threadJeu.DisposeACBarrier();

                    break;
                }
            }
        }

        // La fonction pour les autres joueurs, qui ne servent que d'arbitre
        public void DrawAntiCheatVerif(string idRoom, bool isValid, ulong idTuile, Position pos)
        {
            foreach (Thread_serveur_jeu threadJeu in Get_list_server_thread())
            {
                if (threadJeu.Get_ID() == Int32.Parse(idRoom))
                {
                    if (isValid)
                    {
                        bool isLegal = threadJeu.isTilePlacementLegal(idTuile, pos.X, pos.Y, pos.ROT);
                        if (isLegal) // S'il s'avère que le coup est valide, on passe l'attribut à true
                        {
                            threadJeu.SetValid_AC_drawedTilesValid();
                        }
                    }

                    // Signale que le rôle d'arbitre de ce joueur a été joué
                    threadJeu.WaitACBarrier();

                    break;
                }
            }
        }

        public void ChooseIdTile(string idRoom, ulong idPlayer, ulong idTuile, Position exemplePos, Socket? playerSocket)
        {
            foreach (Thread_serveur_jeu threadJeu in Get_list_server_thread())
            {
                if (threadJeu.Get_ID() == Int32.Parse(idRoom))
                {
                    // Vérifie que l'exemple de position est bon
                    bool isLegal = threadJeu.isTilePlacementLegal(idTuile, exemplePos.X, exemplePos.Y, exemplePos.ROT);
                    if (isLegal) // Si tuile en effet posable
                    {
                        // Sauvegarde de l'id de la tuile choisie
                        threadJeu.Set_idTuileChoisie(idTuile);
                    }
                    else
                    {
                        // La tuile n'est pas vraiment posable
                        PlayerCheated(idPlayer, playerSocket, idRoom);

                        // Renvoie les 3 tuiles
                        string[] tuilesAEnvoyer = threadJeu.GetThreeLastTiles();
                        SendBroadcast(idRoom, Tools.IdMessage.TuileDraw, threadJeu.GetThreeLastTiles());
                    }

                    break;
                }
            }
        }

        public Tools.Errors VerifyTilePlacement(ulong idPlayer, Socket? playerSocket, string idRoom, string idTuile, string posX, string posY, string rotat)
        {
            // Si la demande ne trouve pas de partie ou qu'elle ne provient pas d'un joueur à qui c'est le tour : permission error
            Tools.Errors errors = Tools.Errors.Permission;

            // Parcours des threads de jeu pour trouver celui qui gère la partie cherchée

            foreach (Thread_serveur_jeu thread_serv_ite in _lst_serveur_jeu)
            {
                if (idRoom != thread_serv_ite.Get_ID().ToString()) continue;
                if (idPlayer == thread_serv_ite.Get_ActualPlayerId())
                {
                    // Vérification qu'une tuile n'a pas déjà été placée auparavant 
                    if (thread_serv_ite.Get_posTuileTourActu().IsExisting() == true)
                    {
                        // Permission refusée de poser une tuile
                        errors = Tools.Errors.Permission;
                        break;
                    }
                    else if (thread_serv_ite.Get_idTuileChoisie() != UInt64.Parse(idTuile)) // Vérifie qu'il s'agit de la même qu'il essaie de poser
                    {
                        // Coup illégal : tentative de pose d'une autre tuile que celle choisie
                        errors = Tools.Errors.IllegalPlay;
                        PlayerCheated(idPlayer, playerSocket, idRoom);
                        break;
                    }
                    else
                    {
                        // Vérification du placement
                        errors = thread_serv_ite.TilePlacement(idPlayer, UInt64.Parse(idTuile), Int32.Parse(posX), Int32.Parse(posY), Int32.Parse(rotat));

                        if (errors == Tools.Errors.None) // Si coup légal
                        {
                            // Envoi de l'information à tous pour l'affichage
                            string[] dataToSend = new string[] { idTuile, posX, posY, rotat };
                            SendBroadcast(idRoom, Tools.IdMessage.TuilePlacement, idPlayer, dataToSend);
                        }
                            
                        break;
                    }

                     
                }

            }
            
            if(errors == Tools.Errors.IllegalPlay)
            {
                PlayerCheated(idPlayer, playerSocket, idRoom);
            }

            return errors; // return valeur correcte
        }

        public Tools.Errors VerifyPionPlacement(ulong idPlayer, Socket? playerSocket, string idRoom, string idTuile, string idMeeple, string slotPos)
        {
            // Si la demande ne trouve pas de partie ou qu'elle ne provient pas d'un joueur à qui c'est le tour : permission error
            Tools.Errors errors = Tools.Errors.Permission;

            // Parcours des threads de jeu pour trouver celui qui gère la partie cherchée

            foreach (Thread_serveur_jeu thread_serv_ite in _lst_serveur_jeu)
            {
                if (idRoom != thread_serv_ite.Get_ID().ToString()) continue;
                if (idPlayer == thread_serv_ite.Get_ActualPlayerId())
                {
                    // Vérification qu'une tuile a bien été placée auparavant ET qu'aucun pion n'est pas déjà placé
                    if(thread_serv_ite.Get_posTuileTourActu().IsExisting() == true && 
                        thread_serv_ite.Get_posPionTourActu().Length == 0)
                    {
                        // Vérification qu'il lui reste un pion
                        if(thread_serv_ite.Get_Dico_Joueurs()[idPlayer]._nbMeeples < 1)
                        {
                            return Tools.Errors.Permission;
                        }

                        // Vérification du placement
                        errors = thread_serv_ite.PionPlacement(idPlayer, UInt64.Parse(idTuile), Int32.Parse(idMeeple), Int32.Parse(slotPos));
                        
                        if(errors == Tools.Errors.None) // Si placement légal
                        {
                            // Envoi de l'information à tous pour l'affichage 
                            string[] dataToSend = new string[] { idTuile, idMeeple, slotPos };
                            SendBroadcast(idRoom, Tools.IdMessage.PionPlacement, idPlayer, dataToSend);
                        }
                        
                        break;
                    }

                    // Dans le cas où aucun pion n'a pas été placée auparavant ou qu'un pion est déjà placé,
                    // on renvoie une erreur Permission                 
                }

            }

            if (errors == Tools.Errors.IllegalPlay)
            {
                PlayerCheated(idPlayer, playerSocket, idRoom);
            }

            return errors; // return valeur correcte
        }

        public Tools.Errors CancelTuilePlacement(ulong idPlayer, Socket? playerSocket, string idRoom)
        {
            // Si la demande ne trouve pas de partie ou qu'elle ne provient pas d'un joueur à qui c'est le tour : permission error
            Tools.Errors errors = Tools.Errors.Permission;

            // Parcours des threads de jeu pour trouver celui qui gère la partie cherchée

            foreach (Thread_serveur_jeu thread_serv_ite in _lst_serveur_jeu)
            {
                if (idRoom != thread_serv_ite.Get_ID().ToString()) continue;
                if (idPlayer == thread_serv_ite.Get_ActualPlayerId())
                {
                    // Vérification qu'une tuile a bien été placée auparavant ET qu'un pion n'y est pas déjà placé
                    if (thread_serv_ite.Get_posTuileTourActu().IsExisting() == true &&
                        thread_serv_ite.Get_posPionTourActu().Length == 0)
                    {
                        // Retrait de la position de tuile
                        thread_serv_ite.RetirerTuileTourActu();

                        // Envoie l'information display
                        SendBroadcast(idRoom, Tools.IdMessage.CancelTuilePlacement, idPlayer);

                        errors = Tools.Errors.None;
                        break;
                    }

                    // Dans le cas où aucune tuile n'a été placée auparavant ou qu'un pion est toujours placé,
                    // on renvoie une erreur Permission                 
                }

            }

            return errors; 
        }

        public Tools.Errors CancelPionPlacement(ulong idPlayer, Socket? playerSocket, string idRoom)
        {
            // Si la demande ne trouve pas de partie ou qu'elle ne provient pas d'un joueur à qui c'est le tour : permission error
            Tools.Errors errors = Tools.Errors.Permission;

            // Parcours des threads de jeu pour trouver celui qui gère la partie cherchée

            foreach (Thread_serveur_jeu thread_serv_ite in _lst_serveur_jeu)
            {
                if (idRoom != thread_serv_ite.Get_ID().ToString()) continue;
                if (idPlayer == thread_serv_ite.Get_ActualPlayerId())
                {
                    // Vérification qu'un pion a bien été placée auparavant
                    if (thread_serv_ite.Get_posPionTourActu().Length != 0)
                    {
                        // Retrait de la position du pion
                        thread_serv_ite.RetirerPionTourActu();

                        // Envoie l'information display
                        SendBroadcast(idRoom, Tools.IdMessage.CancelPionPlacement, idPlayer);

                        errors = Tools.Errors.None;
                        break;
                    }

                    // Dans le cas où aucun pion n'a été placée auparavant ou qu'un pion est toujours placé,
                    // on renvoie une erreur Permission                 
                }

            }

            return errors;
        }

        public Tools.Errors Com_EndTurn(ulong idPlayer, string idRoom)
        {
            // Si la demande ne trouve pas de partie ou qu'elle ne provient pas d'un joueur à qui c'est le tour : permission error
            Tools.Errors errors = Tools.Errors.Permission;

            foreach (Thread_serveur_jeu thread_serv_ite in _lst_serveur_jeu)
            {
                if (idRoom != thread_serv_ite.Get_ID().ToString()) continue;
                if (idPlayer == thread_serv_ite.Get_ActualPlayerId())
                {
                    // Vérifie qu'il a au moins placé une tuile validée
                    if(thread_serv_ite.Get_posTuileTourActu().IsExisting())
                    {
                        // Fin du tour actuel
                        Socket? nextPlayerSocket = thread_serv_ite.EndTurn(idPlayer);
                        // Mise à jour du status de la game
                        Tools.GameStatus statusGame = thread_serv_ite.UpdateGameStatus();
                        if(statusGame == Tools.GameStatus.Stopped) // Si la partie est terminée
                        {
                            ulong idPlayerWinner = thread_serv_ite.GetWinner();
                            string[] dataToSend = new string[] { idPlayerWinner.ToString() };
                            SendBroadcast(idRoom, Tools.IdMessage.EndGame, dataToSend);
                            DeleteGame(idRoom);
                        }
                        else // Si la partie n'est pas terminée
                        {
                            ulong idPlayerActu = thread_serv_ite.Get_ActualPlayerId();
                            // Envoie des 3 tuiles au suivant
                            thread_serv_ite.ShuffleTilesGame();
                            SendBroadcast(idRoom, Tools.IdMessage.TuileDraw, idPlayerActu, thread_serv_ite.GetThreeLastTiles());
                        }

                        return Tools.Errors.None;
                    }

                    // S'il n'a pas posé de tuile : erreur Permission

                    
                }

            }

            return errors;
        }

        public void PlayerCheated(ulong idPlayer, Socket? playerSocket, string idRoom)
        {
            Packet packet = new Packet();


            // Recherche de la partie
            foreach (Thread_serveur_jeu threadJeu in _lst_serveur_jeu)
            {
                if(threadJeu.Get_ID() == Int32.Parse(idRoom))
                {
                    // Indique au serveur la triche
                    Tools.PlayerStatus playerStatus = threadJeu.SetPlayerStatus(idPlayer);


                    if (playerStatus == Tools.PlayerStatus.Kicked) // Deuxième triche -> kick
                    {
                        packet.IdMessage = Tools.IdMessage.PlayerKick;
                    }
                    else // Première triche -> avertissement
                    {
                        packet.IdMessage = Tools.IdMessage.PlayerCheat;
                    }
                    break;
                }
            }

            ClientAsync.Send(playerSocket, packet);

            // Lancement de l'écoute des réponse du client async
            ClientAsync.OnPacketReceived += OnPacketReceived;
            ClientAsync.Receive(playerSocket);

        }

        public Tools.PlayerStatus PlayerLeave(ulong idPlayer, string idRoom)
        {
            // Recherche de la partie
            foreach (Thread_serveur_jeu threadJeu in _lst_serveur_jeu)
            {
                if (threadJeu.Get_ID() == Int32.Parse(idRoom))
                {
                    // Retrait du joueur de la partie
                    Tools.PlayerStatus playerStatus = threadJeu.RemoveJoueur(idPlayer);
                    return playerStatus;

                    // Si le joueur quitte durant son tour
                    if(threadJeu.Get_ActualPlayerId() == idPlayer)
                    {
                        // On abandonne les informations du tour actuel
                        Socket? nextPlayerSock = threadJeu.CancelTurn(idRoom);
                        // On lui envoit les 3 tuiles
                        SendBroadcast(idRoom, Tools.IdMessage.TuileDraw, threadJeu.GetThreeLastTiles());
                    }

                    // Vérification du status de la partie (si le dernier joueur quitte -> fin de partie)
                    if(threadJeu.Get_Status() == Tools.GameStatus.Stopped)
                    {
                        DeleteGame(idRoom);
                    }

                    break;
                }
            }

            return Tools.PlayerStatus.NotFound;
        }

        // ===============================
        // Méthode réseau de réception
        // ===============================

        public void OnPacketReceived(object sender, Packet packet, Socket? socket)
        {
            if(packet.Error != Tools.Errors.None) // Gestion des erreurs
            {
                switch (packet.IdMessage)
                {
                    // Cas où aucune des 3 tuiles n'est posable
                    case Tools.IdMessage.TuileDraw:
                        // Récupère à part les 3 idTuiles dont il est question
                        string[] tuilesEnvoyees = new string[3];
                        try
                        {
                            Array.Copy(packet.Data, 1, tuilesEnvoyees, 0, 3);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("ERROR: Parsing the array representing the 3 tiles sended : " + ex);
                        }
                        
                        DrawAntiCheatPlayer(packet.Data[0], packet.IdPlayer, socket, tuilesEnvoyees);
                        break;

                    // Réponse d'un autre joueur (anti cheat) -> pas posable
                    case Tools.IdMessage.TuileVerification:
                        DrawAntiCheatVerif(packet.Data[0], false, (ulong)0, new Position(-1,-1,-1));
                        break;

                    case Tools.IdMessage.StartGame:
                        // TODO : Check s'il renvoit des erreurs
                        break;
                }
            }
            else // Gestion des réponses saines
            {
                switch (packet.IdMessage)
                {
                    // Joueur choisi une des tuiles posables
                    case Tools.IdMessage.TuileDraw:
                        Position exemplePosValid = new Position(Int32.Parse(packet.Data[2]), Int32.Parse(packet.Data[3]), Int32.Parse(packet.Data[4]));
                        ChooseIdTile(packet.Data[0], packet.IdPlayer, UInt64.Parse(packet.Data[1]), exemplePosValid, socket);
                        break;


                    // Réponse d'un autre joueur (anti cheat) -> posable
                    case Tools.IdMessage.TuileVerification:
                        Position posValid = new Position(Int32.Parse(packet.Data[2]), Int32.Parse(packet.Data[3]), Int32.Parse(packet.Data[4]));
                        DrawAntiCheatVerif(packet.Data[0], true, UInt64.Parse(packet.Data[1]), posValid);
                        break;

                }
            }

     
        }

        // ===============================
        // Fonction principale (threadée)
        // ===============================

        public void Lancement_thread_com()
        {

            // Informations du thread

            Console.WriteLine(string.Format("[{0}] Je suis un thread !", _id_thread_com));
            Console.WriteLine(string.Format("[{0}] J'officie sur le port numéro {1} !", _id_thread_com, _numero_localPort));
            Console.WriteLine(string.Format("[{0}] Je gère actuellement {1} parties!", _id_thread_com, _nb_parties_gerees));
            foreach (int id_ite in _id_parties_gerees)
            {
                Console.WriteLine(string.Format("[{0}] Je gère la partie d'ID {1}", _id_thread_com, id_ite));
            }

            //Debug.Log(string.Format("Compteur d'id de strings : {0}", _compteur_id_thread_com));


            
            // Lancement du serveur d'écoute du thread de com
            Server.Server.StartListening(_id_thread_com + 1);


            


            /*
            TextAsset contents = Resources.Load<TextAsset>("network/config");
            Parameters parameters = JsonConvert.DeserializeObject<Parameters>(contents.ToString());
            parameters.ServerPort = Convert.ToInt32(packet.Data[0]);
            _mon_id = packet.IdPlayer;

            ClientAsync.Connection(socket, parameters);
            ClientAsync.connectDone.WaitOne();

            ClientAsync.OnPacketReceived += OnPacketReceived;
            ClientAsync.Receive(socket);
            
            */

        }
    }
}
