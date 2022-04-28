using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum DisplaySystemState
{
        tilePosing,
        meeplePosing,
        turnStart,
        StateTransition,
        noState,
        idleState,
        endOfGame,
        scoreChange
};

public class DisplaySystem : MonoBehaviour
{
        public static LayerMask TableLayer { get; private set; }
        public static LayerMask BoardLayer { get; private set; }

        // * BACK ************************************************
        [SerializeField] private CarcasheimBack system_back;
        [SerializeField] private Table table;
        [SerializeField] private Banner banner;
        [SerializeField] private PlayerList player_list;
        [SerializeField] private ScoreBoard score_board;

        // * STATE ***********************************************

        private DisplaySystemState act_system_state = DisplaySystemState.noState;
        private DisplaySystemState prev_system_state = DisplaySystemState.noState;
        private Queue<DisplaySystemState> state_transition;


        // * BOARD ***********************************************

        [SerializeField] private PlateauRepre board;
        // Plane board_plane;
        public TuileRepre reference_tile;


    // * PLAYER GESTION ***************************************
    public event System.Action<PlayerRepre> OnPlayerDisconnected;
    private PauseMenu pauseMenu;

        // * TILE *************************************************
        [SerializeField] private TuileRepre tuile_model;
        private List<TuileRepre> tiles_hand;
        private Queue<TuileRepre> tiles_drawned;
        private Queue<bool> lifespan_tiles_drawned;

        public TuileRepre act_tile;

        // * MEEPLE ***********************************************

        [SerializeField] private MeepleRepre meeple_model;
        private List<MeepleRepre> meeples_hand;
        public MeepleRepre act_meeple;
        public Dictionary<int, int> meeple_distrib;

        // * PLAYERS **********************************************

        private Dictionary<int, PlayerRepre> players_mapping;
        private PlayerRepre my_player;
        public PlayerRepre act_player;

        [SerializeField] private List<Color> players_color;

        [SerializeField] private bool DEBUG = false;

        // * BUTTONS **********************************************

        private Button cancelButt, AcceptButt, LeftArrowButt, RightArrowButt;

        // Start is called before the first frame update

        void Awake()
        {
                Transform t_buttons = GameObject.Find("Boutons").transform;

                cancelButt = t_buttons.Find("Cancel").GetComponent<Button>();
                cancelButt.onClick.AddListener(cancel);


                AcceptButt = t_buttons.Find("Validate").GetComponent<Button>();
                AcceptButt.onClick.AddListener(accept);

                LeftArrowButt = t_buttons.Find("LeftArrow").GetComponent<Button>();
                LeftArrowButt.onClick.AddListener(left_possibility);

                RightArrowButt = t_buttons.Find("RightArrow").GetComponent<Button>();
                RightArrowButt.onClick.AddListener(right_possibility);

                TableLayer = LayerMask.NameToLayer("Table");
                BoardLayer = LayerMask.NameToLayer("Board");

                players_mapping = new Dictionary<int, PlayerRepre>();
                state_transition = new Queue<DisplaySystemState>();

                meeples_hand = new List<MeepleRepre>();
                meeple_distrib = new Dictionary<int, int>();
                tiles_hand = new List<TuileRepre>();
                tiles_drawned = new Queue<TuileRepre>();
                lifespan_tiles_drawned = new Queue<bool>();

                if (players_color.Count == 0)
                {
                        for (int i = 0; i < 10; i++)
                        {
                                players_color.Add(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
                        }
                }
                //! TEST
                //gameBegin();
                pauseMenu = new PauseMenu();
        }

        public void execAction(DisplaySystemAction action)
        {
                if (act_player != my_player)
                {
                        switch (action.action_type)
                        {
                                case DisplaySystemActionTypes.tileSetCoord:
                                        DisplaySystemActionTileSetCoord action_tsc = (DisplaySystemActionTileSetCoord)action;
                                        if (act_tile != null && act_tile.Id == action_tsc.tile_id)
                                        {
                                                board.setTileAt(action_tsc.new_pos, act_tile);
                                                table.tilePositionChanged(act_tile);
                                        }
                                        break;
                                case DisplaySystemActionTypes.tileSelection:
                                        DisplaySystemActionTileSelection action_ts = (DisplaySystemActionTileSelection)action;
                                        if (action_ts.index_in_hand < 0 || action_ts.index_in_hand >= tiles_hand.Count || tiles_hand[action_ts.index_in_hand].Id != action_ts.tile_id)
                                        {
                                                action_ts.index_in_hand = -1;
                                                for (int i = 0; i < tiles_hand.Count; i++)
                                                {
                                                        if (tiles_hand[action_ts.index_in_hand].Id == action_ts.index_in_hand) { action_ts.index_in_hand = i; break; }
                                                }
                                        }
                                        if (action_ts.index_in_hand != -1)
                                                setSelectedTile(action_ts.index_in_hand, true);
                                        break;
                                case DisplaySystemActionTypes.meepleSelection:
                                        DisplaySystemActionMeepleSelection action_ms = (DisplaySystemActionMeepleSelection)action;
                                        {
                                                if (action_ms.index_in_hand < 0 || action_ms.index_in_hand >= tiles_hand.Count || meeples_hand[action_ms.index_in_hand].Id != action_ms.meeple_id)
                                                {
                                                        action_ms.index_in_hand = -1;
                                                        for (int i = 0; i < meeples_hand.Count; i++)
                                                        {
                                                                if (meeples_hand[i].Id == action_ms.meeple_id) { action_ms.index_in_hand = i; break; }
                                                        }
                                                }
                                                if (action_ms.index_in_hand != -1)
                                                        setSelectedMeeple(action_ms.index_in_hand, true);
                                        }
                                        break;
                                case DisplaySystemActionTypes.meepleSetCoord:
                                        DisplaySystemActionMeepleSetCoord action_msc = (DisplaySystemActionMeepleSetCoord)action;
                                        if (board.getTileAt(action_msc.tile_pos) == act_tile && act_meeple != null && act_meeple.Id == action_msc.meeple_id)
                                        {
                                                bool meeple_pos_null = act_meeple.ParentTile == null;
                                                if (act_tile.setMeeplePos(act_meeple, action_msc.slot_pos))
                                                {
                                                        if (meeple_pos_null != (act_meeple.ParentTile == null))
                                                                table.meeplePositionChanged(act_meeple);
                                                }
                                        }
                                        break;
                                case DisplaySystemActionTypes.StateSelection:
                                        DisplaySystemActionStateSelection action_ss = (DisplaySystemActionStateSelection)action;
                                        setNextState(action_ss.new_state);
                                        break;
                        }
                }
        }


        // TODO : Attention à la touche entrer : elle permet aussi de faire l'action du dernier bouton qui a été appelé
        public void meeple_cancel()
        {
                if (act_meeple.SlotPos != -1 && act_tile.setMeeplePos(act_meeple, -1))
                        table.meeplePositionChanged(act_meeple);
                else
                {
                        setNextState(DisplaySystemState.tilePosing);
                        system_back.sendAction(new DisplaySystemActionStateSelection(DisplaySystemState.tilePosing));
                }
        }

        public void tile_cancel()
        {
                if (act_tile != null && act_tile.Pos != null)
                {
                        if (board.setTileAt(null, act_tile))
                        {
                                act_tile.setIndexFromPos();
                                table.tilePositionChanged(act_tile);
                        }
                        act_tile.Index = -1;
                        system_back.sendAction(new DisplaySystemActionTileSetCoord(act_tile.Id, act_tile.Pos));
                }
        }

        public void meeple_accept()
        {
                if (act_meeple.SlotPos != -1)
                {
                        setNextState(DisplaySystemState.idleState);
                        system_back.sendAction(new DisplaySystemActionStateSelection(DisplaySystemState.idleState));
                }
        }

        public void tile_accept()
        {
                if (act_tile != null && act_tile.Pos != null)
                {
                        if (meeples_hand.Count > 0)
                        {
                                setNextState(DisplaySystemState.meeplePosing);
                                system_back.sendAction(new DisplaySystemActionStateSelection(DisplaySystemState.meeplePosing));
                        }
                        else
                        {
                                setNextState(DisplaySystemState.idleState);
                                system_back.sendAction(new DisplaySystemActionStateSelection(DisplaySystemState.idleState));
                        }
                }
        }

        public void meeple_poss(bool right)
        {
                bool verif_meeple = false;
                if (right)
                {

                        if (act_meeple.SlotPos <= 0)
                                verif_meeple = act_tile.setMeeplePos(act_meeple, act_tile.slots_mapping.Count - 1); // On va à droite donc on prend la dernière position possible
                        else if (act_meeple.SlotPos >= act_tile.slots_mapping.Count)
                                verif_meeple = act_tile.setMeeplePos(act_meeple, act_tile.slots_mapping.Count - 1);
                        else
                                verif_meeple = act_tile.setMeeplePos(act_meeple, act_meeple.SlotPos - 1);
                }
                else
                {
                        if (act_meeple.SlotPos <= 0)
                                verif_meeple = act_tile.setMeeplePos(act_meeple, act_tile.slots_mapping.Count - 1); // On va à droite donc on prend la dernière position possible
                        else if (act_meeple.SlotPos >= act_tile.slots_mapping.Count)
                                verif_meeple = act_tile.setMeeplePos(act_meeple, act_tile.slots_mapping.Count - 1);
                        else
                                verif_meeple = act_tile.setMeeplePos(act_meeple, act_meeple.SlotPos - 1);
                }

                bool meeple_position_is_null = act_meeple.ParentTile == null;
                if (verif_meeple)
                {
                        if (meeple_position_is_null != (act_meeple.ParentTile == null))
                                table.meeplePositionChanged(act_meeple);
                }
                system_back.sendAction(new DisplaySystemActionMeepleSetCoord(act_tile.Id, act_tile.Pos, act_meeple.Id, act_meeple.SlotPos));
        }

        public void tile_poss(bool right)
        {
                int old_index = act_tile.Index;
                PositionRepre old_pos = act_tile.Pos, new_pos;
                bool have_same_pos = false;

                //Si les index ont les mêmes x et y, on cherche celui à partir duquel la position change
                do
                {
                        if (right)
                        {
                                act_tile.Index++;
                                if (act_tile.Index <= -1)
                                        act_tile.Index = 0; // On va à droite donc on prend la première position possible

                                else if (act_tile.Index >= act_tile.possibilitiesPosition.Count)
                                        act_tile.Index = 0;
                        }
                        else
                        {
                                act_tile.Index--;
                                if (act_tile.Index <= -1)
                                        act_tile.Index = act_tile.possibilitiesPosition.Count - 1; // On va à gauche donc on prend la dernière position possible

                                else if (act_tile.Index >= act_tile.possibilitiesPosition.Count)
                                        act_tile.Index = act_tile.possibilitiesPosition.Count - 1;
                        }

                        new_pos = act_tile.possibilitiesPosition[act_tile.Index];
                        have_same_pos = areSamePos(old_pos, new_pos);

                } while (have_same_pos && act_tile.Index != old_index);

                if (board.setTileAt(new_pos, act_tile))
                {
                        table.tilePositionChanged(act_tile);
                }
                system_back.sendAction(new DisplaySystemActionTileSetCoord(act_tile.Id, act_tile.Pos));
        }

        public void cancel()
        {
                if (act_player == my_player || DEBUG)
                {
                        switch (act_system_state)
                        {
                                case DisplaySystemState.meeplePosing:
                                        meeple_cancel();
                                        break;
                                case DisplaySystemState.tilePosing:
                                        tile_cancel();
                                        break;
                        }
                }
        }

        public void accept()
        {
                if (act_player == my_player || DEBUG)
                {
                        switch (act_system_state)
                        {
                                case DisplaySystemState.meeplePosing:
                                        meeple_accept();
                                        break;
                                case DisplaySystemState.tilePosing:
                                        tile_accept();
                                        break;
                        }
                }
        }

        public void left_possibility()
        {
                if (act_player == my_player || DEBUG)
                {
                        switch (act_system_state)
                        {
                                case DisplaySystemState.meeplePosing:
                                        meeple_poss(false); //false car on va à gauche et pas à droite
                                        break;
                                case DisplaySystemState.tilePosing:
                                        tile_poss(false);
                                        break;
                        }
                }
        }

        public void right_possibility()
        {
                if (act_player == my_player || DEBUG)
                {
                        switch (act_system_state)
                        {
                                case DisplaySystemState.meeplePosing:
                                        meeple_poss(true);
                                        break;
                                case DisplaySystemState.tilePosing:
                                        tile_poss(true);
                                        break;
                        }
                }
        }

        public bool areSamePos(PositionRepre pos1, PositionRepre pos2)
        {
                if (pos1 == null && pos2 == null)
                        return true;
                else if ((pos1 == null && pos2 != null) || (pos1 != null && pos2 == null))
                        return false;
                else
                        return (pos1.X == pos2.X && pos1.Y == pos2.Y) ? true : false;
        }


        public void setNextState(DisplaySystemState next_state)
        {
                state_transition.Enqueue(next_state);
                DisplaySystemState old_state = act_system_state;
                act_system_state = DisplaySystemState.StateTransition;
                if (state_transition.Count == 1)
                {
                        stateLeave(old_state, next_state);
                }
        }

        void stateLeave(DisplaySystemState old_state, DisplaySystemState new_state)
        {
                Debug.Log("Leaving " + old_state + " to " + new_state);
                switch (new_state)
                {
                        case DisplaySystemState.meeplePosing:
                                table.setBaseState(TableState.MeepleState);
                                break;
                        case DisplaySystemState.tilePosing:
                                table.setBaseState(TableState.TileState);
                                if (old_state == DisplaySystemState.turnStart)
                                        banner.timerTour.startTimer();
                                break;
                        case DisplaySystemState.idleState:
                                if ((DEBUG || my_player == act_player) && (old_state == DisplaySystemState.tilePosing || old_state == DisplaySystemState.meeplePosing))
                                {
                                        if (act_meeple != null)
                                                system_back.sendTile(new TurnPlayParam(act_tile.Id, act_tile.Pos, act_meeple.Id, act_meeple.SlotPos));
                                        else
                                                system_back.sendTile(new TurnPlayParam(act_tile.Id, act_tile.Pos, -1, -1));
                                }
                                break;
                        case DisplaySystemState.turnStart:
                                table.setBaseState(TableState.TileState);
                                banner.timerTour.resetStop();
                                break;
                }

                switch (old_state)
                {
                        case DisplaySystemState.meeplePosing:
                                act_tile.hidePossibilities();
                                break;

                        case DisplaySystemState.idleState:
                                TurnPlayParam play_param;
                                system_back.getTile(out play_param);
                                int index;
                                // tuile posée
                                if (play_param.id_tile != -1)
                                {
                                        if (act_tile == null || act_tile.Id != play_param.id_tile)
                                        {
                                                index = -1;
                                                for (int i = 0; index < 0 && i < tiles_hand.Count; i++)
                                                {
                                                        index = tiles_hand[i].Id == play_param.id_tile ? i : -1;
                                                }
                                                if (index == -1)
                                                {
                                                        tiles_hand.Add(instantiateTileOfId(play_param.id_tile));
                                                        index = tiles_hand.Count - 1;
                                                }
                                                Debug.Log("Changing tile");
                                                setSelectedTile(index, true);
                                        }
                                        board.finalizeTurn(play_param.tile_pos, act_tile);
                                        table.tilePositionChanged(act_tile);

                                        if (play_param.id_meeple != -1)
                                        {
                                                if (act_meeple == null || act_meeple.Id != play_param.id_meeple)
                                                {
                                                        index = -1;
                                                        for (int i = 0; index == -1 && i < meeples_hand.Count; i++)
                                                        {
                                                                index = meeples_hand[i].Id == play_param.id_meeple ? i : -1;
                                                        }
                                                        if (index == -1)
                                                        {
                                                                //TODO creer meeple
                                                        }
                                                        setSelectedMeeple(index);
                                                }
                                                act_tile.setMeeplePos(act_meeple, play_param.slot_pos);
                                                table.meeplePositionChanged(act_meeple);
                                        }
                                        else
                                        {
                                                if (act_meeple != null && act_meeple.ParentTile != null)
                                                {
                                                        act_meeple.ParentTile = null;
                                                        table.meeplePositionChanged(act_meeple);
                                                }
                                        }
                                }
                                else
                                {
                                        if (act_tile != null && act_tile.Pos != null)
                                        {
                                                board.finalizeTurn(null, act_tile);
                                                table.tilePositionChanged(act_tile);
                                        }
                                        if (act_meeple != null && act_meeple.ParentTile != null)
                                        {
                                                act_meeple.ParentTile = null;
                                                table.meeplePositionChanged(act_meeple);
                                        }
                                }
                                act_player = null;
                                break;
                        case DisplaySystemState.tilePosing:
                                board.hideTilePossibilities();
                                break;
                }
                prev_system_state = old_state;
        }


        void stateEnter(DisplaySystemState new_state, DisplaySystemState old_state)
        {
                Debug.Log("State enterring from " + old_state + " to " + new_state);
                switch (new_state)
                {
                        case DisplaySystemState.turnStart:
                                act_player = players_mapping[system_back.getNextPlayer()];
                                Debug.Log("Tour de " + act_player.Name + " " + act_player.Id.ToString());
                                if (old_state != DisplaySystemState.noState)
                                        player_list.nextPlayer(act_player);
                                table.Focus = (my_player.Id == player_list.getActPlayer().Id);
                                turnBegin();
                                break;
                        case DisplaySystemState.tilePosing:
                                if (old_state == DisplaySystemState.turnStart)
                                {
                                        if (tiles_hand.Count > 0)
                                                setSelectedTile(0, true);
                                        if (meeples_hand.Count > 0)
                                                setSelectedMeeple(0, true);
                                }
                                board.displayTilePossibilities();
                                break;
                        case DisplaySystemState.meeplePosing:
                                act_tile.showPossibilities(act_player);
                                break;
                        case DisplaySystemState.scoreChange:
                                List<PlayerScoreParam> scores = new List<PlayerScoreParam>();
                                system_back.askScores(scores);
                                foreach (PlayerScoreParam score in scores)
                                {
                                        players_mapping[score.id_player].Score = score.points_gagnes;
                                        Debug.Log("score for " + score.id_player + " " + score.points_gagnes);
                                }
                                break;
                        case DisplaySystemState.endOfGame:
                                table.Focus = false;
                                List<PlayerScoreParam> scores_final = new List<PlayerScoreParam>();
                                system_back.askFinalScore(scores_final);
                                foreach (PlayerScoreParam score in scores_final)
                                {
                                        players_mapping[score.id_player].Score = score.points_gagnes;
                                        Debug.Log("score for " + score.id_player + " " + score.points_gagnes);
                                }
                                int n_sup = 0;
                                foreach (PlayerRepre player in players_mapping.Values)
                                {
                                        if (player.Id != my_player.Id && my_player.Score < player.Score)
                                        {
                                                n_sup += 1;
                                        }
                                }
                                score_board.setEndOfGame(my_player, 1 + n_sup);
                                break;
                }
                act_system_state = new_state;
        }

        void tableCheck(Ray ray, ref bool consumed)
        {
                RaycastHit hit;
                if ((true || act_player == my_player) && Physics.Raycast(ray, out hit, Mathf.Infinity, (1 << TableLayer)))
                {
                        consumed = table.colliderHit(hit.transform);
                }
        }

        // Update is called once per frame
        void Update()
        {
                bool mouse_consumed = false;
                if (!mouse_consumed && Input.GetMouseButtonUp(0))
                {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                        tableCheck(ray, ref mouse_consumed);

                        RaycastHit hit = new RaycastHit();
                        bool hit_valid = (DEBUG || act_player == my_player) && !mouse_consumed && Physics.Raycast(ray, out hit, Mathf.Infinity, (1 << BoardLayer));

                        if (hit_valid)
                        {
                                SlotIndic slot;
                                switch (hit.transform.tag)
                                {
                                        case "TileIndicCollider":
                                                if (act_system_state == DisplaySystemState.tilePosing && act_tile != null)
                                                {
                                                        TileIndicator tile_indic = hit.transform.parent.GetComponent<TileIndicator>();
                                                        if (board.setTileAt(tile_indic.position, act_tile))
                                                        {
                                                                act_tile.setIndexFromPos();
                                                                table.tilePositionChanged(act_tile);

                                                        }
                                                        else
                                                                Debug.Log("Tuile non placée à " + tile_indic.position.ToString());
                                                        system_back.sendAction(new DisplaySystemActionTileSetCoord(act_tile.Id, act_tile.Pos));
                                                }
                                                break;

                                        case "SlotCollider":
                                                if (act_system_state == DisplaySystemState.meeplePosing && act_meeple != null)
                                                {
                                                        slot = hit.transform.parent.GetComponent<SlotIndic>();
                                                        if (hit.transform.parent.parent != act_tile.pivotPoint)
                                                                Debug.Log("Wrong parent " + hit.transform.parent.parent.name + " instead of " + act_tile.pivotPoint.name);
                                                        else
                                                        {
                                                                bool meeple_position_is_null = act_meeple.ParentTile == null;
                                                                if (act_tile.setMeeplePos(act_meeple, slot))
                                                                {
                                                                        if (meeple_position_is_null != (act_meeple.ParentTile == null))
                                                                                table.meeplePositionChanged(act_meeple);
                                                                }
                                                                system_back.sendAction(new DisplaySystemActionMeepleSetCoord(act_tile.Id, act_tile.Pos, act_meeple.Id, act_meeple.SlotPos));
                                                        }
                                                }
                                                break;
                                        case "TileBodyCollider":
                                                if (act_system_state == DisplaySystemState.tilePosing && act_tile != null)
                                                {
                                                        PositionRepre rotation;
                                                        if (hit.transform.parent != act_tile.pivotPoint)
                                                                Debug.Log("Tried to rotate " + hit.transform.parent.name + " instead of act tile");
                                                        else if (act_tile.nextRotation(out rotation))
                                                        {
                                                                if (board.setTileAt(rotation, act_tile))
                                                                        act_tile.setIndexFromPos();
                                                                system_back.sendAction(new DisplaySystemActionTileSetCoord(act_tile.Id, act_tile.Pos));
                                                        }
                                                }
                                                break;
                                        default:
                                                Debug.Log("Hit " + hit.transform.tag + " collider in system");
                                                break;
                                }
                        }
                }
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                        var pause = pauseMenu.getGameInPause();
                        pauseMenu.setGameInPause(pause);
                }

                switch (act_system_state)
                {
                        case DisplaySystemState.meeplePosing:
                                if (act_player == my_player || DEBUG)
                                {
                                        if (Input.GetKeyDown(KeyCode.Return))
                                                meeple_accept();
                                        else if (Input.GetKeyDown(KeyCode.Backspace))
                                                meeple_cancel();
                                        else if (Input.GetKeyDown(KeyCode.L))
                                                meeple_poss(false);
                                        else if (Input.GetKeyDown(KeyCode.R))
                                                meeple_poss(true);
                                }
                                break;
                        case DisplaySystemState.tilePosing:
                                if (act_player == my_player || DEBUG)
                                {
                                        if (Input.GetKeyDown(KeyCode.Return))
                                                tile_accept();
                                        else if (Input.GetKeyDown(KeyCode.Backspace))
                                                tile_cancel();
                                        else if (Input.GetKeyDown(KeyCode.L))
                                                tile_poss(false);
                                        else if (Input.GetKeyDown(KeyCode.R))
                                                tile_poss(true);
                                }
                                break;
                        case DisplaySystemState.turnStart:
                                break;
                        case DisplaySystemState.StateTransition:
                                act_system_state = state_transition.Dequeue();
                                stateEnter(act_system_state, prev_system_state);
                                if (state_transition.Count > 0)
                                {
                                        stateLeave(act_system_state, state_transition.Peek());
                                        act_system_state = DisplaySystemState.StateTransition;
                                }
                                break;
                }
        }

        public void gameBegin()
        {
                WinCondition win = WinCondition.WinByTime;
                List<int> param = new List<int>();
                system_back.askWinCondition(ref win, param);

                List<PlayerInitParam> players_init = new List<PlayerInitParam>();
                system_back.askPlayersInit(players_init);

                int L = players_init.Count;
                for (int i = 0; i < L; i++)
                {
                        PlayerRepre pl = new PlayerRepre(players_init[i], players_color[i]);
                        players_mapping.Add(pl.Id, pl);
                        player_list.addPlayer(pl);
                }

                int first_tile = system_back.askIdTileInitial();
                TuileRepre tl = instantiateTileOfId(first_tile);
                board.setTileAt(new PositionRepre(0, 0, 0), tl);

                int min, sec;
                system_back.askTimerTour(out min, out sec);
                banner.setTimerTour(min, sec);
                banner.setPlayerNumber(L);
                my_player = players_mapping[system_back.getMyPlayer()];
                banner.setPlayer(my_player);

                banner.setWinCondition(win, table, param);
                setNextState(DisplaySystemState.turnStart);
        }

        public TuileRepre instantiateTileOfId(int id)
        {
                TuileRepre model;
                model = Resources.Load<TuileRepre>("tile" + id.ToString());
                if (model == null)
                {
                        Debug.Log("Tried to log unknown tile " + id.ToString());
                        model = tuile_model;
                }
                TuileRepre tl = Instantiate<TuileRepre>(model);
                tl.Id = id;
                return tl;
        }

        public void turnBegin()
        {
                List<MeepleInitParam> meeples_init = new List<MeepleInitParam>();
                List<TileInitParam> tiles_init = new List<TileInitParam>();

                act_meeple = null;
                act_tile = null;

                tiles_hand.Clear();

                meeples_hand.Clear();
                meeple_distrib.Clear();

                system_back.askMeeplesInit(meeples_init);
                int L = meeples_init.Count;
                for (int i = 0; i < L; i++)
                {
                        // TODO should instantiate dependnat on the type
                        MeepleRepre mp = Instantiate<MeepleRepre>(meeple_model);
                        mp.color.material.color = act_player.color;
                        mp.Id = meeples_init[i].id_meeple;
                        meeples_hand.Add(mp);
                        meeple_distrib[mp.Id] = meeples_init[i].nb_meeple;
                }

                int final_count = system_back.askTilesInit(tiles_init);
                L = tiles_init.Count;
                TuileRepre tl;
                for (int i = 0; i < L; i++)
                {
                        tl = instantiateTileOfId(tiles_init[i].id_tile);
                        if (tiles_init[i].tile_flags)
                                system_back.getTilePossibilities(tl.Id, tl.possibilitiesPosition);
                        tiles_drawned.Enqueue(tl);
                        lifespan_tiles_drawned.Enqueue(tiles_init[i].tile_flags);
                }

                table.resetHandSize(final_count, meeples_hand, meeple_distrib);
        }

        public void askPlayerOrder(LinkedList<PlayerRepre> players)
        {
                List<int> players_id = new List<int>();
                players.Clear();
                system_back.askPlayerOrder(players_id);
                for (int i = 0; i < players_id.Count; i++)
                        players.AddLast(players_mapping[players_id[i]]);
        }

        public TuileRepre getNextTile(out bool perma)
        {
                perma = false;
                if (tiles_drawned.Count == 0)
                {
                        setNextState(DisplaySystemState.tilePosing);
                        return null;
                }

                TuileRepre tile = tiles_drawned.Dequeue();

                perma = lifespan_tiles_drawned.Dequeue();
                if (perma)
                {
                        tiles_hand.Add(tile);
                }
                return tile;
        }

        public void setSelectedTile(int index, bool forced = false)
        {
                if (0 <= index && index < tiles_hand.Count && (act_system_state == DisplaySystemState.tilePosing || forced))
                {

                        TuileRepre n_tuile = tiles_hand[index];
                        if (n_tuile == act_tile)
                                return;
                        if (act_tile != null && act_tile.Pos != null)
                        {
                                board.setTileAt(null, act_tile);
                                table.tilePositionChanged(act_tile);
                        }
                        if (act_meeple != null && act_meeple.ParentTile != null)
                        {
                                act_meeple.ParentTile = null;
                                table.meeplePositionChanged(act_meeple);
                        }
                        table.activeTileChanged(act_tile, n_tuile);
                        act_tile = n_tuile;
                        if (act_system_state == DisplaySystemState.tilePosing)
                                board.setTilePossibilities(act_player, act_tile);
                        if (act_player == my_player)
                                system_back.sendAction(new DisplaySystemActionTileSelection(act_tile.Id, index));
                }
                else
                {
                        Debug.Log("Invalid tile access " + index.ToString() + " " + act_system_state.ToString());
                }
        }

        public void setSelectedMeeple(int index, bool forced = false)
        {
                // Debug.Log("Meeple posing : " + index.ToString() + " " + meeples_hand.Count);
                if (0 <= index && index < meeples_hand.Count && (act_system_state == DisplaySystemState.meeplePosing || forced))
                {
                        MeepleRepre n_meeple = meeples_hand[index];
                        if (n_meeple == act_meeple)
                                return;
                        if (act_meeple != null && act_meeple.ParentTile != null)
                        {
                                act_meeple.ParentTile = null;
                                table.meeplePositionChanged(act_meeple);
                        }
                        table.activeMeepleChanged(act_meeple, n_meeple);
                        act_meeple = n_meeple;
                        if (act_player == my_player)
                                system_back.sendAction(new DisplaySystemActionMeepleSelection(act_meeple.Id, index));
                }
                else
                {
                        Debug.Log("Invalid meeple access " + index.ToString() + " " + act_system_state.ToString());
                }
        }
}
