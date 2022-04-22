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
        private int _numero_port;
        private int _nb_parties_gerees;

        private List<int> _id_parties_gerees;
        private List<Thread_serveur_jeu> _lst_serveur_jeu;

        private static int _compteur_id_thread_com;

        // Locks

        private readonly object _lock_nb_parties_gerees;
        private readonly object _lock_id_parties_gerees;

        // =============
        // Constructeur
        // =============

        public Thread_communication(int num_port, int id)
        {
            _numero_port = num_port;
            _nb_parties_gerees = 0;
            _id_parties_gerees = new List<int>();
            _id_thread_com = id;
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

        public int Get_port()
        {
            return _numero_port;
        }

        public List<Thread_serveur_jeu> Get_list_server_thread()
        {
            return this._lst_serveur_jeu;
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
                Thread_serveur_jeu thread_serveur_jeu = new Thread_serveur_jeu(id_nouv_partie, playerId, playerSocket);
                Thread nouv_thread = new Thread(new ThreadStart(thread_serveur_jeu.Lancement_thread_serveur_jeu));

                _lst_serveur_jeu.Add(thread_serveur_jeu);

                nouv_thread.Start();

                return id_nouv_partie;

            }
            else // La partie n'a pas pu être créée
            {
                return id_nouv_partie;
            }


        }

        public void TransmitStartToAll(int roomId)
        {
            Packet packet = new Packet();
            packet.IdMessage = Tools.IdMessage.RoomStart;
            packet.Type = true;

            foreach (Thread_serveur_jeu thread_serv_ite in Get_list_server_thread())
            {
                if(thread_serv_ite.Get_ID() == roomId)
                {
                    foreach(var joueur in thread_serv_ite.Get_Dico_Joueurs())
                    {
                        ClientAsync.Send(joueur.Value._socket_of_player, packet);

                        // Lancement de l'écoute des réponse du client async
                        ClientAsync.OnPacketReceived += OnPacketReceived;
                        ClientAsync.Receive(joueur.Value._socket_of_player);

                        break;
                    }
                }
            }
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

        public void SendTilesRoundStart(string[] tilesToSend, Socket? socket, string idRoom)
        {
            Packet packet = new Packet();
            packet.IdMessage = Tools.IdMessage.TuileDraw;
            packet.Type = true;

            packet.Data = tilesToSend;

            ClientAsync.Send(socket, packet);

            // Lancement de l'écoute de la réponse du joueur modérateur
            ClientAsync.OnPacketReceived += OnPacketReceived;
            ClientAsync.Receive(socket);

            
            // Envoi des tuiles à tous les autres joueurs
            foreach(Thread_serveur_jeu threadJeu in _lst_serveur_jeu)
            {
                if(threadJeu.Get_ID() == Int32.Parse(idRoom))
                {

                    foreach(var joueur in threadJeu.Get_Dico_Joueurs())
                    {
                        ClientAsync.Send(joueur.Value._socket_of_player, packet);

                        // Lancement de l'écoute de la réponse du joueur modérateur
                        ClientAsync.OnPacketReceived += OnPacketReceived;
                        ClientAsync.Receive(socket);
                    }

                    break;
                }
            }

        }

        // La fonction du joueur dont c'est le tour
        public void DrawAntiCheatPlayer(string idRoom, ulong idPlayer, Socket? playerSocket)
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
                    }
                    else
                    {
                        // En effet aucune tuile n'est valide, nous renvoyons trois nouvelles tuiles
                        SendTilesRoundStart(threadJeu.GenerateThreeTiles(), playerSocket, idRoom);
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
                    else
                    {
                        // Vérification du placement
                        errors = thread_serv_ite.TilePlacement(idPlayer, UInt32.Parse(idTuile), Int32.Parse(posX), Int32.Parse(posY), Int32.Parse(rotat));
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
                        // Vérification du placement
                        errors = thread_serv_ite.PionPlacement(idPlayer, UInt32.Parse(idTuile), Int32.Parse(idMeeple), Int32.Parse(slotPos));
                        break;
                    }

                    // Dans le cas où aucune tuile n'a pas été placée auparavant ou qu'un pion est déjà placé,
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
                        errors = Tools.Errors.None;
                        break;
                    }

                    // Dans le cas où aucune tuile n'a été placée auparavant ou qu'un pion est toujours placé,
                    // on renvoie une erreur Permission                 
                }

            }

            return errors; 
        }

        public void PlayerCheated(ulong idPlayer, Socket? playerSocket, string idRoom)
        {
            Packet packet = new Packet();
            packet.Type = true;


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
                        DrawAntiCheatPlayer(packet.Data[0], packet.IdPlayer, socket);
                        break;

                    // Réponse d'un autre joueur (anti cheat) -> pas posable
                    case Tools.IdMessage.TuileVerification:
                        DrawAntiCheatVerif(packet.Data[0], false, (ulong)0, new Position(-1,-1,-1));
                        break;

                    case Tools.IdMessage.RoomStart:
                        // TODO : Check s'il renvoit des erreurs
                        break;
                }
            }
            else // Gestion des réponses saines
            {
                switch (packet.IdMessage)
                {
                    // Réponse d'un autre joueur (anti cheat) -> posable
                    case Tools.IdMessage.TuileVerification:
                        Position posValid = new Position(Int32.Parse(packet.Data[2]), Int32.Parse(packet.Data[3]), Int32.Parse(packet.Data[4]));
                        DrawAntiCheatVerif(packet.Data[0], true, UInt32.Parse(packet.Data[1]), posValid);
                        break;

                }
            }

     
        }

        // ===============================
        // Fonction principale (threadée)
        // ===============================

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

            //Server.Server.StartListening();

            
            // Lancement du serveur d'écoute du thread de com
            Server.Server.StartListening(_numero_port);


            


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
