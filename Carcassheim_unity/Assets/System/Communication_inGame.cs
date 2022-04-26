using System;
using System.Net.Sockets;
using UnityEngine;
using ClassLibrary;
using System.Collections.Generic;
using Assert.system;
using Assets.System;
using System.Threading;

namespace Assets.system
{

    public class Communication_inGame : CarcasheimBack
    {

        // Param�tres de la partie 
        private ulong _id_partie;

        private ulong _mon_id;


        private int id_tile_init;
        bool id_tile_init_received = false;

        private int _mode; // 0 -> Classique | 1 -> Time-attack | 2 -> Score
        private int _nb_tuiles = -1;
        private int _score_max = -1;
        private int _timer_max_joueur = -1; // En secondes
        bool win_cond_received = false;

        private int _nb_joueur_max;
        private int _timer; // En secondes
        bool timer_tour_received = false;
        private int _meeples; // Nombre de meeples par joueur

        private Dictionary<ulong, Tuile> dico_tuile;
        private Plateau lePlateau;

        private Player[] playerList;
        bool player_received = false;
        private ulong nextPlayer;
        private Position[] allposition;

        // DISPLAY SYSTEM
        [SerializeField] DisplaySystem system_display = null;
        //====================================================================================================
        // COMM AFFICHAGE
        // => Display; indique que la fonction est utilis� pour communiquer avec l'affichage
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
        // !!! le cas de l'explorateur n'est pas encore fait (apr�s getTile)

        //=======================================================
        // ATTRIBUTES
        private List<TileInitParam> tiles_drawed;

        //=======================================================
        // END TURN
        override public void sendTile(TurnPlayParam param)
        {
            //TODO ENVOYER !!AU SERVEUR!! LE COUP valid� PAR LE JOUEUR ELU => Serveur
            int tile_id = param.id_tile;
            PositionRepre tile_pos = param.tile_pos;
            int id_meeple = param.id_meeple;
            int slot_pos = param.slot_pos;
            
            SendPosition((ulong)tile_id, tile_pos.X, tile_pos.Y, tile_pos.Rotation, id_meeple, slot_pos);
        }

        override public void getTile(out TurnPlayParam param)
        {
            //TODO PARTAGER LE COUP valid� PAR LE JOUEUR ELU => Display
            param = new TurnPlayParam(-1, null, -1, -1);
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
            //TODO PARTAGER LA LISTE DES ID DES MEEPLES POSABLES ET LE NOMBRE DISP�NIBLE => Display
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
        override public void askScores(List<PlayerScoreParam> players_scores)
        {
            //TODO PARTAGER LES NOUVEAUX SCORES POUR CHAQUE JOUEUR => Display
            // Id du joueur dont le score change, Nouveau score, Zone provoquant le changement de score
            // Zone: array/list de (id tuile, position de la tuile sur le plateau, id du slot)
            int taille = playerList.Length;
            for (int j = 0; j < taille; j++)
            {
                players_scores.Add(new PlayerScoreParam(
                    (int)playerList[j].id,
                    (int)playerList[j].score,
                    Array.Empty<Zone>()));//JUSTIN si tu ne n'as pas fait, je te tuerais
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
            return id_tile_init;
        }

        override public void askTimerTour(out int min, out int sec)
        {
            // TODO PARTAGER LE TEMPS DISPONIBLE PAR TOUR => Display
            min = _timer / 60;
            sec = _timer % 60;
        }

        override public void askWinCondition(ref WinCondition win_cond, List<int> parameters)
        {
            // TODO PARTAGER CONDITION DE VICTOIRE => Display
            // 0 TUILE => nb de tuile
            // 1 TEMPS => nb de min, puis nb de sec
            // 2 SCORE => score � atteindre
            switch (_mode)
            {
                case 1:
                    win_cond = WinCondition.WinByTime;
                    parameters.Add(_timer_max_joueur / 60);
                    parameters.Add(_timer_max_joueur % 60);
                    break;
                case 2:
                    win_cond = WinCondition.WinByPoint;
                    parameters.Add(_score_max);
                    break;
                default:
                    win_cond = WinCondition.WinByTile;
                    parameters.Add(_nb_tuiles);
                    break;
            }
        }

        override public int getMyPlayer()
        {
            // TODO PARTAGER ID DU JOUEUR SUR CE CLIENT => Display
            return (int)_mon_id;
        }


        //=======================================================
        // PLEASE NEVER CALL ME

        override public void askPlayerOrder(List<int> player_ids)
        {
            // TODO PARTAGER LES IDS DES JOUEURS DANS L'ORDRE DU TOUR => Display
            int taille = playerList.Length;
            for (int j = 0; j < taille; j++)
            {
                player_ids.Add((int)playerList[j].id);
            }
        }

        //=======================================================
        // END GAME

        override public void askFinalScore(List<PlayerScoreParam> playerScores)
        {
            //TODO Pareil que askScore
            int taille = playerList.Length;
            for (int j = 0; j < taille; j++)
            {
                playerScores.Add(new PlayerScoreParam(
                    (int)playerList[j].id,
                    (int)playerList[j].score,
                    Array.Empty<Zone>()));//JUSTIN si tu ne n'as pas fait, je te tuerais
            }
        }

        //=======================================================
        // ACTION IN GAME
        override public void sendAction(DisplaySystemAction action)
        {
            /*
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
            */
            return;
        }

        //====================================================================================================


        // Start is called before the first frame update
        void Start()
        {
            // ======================
            // ==== DANS LA GAME ====
            // ======================
            lePlateau = new Plateau();
            dico_tuile = LireXML2.Read("config_back.xml");
            tiles_drawed = new List<TileInitParam>();

            Communication.Instance.StartListening(OnPacketReceived);

            _id_partie = (ulong)Communication.Instance.idRoom;
            _mon_id = Communication.Instance.idClient;

            _mode = (int)RoomInfo.Instance.mode; // 0 -> Classique | 1 -> Time-attack | 2 -> Score
            _nb_tuiles = -1;
            _score_max = RoomInfo.Instance.scoreMax;
            _timer_max_joueur = (int)RoomInfo.Instance.timerJoueur; // En secondes
            win_cond_received = true;

            _nb_joueur_max = RoomInfo.Instance.nbJoueurMax;
            _timer = (int)RoomInfo.Instance.timerPartie; // En secondes
            timer_tour_received = true;
            _meeples = RoomInfo.Instance.meeples; // Nombre de meeples par joueur

            Packet packet = new Packet();
            packet.IdPlayer = Communication.Instance.idClient;
            packet.Data = new string[] { Communication.Instance.idRoom.ToString() };
            packet.IdMessage = Tools.IdMessage.PlayerList;
            
            Communication.Instance.SendAsync(packet);
            Thread.Sleep(500);
            packet.IdMessage = Tools.IdMessage.PlayerCurrent;
            Communication.Instance.SendAsync(packet);
            Debug.Log("On est dans la game");

            /*
            Action listening = () =>
            {
                ClientAsync.StartClient(socket);
            };
            Task.Run(listening);
            */



            // Boucle d'�coute du serveur

            // Type de r�ceptions :
            //          - mise � jour affichage (coup d'un autre joueur, m�me non valid�)
            //          - r�ception d'un WarningCheat
            //          - indication de la part du serveur que c'est au client de jouer
            //                  -> d�but de la phase d'interactions entres placements du joueur et le serveur
            //                  -> se base sur des fonctions d'attente personnalis�es, o� le script attend que le joueur place ses tuiles/pions


            // Description de la phase d'interaction  :
            //      - R�ception des 3 tuiles 
            //      - V�rification qu'une des 3 est posable, si ce n'est pas le cas on pr�vient le serveur et on demande d'autres tuiles
            //      - Affichage de la tuile ainsi choisie par le client (la premi�re � �tre posable)
            //          -> MaJ graphique (tuile choisie)
            //      - (Attente d'une action du joueur : pose d'une tuile)
            //      - Envoie la pose de tuile au serveur et observe sa r�ponse (si le coup est ill�gal par exemple)
            //          -> MaJ graphique (tuile pos�e)
            //      - (Attente d'une action du joueur : pose d'un pion)
            //      - Envoie la pose de pion au serveur et observe sa r�ponse (si le coup est ill�gal par exemple)
            //          -> MaJ graphique (pion pos�)
            //      - (Attente d'une action du joueur : validation de son tour)
            //      - Envoie la validation du tour au serveur et observe sa r�ponse (si le coup est ill�gal par exemple)
            //          -> MaJ graphique (tour termin�)



        }

        // Update is called once per frame
        public void Update()
        {
        }

        public void Disconnection(Socket socket)
        {
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
            else if (packet.IdMessage == Tools.IdMessage.PlayerList)
            {
                OnPlayerListReceive(packet);
            }
            else if (packet.IdMessage == Tools.IdMessage.PlayerCurrent)
            {
                OnPlayerCurrentReceive(packet);
            }
            else if(packet.IdMessage == Tools.IdMessage.TuilePlacement)
            {
                id_tile_init = Convert.ToInt32(packet.Data[0]);
                id_tile_init_received = true;

                Debug.Log("packet.Data[1]) = " + packet.Data[1] + " (packet.Data[2]) = " + packet.Data[2] + "(packet.Data[3])) = " + packet.Data[3]);
                lePlateau.PoserTuile(dico_tuile[(ulong)id_tile_init],
                    Convert.ToInt32(packet.Data[1]),
                    Convert.ToInt32(packet.Data[2]),
                    Convert.ToInt32(packet.Data[3]));
                /* TODO placement */
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
                        Debug.Log("Les positions de la " + i + "�me tuile : " + position.ToString());
                        SendAllPosition(position, id_tuile);
                        tileParam.tile_flags = true;
                        allposition = position;
                    }
                }

                tiles_drawed.Add(tileParam);
                if (tuile_ok)
                    return;
            }

            packet.Error = Tools.Errors.Data;
            Communication.Instance.SendAsync(packet);
        }

        private void SendAllPosition(Position[] position, ulong id_tuile)
        {
            Packet packet = new Packet();
            packet.IdMessage = Tools.IdMessage.TuileVerification;
            packet.IdPlayer = _mon_id;

            int i, taille = position.Length;
            int taille_data = 2 + taille * 3;
            packet.Data = new string[taille_data];

            packet.Data[0] = _id_partie.ToString();
            packet.Data[1] = id_tuile.ToString();

            for (i = 2; i < taille_data - 2; i += 3)
            {
                packet.Data[i] = position[taille_data / 3].X.ToString();
                packet.Data[i + 1] = position[(taille_data / 3)].Y.ToString();
                packet.Data[i + 2] = position[(taille_data / 3)].ROT.ToString();

            }

            Communication.Instance.SendAsync(packet);
        }

        public void SendPosition(ulong id_tuile, int X, int Y, int ROT, int id_meeple, int slot_pos)
        {//ToDo Retirer Meeple et slot et le changer
            Packet packet = new Packet();
            packet.IdMessage = Tools.IdMessage.TuilePlacement;
            packet.IdPlayer = _mon_id;

            packet.Data = new string[7];

            packet.Data[0] = _id_partie.ToString();
            packet.Data[1] = id_tuile.ToString();
            packet.Data[2] = X.ToString();
            packet.Data[3] = Y.ToString();
            packet.Data[4] = ROT.ToString();
            //packet.Data[5] = id_meeple.ToString();
            //packet.Data[6] = slot_pos.ToString();

            Communication.Instance.SendAsync(packet);
        }

        public void OnPlayerListReceive(Packet packet)
        {
            Debug.Log("132156456156456156156126156451261594561549456");
            int i, compteur = 0, taille_data = packet.Data.Length;
            playerList = new Player[taille_data / 2];

            for (i = 0; i < taille_data; i += 2)
            {
                playerList[compteur] = new Player(ulong.Parse(packet.Data[i]), packet.Data[i + 1], (uint)_meeples, 0);
            }
            player_received = true;
            
            checkGameBegin();
        }

        public void OnPlayerCurrentReceive(Packet packet)
        {
            nextPlayer = packet.IdPlayer;
        }

        public void checkGameBegin()
        {
            if (player_received && win_cond_received && id_tile_init_received && timer_tour_received)
            {
                system_display.gameBegin();
                Debug.Log("Game commence");
            }
        }
    }
}