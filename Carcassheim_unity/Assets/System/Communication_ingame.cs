using System;
using System.Net.Sockets;
using UnityEngine;
using ClassLibrary;
using System.Collections.Generic;
using Assert.system;

namespace Assets.system
{
    public class Communication_inGame : CarcasheimBack
    {

        // Paramètres de la partie 
        private readonly ulong _id_partie;

        private ulong _mon_id;

        private int _mode; // 0 -> Classique | 1 -> Time-attack | 2 -> Score

        private int _nb_tuiles;
        private int _score_max;

        private int _nb_joueur_max;
        private int _timer; // En secondes
        private int _timer_max_joueur; // En secondes
        private int _meeples; // Nombre de meeples par joueur

        private Dictionary<ulong, Tuile> dico_tuile;
        private Plateau lePlateau;

        private Player[] playerList;
        private ulong nextPlayer;
        private Position[] allposition;

        // DISPLAY SYSTEM
        [SerializeField] DisplaySystem system_display = null;
        //====================================================================================================
        // COMM AFFICHAGE
        // => Display; indique que la fonction est utilisé pour communiquer avec l'affichage
        // => Serveur; indique qu'il faut communiqyer avec le serveur
        // UNE FOIS QUE LES DONNES DE DEBUT DE PARTIE SONT DISPONNIBLE :
        //      APPELER gameBegin()
        // UNE FOIS QUE getTile EST PRET 
        //      APPELER system_display.setNextState(
        //  SI CONSTRUIRE RIVIERE :  turnStart
        //  SINON SI SCORE CHANGE : scoreChange
        //  SINON SI FIN DE PARTIE : endOfGame
        //      )
        // UNE FOIS QUE LE Score A ETE AFFICHE
        //      APPELER system_display.setNextState(
        //  SINON SI NOUVEAU TOUR : turnStart
        //  SINON SI FIN DE PARTIE : endOfGame
        //      )
        // !!! le cas de l'explorateur n'est pas encore fait (après getTile)

        //=======================================================
        // ATTRIBUTES
        private List<TileInitParam> tiles_drawed;

        //=======================================================
        // END TURN
        override public void sendTile(int tile_id, PositionRepre tile_pos, int id_meeple, int slot_pos)
        {
            //TODO ENVOYER !!AU SERVEUR!! LE COUP validé PAR LE JOUEUR ELU => Serveur
            SendPosition((ulong)tile_id, tile_pos.X, tile_pos.Y, tile_pos.Rotation,  id_meeple,  slot_pos);
        }

        override public void getTile(out int tile_id, out PositionRepre pos, out int id_meeple, out int slot_pos)
        {
            //TODO PARTAGER LE COUP validé PAR LE JOUEUR ELU => Display
            tile_id = -1;
            pos = null;
            id_meeple = -1;
            slot_pos = -1;
        }

        //=======================================================
        // START TURN
        override public int getNextPlayer()
        {
            //TODO PARTAGER L'ID DU JOUEUR ELU => Display
            return (int)nextPlayer;
        }

        override public int askTilesInit(List<TileInitParam> tiles)
        {
            //TODO PARTAGER LA LISTE DES TUILES TIRES => Display
            // Id tuile,
            // FLAG => true : garder la tuile; false: jeter la tuile
            // RETURN nombre de tuile dans la main au final
            tiles.AddRange(tiles_drawed);
            tiles_drawed.Clear();
            return 1;
            // (| f) (| f) (| f) (| f) >>(| t)
        }
        
        override public void askMeeplesInit(List<MeepleInitParam> meeples)
        {
            //TODO PARTAGER LA LISTE DES ID DES MEEPLES POSABLES ET LE NOMBRE DISPÖNIBLE => Display
            // Id meeple, nb de meeple d'id Id disponible
        }

        override public void getTilePossibilities(int tile_id, List<PositionRepre> positions)
        {
            //TODO PARTAGER LA LISTE DES POSITIONS OU LA TUILE D'ID tile_id PEUT ETRE POSE => Display
            // tile_id est un argument !!!
            // ajouter avec new Position,(X, Y, Rotation) avec rotation = (0: Nord, 1: Est, 2: Sud, 3: Ouest)
            // mettre une position par rotation
            int i, taille = allposition.Length;
            for (i = 0; i < taille; i++)
            {
                positions.Add(new PositionRepre(
                    allposition[i].X,
                    allposition[i].Y,
                    allposition[i].ROT
                    ));
            }
        }
        
        //=======================================================
        // SCORE
        override public void askScores(List<PlayerScore> players_scores)
        {
            //TODO PARTAGER LES NOUVEAUX SCORES POUR CHAQUE JOUEUR => Display
            // Id du joueur dont le score change, Nouveau score, Zone provoquant le changement de score
            // Zone: array/list de (id tuile, position de la tuile sur le plateau, id du slot)
            int taille = playerList.Length;
            for (int j = 0; j < taille; j++)
            {
                players_scores.Add(new PlayerScore(
                    (int)playerList[j].id,
                    (int)playerList[j].score,
                    Array.Empty<Zone>())) ;//JUSTIN si tu ne n'as pas fait, je te tuerais
            }
        }
        
        //=======================================================
        // GAME BEGIN
        override public void askPlayersInit(List<PlayerInitParam> players)
        {
            //TODO PARTAGER ID JOUEUR, NOM JOUEUR, NB DE MEEPLE => Display
            int taille = playerList.Length;
            for (int j = 0; j < taille; j++)
            {
                players.Add(new PlayerInitParam(
                    (int)playerList[j].id,
                    (int)playerList[j].nbMeeples,
                    playerList[j].name
                    ));
            }
        }
        override public int askIdTileInitial()
        {
            // TODO PARTAGER ID DE LA TUILE INITIAL EN POSITION (0, 0) => Display
            return 0;
        }

        override public void askTimerTour(out int min, out int sec)
        {
            // TODO PARTAGER LE TEMPS DISPONIBLE PAR TOUR => Display
            min = 0;
            sec = 0;
        }
        
        override public void askWinCondition(ref WinCondition win_cond, List<int> parameters)
        {
            // TODO PARTAGER CONDITION DE VICTOIRE => Display
            // TUILE => nb de tuile
            // SCORE => score à atteindre
            // TEMPS => nb de min, puis nb de sec
            win_cond = WinCondition.WinByTile;
            parameters.Add(100);
        }
            
        override public int getMyPlayer()
        {
            // TODO PARTAGER ID DU JOUEUR SUR CE CLIENT => Display
            return (int) _mon_id;
        }

        
        //=======================================================
        // PLEASE NEVER CALL ME

        override public void askPlayerOrder(List<int> player_ids)
        {
            // TODO PARTAGER LES IDS DES JOUEURS DANS L'ORDRE DU TOUR => Display
        }

        //=======================================================
        // ACTION IN GAME
        override public void sendAction(DisplaySystemAction action)
        {
            // TODO ENVOYER AU !!SERVEUR L'ACTION: PARTAGE DIRECT => Serveur
            switch (action.state)
            {
                case DisplaySystemActionTypes.tileSetCoord:
                    DisplaySystemActionTileSetCoord action_tsc = (DisplaySystemActionTileSetCoord) action;
                    break;
                case tileSelection:
                    DisplaySystemActionTileSelection action_ts = (DisplaySystemActionTileSelection) action;
                    break;
                case meepleSetCoord:
                    DisplaySystemActionMeepleSetCoord action_msc = (DisplaySystemActionMeepleSetCoord) action;
                    break;
                case meepleSelection:
                    DisplaySystemActionMeepleSelection action_ms = (DisplaySystemActionMeepleSelection) action;
                    break;
                case StateSelection:
                    DisplaySystemActionStateSelection action_ss = (DisplaySystemActionStateSelection) action;
                    break;
            }
            return;
        }

        //====================================================================================================


        private void CheckErrorSocketConnect(Tools.Errors error_value)
        {
            switch (error_value)
            {
                case Tools.Errors.None:
                    break;
                case Tools.Errors.ConfigFile:
                    // TODO : handle case : config file is bad or issue while extracting the data
                    Debug.Log(string.Format("Errors.ConfigFile"));
                    break;
                case Tools.Errors.Socket:
                    // TODO : handle case : connection could not be established
                    Debug.Log(string.Format("Errors.Socket"));
                    break;
                case Tools.Errors.ToBeDetermined:
                    break;
                case Tools.Errors.Unknown:
                    break;
                case Tools.Errors.Format:
                    break;
                case Tools.Errors.Receive:
                    break;
                case Tools.Errors.Data:
                    break;
                case Tools.Errors.Permission:
                    break;
                default:
                    // TODO : handle case : default
                    Debug.Log(string.Format("Errors.Unknown"));
                    break;
            }
        }


        // Start is called before the first frame update
        void Start()
        {
            // ======================
            // ==== DANS LA GAME ====
            // ======================
            lePlateau = new Plateau();
            LireXml reader = new LireXml("config_back.xml");
            dico_tuile = reader.ReadXml();

            ClientAsync.OnPacketReceived += OnPacketReceived;
            ClientAsync.Receive();

            /*
            Action listening = () =>
            {
                ClientAsync.StartClient(socket);
            };
            Task.Run(listening);
            */



            // Boucle d'écoute du serveur

            // Type de réceptions :
            //          - mise à jour affichage (coup d'un autre joueur, même non validé)
            //          - réception d'un WarningCheat
            //          - indication de la part du serveur que c'est au client de jouer
            //                  -> début de la phase d'interactions entres placements du joueur et le serveur
            //                  -> se base sur des fonctions d'attente personnalisées, où le script attend que le joueur place ses tuiles/pions


            // Description de la phase d'interaction  :
            //      - Réception des 3 tuiles 
            //      - Vérification qu'une des 3 est posable, si ce n'est pas le cas on prévient le serveur et on demande d'autres tuiles
            //      - Affichage de la tuile ainsi choisie par le client (la première à être posable)
            //          -> MaJ graphique (tuile choisie)
            //      - (Attente d'une action du joueur : pose d'une tuile)
            //      - Envoie la pose de tuile au serveur et observe sa réponse (si le coup est illégal par exemple)
            //          -> MaJ graphique (tuile posée)
            //      - (Attente d'une action du joueur : pose d'un pion)
            //      - Envoie la pose de pion au serveur et observe sa réponse (si le coup est illégal par exemple)
            //          -> MaJ graphique (pion posé)
            //      - (Attente d'une action du joueur : validation de son tour)
            //      - Envoie la validation du tour au serveur et observe sa réponse (si le coup est illégal par exemple)
            //          -> MaJ graphique (tour terminé)



        }

        // Update is called once per frame
        public void Update()
        {
        }

        public void Disconnection(Socket socket)
        {
            ClientAsync.StopListening(socket);
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();

            //TODO reload l'ancien socket
        }

        public void OnPacketReceived(object sender, Packet packet)
        {

            if (packet.IdMessage == Tools.IdMessage.TuileDraw)
            {
                OnTuileReceived(packet);
            }
            else if(packet.IdMessage == Tools.IdMessage.PlayerList)
            {
                OnPlayerListReceive(packet);
            }
            else if (packet.IdMessage == Tools.IdMessage.PlayerNext)
            {
                OnPlayerNextReceive(packet);
            }
        }

        public void OnTuileReceived(Packet packet)
        {
            ulong id_tuile;
            Tuile tuile;
            int i;
            Position[] position;
            bool tuile_ok = false;
            TileInitParam tileParam;

            for (i = 0; i < 3; i++)
            {
                id_tuile = Convert.ToUInt64(packet.Data[i]);
                tuile = dico_tuile[id_tuile];
                position = lePlateau.PositionsPlacementPossible(tuile);

                tileParam = new TileInitParam
                {
                    tile_flags = false,
                    id_tile = (int)id_tuile
                };

                if (position != null)
                {
                    if (position.Length >= 0)
                    {
                        Debug.Log("Les positions de la " + i + "ème tuile : " + position);
                        SendAllPosition(position, id_tuile);
                        tileParam.tile_flags = true;
                        allposition = position;
                    }
                }

                tiles_drawed.Add(tileParam);
                if (tuile_ok)
                    return;
            }

            
            ClientAsync.Send( packet);
        }

        private void SendAllPosition(Position[] position,ulong id_tuile)
        {
            Packet packet = new Packet();
            packet.IdMessage = Tools.IdMessage.RoomSettingsSet;
            packet.IdPlayer = _mon_id;

            int i, taille = position.Length;
            int taille_data = 2 + taille*3;
            packet.Data = new string[taille_data];

            packet.Data[0] = _id_partie.ToString();
            packet.Data[1] = id_tuile.ToString();

            for (i= 2; i < taille_data - 2; i+=3)
            {
                packet.Data[i] = position[taille_data / 3].X.ToString();
                packet.Data[i+1] = position[(taille_data / 3)].Y.ToString();
                packet.Data[i+2] = position[(taille_data / 3)].ROT.ToString();

            }

            ClientAsync.Send(packet);
        }

        public void SendPosition(ulong id_tuile, int X, int Y, int ROT, int id_meeple, int slot_pos)
        {
            Packet packet = new Packet();
            packet.IdMessage = Tools.IdMessage.RoomSettingsSet;
            packet.IdPlayer = _mon_id;

            packet.Data = new string[7];
            
            packet.Data[0] = _id_partie.ToString();
            packet.Data[1] = id_tuile.ToString();
            packet.Data[2] = X.ToString();
            packet.Data[3] = Y.ToString();
            packet.Data[4] = ROT.ToString();
            packet.Data[5] = id_meeple.ToString();
            packet.Data[6] = slot_pos.ToString();

            ClientAsync.Send(packet);
        }

        public void OnPlayerListReceive(Packet packet)
        {
            int i,compteur = 0, taille_data = packet.Data.Length;
            playerList = new Player[taille_data/3];

            for(i = 0; i < taille_data; i+=3)
            {
                playerList[compteur] = new Player(Convert.ToUInt64(packet.Data[i]), packet.Data[i + 1], Convert.ToUInt32(packet.Data[i + 2]), Convert.ToUInt32(packet.Data[i + 3]));
            }
        }

        public void OnPlayerNextReceive(Packet packet)
        {
            nextPlayer = Convert.ToUInt64(packet.Data[0]);
        }
    }
}