using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DisplaySystemState
{
    tilePosing,
    meeplePosing,
    turnStart,
    StateTransition,
    noState,
    idleState,
    endOfGame
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
    Plane board_plane;
    public TuileRepre reference_tile;


    // * PLAYER GESTION ***************************************
    public event System.Action<PlayerRepre> OnPlayerDisconnected;

    // * TILE *************************************************
    [SerializeField] private TuileRepre tuile_model; //! shouldn't be a model but read assets to choose model
    private List<TuileRepre> tiles_hand;
    private Queue<TuileRepre> tiles_drawned;
    private Queue<bool> lifespan_tiles_drawned;

    public TuileRepre act_tile;

    // * MEEPLE ***********************************************

    [SerializeField] private MeepleRepre meeple_model; //! shouldn't be a model but read assets to choose model
    private List<MeepleRepre> meeples_hand;
    public MeepleRepre act_meeple;
    public Dictionary<int, int> meeple_distrib;

    // * PLAYERS **********************************************

    private Dictionary<int, PlayerRepre> players_mapping;
    private PlayerRepre my_player, act_player;

    [SerializeField] private List<Color> players_color;



    // Start is called before the first frame update
    void Start()
    {
        TableLayer = LayerMask.NameToLayer("Table");
        BoardLayer = LayerMask.NameToLayer("Board");

        players_mapping = new Dictionary<int, PlayerRepre>();
        state_transition = new Queue<DisplaySystemState>();

        board_plane = new Plane(reference_tile.transform.forward, reference_tile.transform.position);

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
        gameBegin();
        setNextState(DisplaySystemState.turnStart);
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

    public void setNextState(DisplaySystemState next_state)
    {

        if (state_transition.Count == 0)
        {
            stateLeave(act_system_state, next_state);
        }
        state_transition.Enqueue(next_state);
        prev_system_state = act_system_state;
        act_system_state = DisplaySystemState.StateTransition;
    }

    void stateLeave(DisplaySystemState old_state, DisplaySystemState new_state)
    {
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
                if (my_player == act_player && (old_state == DisplaySystemState.tilePosing || old_state == DisplaySystemState.meeplePosing))
                {
                    if (act_meeple != null)
                        system_back.sendTile(act_tile.Id, act_tile.Pos, act_meeple.Id, act_meeple.SlotPos);
                    else
                        system_back.sendTile(act_tile.Id, act_tile.Pos, -1, -1);
                }
                act_player = null;
                break;
            case DisplaySystemState.turnStart:
                table.setBaseState(TableState.TileState);
                banner.timerTour.resetStop();
                break;
            case DisplaySystemState.endOfGame:
                score_board.setEndOfGame(my_player);
                break;

        }

        int id_tile, id_meeple, slot_pos, index;
        Position pos;
        switch (old_state)
        {
            case DisplaySystemState.meeplePosing:
                act_tile.hidePossibilities();
                break;

            case DisplaySystemState.idleState:
                system_back.getTile(out id_tile, out pos, out id_meeple, out slot_pos);
                if (id_tile != -1)
                {
                    if (act_tile.Id != id_tile)
                    {
                        index = -1;
                        for (int i = 0; i < tiles_hand.Count; i++)
                        {
                            if (tiles_hand[i].Id == id_tile) { index = i; break; }
                        }
                        if (index == -1)
                        {
                            //TODO create tile 
                        }
                        setSelectedTile(index, true);
                    }
                    board.setTileAt(pos, act_tile);

                    if (id_meeple != -1)
                    {
                        if (act_meeple.Id != id_meeple)
                        {
                            index = -1;
                            for (int i = 0; i < meeples_hand.Count; i++)
                            {
                                if (meeples_hand[i].Id == id_meeple) { index = i; break; }
                            }
                            if (index == -1)
                            {
                                //TODO create meeple
                            }
                            setSelectedMeeple(index, true);
                        }
                        act_tile.setMeeplePos(act_meeple, slot_pos);
                    }
                    board.finalizeTurn(act_tile.Pos, act_tile);
                }
                break;
            case DisplaySystemState.tilePosing:
                board.hideTilePossibilities();
                break;
        }
    }


    void stateEnter(DisplaySystemState new_state, DisplaySystemState old_state)
    {
        switch (new_state)
        {
            case DisplaySystemState.turnStart:
                act_player = players_mapping[system_back.getNextPlayer()];
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
        }
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
            bool hit_valid = (true || act_player == my_player) && !mouse_consumed && Physics.Raycast(ray, out hit, Mathf.Infinity, (1 << BoardLayer));
            /*
                        float enter;
                        if (!hit_valid && !mouse_consumed && board_plane.Raycast(ray, out enter))
                        {
                            if (act_system_state == DisplaySystemState.tilePosing && hit.transform.tag == "TileIndicCollider" && act_tile != null)
                            {
                                Vector3 p = ray.GetPoint(enter);
                                if (act_system_state == DisplaySystemState.tilePosing && act_tile != null)
                                {
                                    board.setTileAt(act_tile.possibilitiesPosition[Random.Range(0, act_tile.possibilitiesPosition.Count)], act_tile);
                                    act_tile.transform.position = p;
                                    table.tilePositionChanged(act_tile);
                                }
                            }
                            else
                                Debug.Log("PROBLEM " + hit.transform.tag);
                        }
            */
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
                                act_tile.transform.position = tile_indic.transform.position;
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
                            Position rotation;
                            if (hit.transform.parent != act_tile.pivotPoint)
                                Debug.Log("Tried to rotate " + hit.transform.parent.name + " instead of act tile");
                            else if (act_tile.nextRotation(out rotation))
                            {
                                Debug.Log("IT SUPPOSED TO ROTATE MAN");
                                board.setTileAt(rotation, act_tile);
                                system_back.sendAction(new DisplaySystemActionTileSetCoord(act_tile.Id, act_tile.Pos));
                            }
                            else
                            {
                                Debug.Log("DIDN T ROTATE");
                            }
                        }
                        break;
                    default:
                        Debug.Log("Hit " + hit.transform.tag + " collider in system");
                        break;
                }
            }
        }

        switch (act_system_state)
        {
            case DisplaySystemState.meeplePosing:
                if (act_player == my_player || true)
                {
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        setNextState(DisplaySystemState.idleState);
                        system_back.sendAction(new DisplaySystemActionStateSelection(DisplaySystemState.idleState));
                    }
                    else if (Input.GetKeyDown(KeyCode.Backspace))
                    {
                        setNextState(DisplaySystemState.tilePosing);
                        system_back.sendAction(new DisplaySystemActionStateSelection(DisplaySystemState.tilePosing));
                    }
                }
                break;
            case DisplaySystemState.tilePosing:
                if ((act_player == my_player || true) &&
                    Input.GetKeyDown(KeyCode.Return) &&
                    act_tile != null &&
                    act_tile.Pos != null)
                {
                    Debug.Log("Here " + (meeples_hand.Count > 0).ToString());
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

        int min, sec;
        system_back.askTimerTour(out min, out sec);
        banner.setTimerTour(min, sec);
        banner.setPlayerNumber(L);
        my_player = players_mapping[system_back.getMyPlayer()];
        banner.setPlayer(my_player);

        banner.setWinCondition(win, table, param);
    }

    public void turnBegin()
    {
        List<MeepleInitParam> meeples_init = new List<MeepleInitParam>();
        List<TileInitParam> tiles_init = new List<TileInitParam>();

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
        act_meeple = null;

        int final_count = system_back.askTilesInit(tiles_init);
        L = tiles_init.Count;
        TuileRepre model;
        for (int i = 0; i < L; i++)
        {
            model = Resources.Load<TuileRepre>("tile" + tiles_init[i].id_tile.ToString());
            if (model == null)
            {
                Debug.Log("Tried to log unknown tile " + tiles_init[i].id_tile.ToString());
                model = tuile_model;
            }
            TuileRepre tl = Instantiate<TuileRepre>(model);
            Renderer red = tl.model.GetComponent<Renderer>();
            red.material.color = act_player.color;
            tl.Id = tiles_init[i].id_tile;
            system_back.getTilePossibilities(tl.Id, tl.possibilitiesPosition);
            tiles_drawned.Enqueue(tl);
            lifespan_tiles_drawned.Enqueue(tiles_init[i].tile_flags);
        }

        table.resetHandSize(final_count, meeples_hand);
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
            Debug.Log("MEE");
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
        Debug.Log("Meeple posing : " + index.ToString() + " " + meeples_hand.Count);
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
