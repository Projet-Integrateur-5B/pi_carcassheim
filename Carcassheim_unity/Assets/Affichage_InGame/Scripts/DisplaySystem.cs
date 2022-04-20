using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DisplaySystemAction
{
    tileSetCoord,
    tileSelection,
    meepleSetCoord,
    meepleSelection,
    StateSelection
};

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

    [SerializeField] private Plateau board;
    Plane board_plane;
    public Tuile reference_tile;


    // * PLAYER GESTION ***************************************
    public event System.Action<PlayerRepre> OnPlayerDisconnected;

    // * TILE *************************************************
    [SerializeField] private Tuile tuile_model; //! shouldn't be a model but read assets to choose model
    private List<Tuile> tiles_hand;
    private Queue<Tuile> tiles_drawned;
    private Queue<bool> lifespan_tiles_drawned;

    public Tuile act_tile;

    // * MEEPLE ***********************************************

    [SerializeField] private Meeple meeple_model; //! shouldn't be a model but read assets to choose model
    private List<Meeple> meeples_hand;
    public Meeple act_meeple;
    public Dictionary<MeepleType, int> meeple_distrib;

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

        meeples_hand = new List<Meeple>();
        meeple_distrib = new Dictionary<MeepleType, int>();
        tiles_hand = new List<Tuile>();
        tiles_drawned = new Queue<Tuile>();
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
                    act_tile.Pos = pos;

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
                        act_meeple.setPos(act_tile, slot_pos);
                    }
                }
                board.finalizeTurn(act_tile.Pos, act_tile);
                break;
            case DisplaySystemState.tilePosing:
                board.hideTilePossibilities();
                break;
            case DisplaySystemState.meeplePosing:
                board.hideMeeplePossibilities();
                break;
        }
    }

    public void execAction(DisplaySystemAction action)
    {
        Position pos;
        int index, id, id_tile;
        DisplaySystemState state;
        switch (action)
        {
            case DisplaySystemAction.tileSetCoord:
                if (act_tile != null)
                {
                    system_back.getActionTileSetCoord(out pos);
                    act_tile.Pos = pos;
                }
                break;
            case DisplaySystemAction.tileSelection:
                system_back.getActionTileSelection(out index, out id);
                if (index < 0 || index >= tiles_hand.Count || tiles_hand[index].Id != id)
                {
                    index = -1;
                    for (int i = 0; i < tiles_hand.Count; i++)
                    {
                        if (tiles_hand[i].Id == id) { index = i; break; }
                    }
                }
                if (index != -1)
                    setSelectedTile(index, true);
                break;
            case DisplaySystemAction.meepleSelection:
                system_back.getActionMeepleSelection(out index, out id, out pos, out id_tile);
                if (board.getTileAt(pos) == act_tile)
                {
                    if (index < 0 || index >= tiles_hand.Count || meeples_hand[index].Id != id)
                    {
                        index = -1;
                        for (int i = 0; i < meeples_hand.Count; i++)
                        {
                            if (meeples_hand[i].Id == id) { index = i; break; }
                        }
                    }
                    if (index != -1)
                        setSelectedMeeple(index, true);
                }
                break;
            case DisplaySystemAction.StateSelection:
                system_back.getActionNextState(out state);
                setNextState(state);
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
                board.displayMeeplePossiblities();
                break;

        }
    }

    void tableCheck(Ray ray, ref bool consumed)
    {
        RaycastHit hit;
        if ((act_player == my_player) && Physics.Raycast(ray, out hit, Mathf.Infinity, (1 << TableLayer)))
        {
            consumed = table.colliderHit(hit.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool mouse_consumed = true;
        if (mouse_consumed && Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            tableCheck(ray, ref mouse_consumed);

            //! TEST 
            //float enter;
            if ((act_player == my_player) && !mouse_consumed && board.boardCollide(ray))
            {
                //if (enter > 0)
                //{
                Vector3 p = board.BoardCollidePos;
                if (act_system_state == DisplaySystemState.tilePosing && act_tile != null)
                {
                    act_tile.Pos = new Position();
                    act_tile.transform.position = p;
                    act_tile.transform.rotation = Quaternion.identity;
                    table.tilePositionChanged(act_tile);
                }
                else if (act_system_state == DisplaySystemState.meeplePosing && act_meeple != null)
                {
                    act_meeple.ParentTile = tiles_hand[0];
                    act_meeple.transform.position = p;
                    act_meeple.transform.rotation = Quaternion.identity;
                    table.meeplePositionChanged(act_meeple);
                }
                //}
            }
        }

        switch (act_system_state)
        {
            case DisplaySystemState.meeplePosing:
                if (act_player == my_player)
                {
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        setNextState(DisplaySystemState.idleState);
                    }
                    else if (Input.GetKeyDown(KeyCode.Backspace))
                    {
                        setNextState(DisplaySystemState.tilePosing);
                    }
                }
                break;
            case DisplaySystemState.tilePosing:
                if (act_player == my_player &&
                    Input.GetKeyDown(KeyCode.Return) &&
                    act_tile != null &&
                    act_tile.Pos != null)
                {
                    if (meeples_hand.Count > 0)
                        setNextState(DisplaySystemState.meeplePosing);
                    else
                        setNextState(DisplaySystemState.idleState);
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

        List<int> player_ids = new List<int>();
        List<string> player_names = new List<string>();
        system_back.askPlayers(player_ids, player_names);

        int L = player_ids.Count;
        for (int i = 0; i < L; i++)
        {
            PlayerRepre pl = new PlayerRepre(player_names[i], player_ids[i], players_color[i]);
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
        List<MeepleType> meeples_type = new List<MeepleType>();
        List<int> meeples_number = new List<int>();
        List<int> tile_ids = new List<int>();
        List<bool> tile_perma = new List<bool>();

        tiles_hand.Clear();
        tile_perma.Clear();

        meeples_hand.Clear();
        meeple_distrib.Clear();

        system_back.askMeeples(meeples_type, meeples_number);
        int L = meeples_type.Count;
        for (int i = 0; i < L; i++)
        {
            // TODO should instantiate dependnat on the type
            Meeple mp = Instantiate<Meeple>(meeple_model);
            mp.color.material.color = act_player.color;
            mp.meeple_type = meeples_type[i];
            meeples_hand.Add(mp);
            meeple_distrib[meeples_type[i]] = meeples_number[i];
        }
        act_meeple = null;

        int final_count = system_back.askTiles(tile_ids, tile_perma);
        L = tile_ids.Count;
        for (int i = 0; i < L; i++)
        {
            // TODO should instatntiate in function of id
            Tuile tl = Instantiate<Tuile>(tuile_model);
            Renderer red = tl.model.GetComponent<Renderer>();
            red.material.color = act_player.color;
            tl.Id = tile_ids[i];
            system_back.getTilePossibilities(tl.Id, tl.possibilitiesPosition);
            tiles_drawned.Enqueue(tl);
            lifespan_tiles_drawned.Enqueue(tile_perma[i]);
        }

        table.resetHandSize(final_count, meeples_hand);
    }

    public void askPlayerOrder(LinkedList<PlayerRepre> players)
    {
        List<int> players_id = new List<int>();
        players.Clear();
        system_back.askPlayers(players_id);
        for (int i = 0; i < players_id.Count; i++)
            players.AddLast(players_mapping[players_id[i]]);
    }

    public Tuile getNextTile(out bool perma)
    {
        perma = false;
        if (tiles_drawned.Count == 0)
        {
            setNextState(DisplaySystemState.tilePosing);
            return null;
        }

        Tuile tile = tiles_drawned.Dequeue();

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

            Tuile n_tuile = tiles_hand[index];
            if (n_tuile == act_tile)
                return;
            if (act_tile != null && act_tile.Pos != null)
            {
                act_tile.Pos = null;
                table.tilePositionChanged(act_tile);
            }
            if (act_meeple != null && act_meeple.ParentTile != null)
            {
                act_meeple.ParentTile = null;
                table.meeplePositionChanged(act_meeple);
            }
            table.activeTileChanged(act_tile, n_tuile);
            act_tile = n_tuile;
            board.setTilePossibilities(act_player, act_tile); //! peut être pas la bonne manière
            system_back.sendAction(DisplaySystemAction.meepleSelection, index, act_tile.Id);
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
            Meeple n_meeple = meeples_hand[index];
            if (n_meeple == act_meeple)
                return;
            if (act_meeple != null && act_meeple.ParentTile != null)
            {
                act_meeple.ParentTile = null;
                table.meeplePositionChanged(act_meeple); //! should be in event
            }
            table.activeMeepleChanged(act_meeple, n_meeple);
            act_meeple = n_meeple;
            system_back.sendAction(DisplaySystemAction.meepleSelection, index, act_meeple.Id, act_tile.Pos, act_tile.Id);
        }
        else
        {
            Debug.Log("Invalid meeple access " + index.ToString() + " " + act_system_state.ToString());
        }
    }
}
