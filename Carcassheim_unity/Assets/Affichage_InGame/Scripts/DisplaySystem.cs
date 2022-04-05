using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DisplaySystemState
{
    tilePosing,
    meeplePosing,
    turnStart,
    StateTransition,
    noState
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

    // * STATE ***********************************************

    private DisplaySystemState act_system_state = DisplaySystemState.noState;
    private DisplaySystemState prev_system_state = DisplaySystemState.noState;
    private Queue<DisplaySystemState> state_transition;


    // * BOARD ***********************************************

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
    private PlayerRepre my_player;

    [SerializeField] private List<Color> players_color;



    // Start is called before the first frame update
    async void Start()
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
            case DisplaySystemState.turnStart:
                table.setBaseState(TableState.TileState);
                banner.timerTour.resetStop();
                break;
        }
        switch (old_state)
        {
            case DisplaySystemState.turnStart:
                table.activeTileChanged(null, act_tile);
                table.activeMeepleChanged(null, act_meeple);
                break;
        }
    }

    void stateEnter(DisplaySystemState new_state, DisplaySystemState old_state)
    {
        PlayerRepre player_act;
        switch (new_state)
        {
            case DisplaySystemState.turnStart:
                player_act = players_mapping[system_back.getNextPlayer()];
                if (old_state != DisplaySystemState.noState)
                    player_list.nextPlayer(player_act);
                table.Focus = (my_player.Id == player_list.getActPlayer().Id);
                turnBegin(player_act);
                break;
        }
    }

    void tableCheck(Ray ray, ref bool consumed)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, (1 << TableLayer)))
        {
            consumed = table.colliderHit(hit.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool consumed = false;
            tableCheck(ray, ref consumed);

            //! TEST 
            float enter;
            if (!consumed && board_plane.Raycast(ray, out enter))
            {
                if (enter > 0)
                {
                    Vector3 p = ray.GetPoint(enter);
                    if (act_system_state == DisplaySystemState.tilePosing && act_tile != null)
                    {
                        act_tile.Pos = new Position();
                        act_tile.transform.position = p;
                        act_tile.transform.rotation = Quaternion.identity;
                        table.tilePositionChanged(act_tile);
                    }
                    else if (act_system_state == DisplaySystemState.meeplePosing && act_meeple != null)
                    {
                        act_meeple.Pos = new Position();
                        act_meeple.transform.position = p;
                        act_meeple.transform.rotation = Quaternion.identity;
                        table.meeplePositionChanged(act_meeple);
                    }
                }
            }
        }

        switch (act_system_state)
        {
            case DisplaySystemState.meeplePosing:
                if (Input.GetKeyDown(KeyCode.Return) && act_meeple != null && act_meeple.Pos != null)
                {
                    setNextState(DisplaySystemState.turnStart);
                }
                else if (Input.GetKeyDown(KeyCode.Backspace))
                {
                    setNextState(DisplaySystemState.tilePosing);
                }
                break;
            case DisplaySystemState.tilePosing:
                if (Input.GetKeyDown(KeyCode.Return) && act_tile != null && act_tile.Pos != null)
                {
                    setNextState(DisplaySystemState.meeplePosing);
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

        table.setTileNumber(system_back.tile_number);
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

        banner.setWinCondition(win, param);

    }

    public void turnBegin(PlayerRepre player_act)
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
            mp.color.material.color = player_act.color;
            mp.meeple_type = meeples_type[i];
            meeples_hand.Add(mp);
            meeple_distrib[meeples_type[i]] = meeples_number[i];
        }
        act_meeple = meeples_hand[0];

        int final_count = system_back.askTiles(tile_ids, tile_perma);
        L = tile_ids.Count;
        for (int i = 0; i < L; i++)
        {
            // TODO should instatntiate in function of id
            Tuile tl = Instantiate<Tuile>(tuile_model);
            Renderer red = tl.model.GetComponent<Renderer>();
            red.material.color = player_act.color;
            tl.Id = tile_ids[i];
            tiles_drawned.Enqueue(tl);
            lifespan_tiles_drawned.Enqueue(tile_perma[i]);
        }

        table.resetHandSize(final_count, meeples_hand);
    }

    public void askPlayerOrder(LinkedList<PlayerRepre> players)
    {
        players.Clear();
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
            tiles_hand.Add(tile);
            if (tiles_hand.Count == 1)
            {
                act_tile = tile;
                // TODO notify the selection of this tile when entering tile selection and player
            }
        }
        return tile;
    }

    public void setSelectedTile(int index)
    {
        if (0 <= index && index < tiles_hand.Count && act_system_state == DisplaySystemState.tilePosing)
        {

            Tuile n_tuile = tiles_hand[index];
            if (n_tuile == act_tile)
                return;
            if (act_tile != null && act_tile.Pos != null)
            {
                act_tile.Pos = null;
                table.tilePositionChanged(act_tile); //! should be in event
            }
            if (act_meeple != null && act_meeple.Pos != null)
            {
                act_meeple.Pos = null;
                table.meeplePositionChanged(act_meeple); //! should be in event
            }
            table.activeTileChanged(act_tile, n_tuile);
            act_tile = n_tuile;
        }
        else
        {
            Debug.Log("Invalid tile access " + index.ToString() + " " + act_system_state.ToString());
        }
    }

    public void setSelectedMeeple(int index)
    {
        if (0 <= index && index < meeples_hand.Count && act_system_state == DisplaySystemState.meeplePosing)
        {
            Meeple n_meeple = meeples_hand[index];
            if (n_meeple == act_meeple)
                return;
            if (act_meeple != null && act_meeple.Pos != null)
            {
                act_meeple.Pos = null;
                table.meeplePositionChanged(act_meeple); //! should be in event
            }
            table.activeMeepleChanged(act_meeple, n_meeple);
            act_meeple = n_meeple;
        }
        else
        {
            Debug.Log("Invalid meeple access " + index.ToString() + " " + act_system_state.ToString());
        }
    }
}
