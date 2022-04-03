using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TableState
{
    TileState,
    MeepleState,
    InfoState,

    StateTransition,
    ValidState
}

public class Table : MonoBehaviour
{

    [SerializeField] private DisplaySystem display_system;

    // * TURN GESTION *****************************************
    [SerializeField] private candle_manager candle;

    private bool _focus = false;
    public bool Focus
    {
        set
        {
            _focus = value;
            if (_focus)
                candle.lightCandle();
            else
                candle.shutCandle();
        }
        get { return _focus; }
    }

    // * PLACEMENT ********************************************
    [SerializeField] private Transform balise1_model;
    [SerializeField] private Transform balise2_model;

    private Vector3 balise1, balise2;
    private Quaternion unselected_angle;

    // * TABLE STATE ******************************************
    private TableState act_table_state = TableState.TileState;
    private TableState prev_table_state = TableState.TileState;
    private TableState valid_table_state = TableState.TileState;
    private Queue<TableState> state_transition;

    // * TILE *************************************************

    [SerializeField] private GameObject tile_zone;
    [SerializeField] private Indicator tile_pack;
    [SerializeField] private ColliderStat tile_collider_model;

    private int act_tile_count;
    private int planned_tile_count;

    private Vector3 tile_origin, tile_step;
    private Dictionary<Tuile, ColliderStat> tile_mapping;

    // * MEEPLE ***********************************************
    [SerializeField] private GameObject meeple_zone;
    [SerializeField] private MeepleColliderStat meeple_collider_model;

    private Vector3 meeple_origin, meeple_step;
    private Dictionary<Meeple, MeepleColliderStat> meeple_mapping;

    // * INFO *************************************************
    [SerializeField] private GameObject info_zone;


    // Start is called before the first frame update
    void Start()
    {
        tile_mapping = new Dictionary<Tuile, ColliderStat>();
        meeple_mapping = new Dictionary<Meeple, MeepleColliderStat>();
        state_transition = new Queue<TableState>();

        balise1 = balise1_model.position;
        balise2 = balise2_model.position;
        tile_zone.transform.localPosition = balise1_model.localPosition;
        meeple_zone.transform.localPosition = balise1_model.localPosition;
        unselected_angle = Quaternion.AngleAxis(90, balise2 - balise1);
    }

    // Update is called once per frame
    void Update()
    {
        switch (act_table_state)
        {
            case TableState.StateTransition:
                // TODO only pass to next state if animation are finished
                stateEnter(state_transition.Dequeue(), prev_table_state);
                break;
        }
    }

    public void setTileNumber(int remmaining_card)
    {
        tile_pack.Value = remmaining_card;
    }

    IEnumerator RawDrawTile()
    {
        Tuile last_tile = null;
        bool perma_tile, last_perma_tile = true;
        Tuile tile = null;
        while ((tile = display_system.getNextTile(out perma_tile)) != null)
        {
            tile_pack.Value--;
            if (!last_perma_tile)
            {
                Destroy(last_tile.gameObject);
            }

            tile.transform.parent = tile_zone.transform;
            tile.model.layer = DisplaySystem.TableLayer;
            tile.pivotPoint.rotation = unselected_angle;
            tile.transform.localPosition = tile_origin + tile_step * act_tile_count;

            if (perma_tile)
            {
                ColliderStat collider = Instantiate<ColliderStat>(tile_collider_model, tile_zone.transform);
                collider.transform.position = tile.transform.position;
                collider.Index = act_tile_count;
                tile_mapping.Add(tile, collider);
                act_tile_count += 1;
            }

            last_tile = tile;
            last_perma_tile = perma_tile;
            yield return new WaitForSeconds(0.3f);
        }
    }

    public void resetHandSize(int hand_size, List<Meeple> meeples)
    {
        planned_tile_count = hand_size;

        tile_step = (balise2 - balise1) / hand_size;
        tile_origin = tile_step / 2f;

        meeple_step = (balise2 - balise1) / meeples.Count;
        meeple_origin = meeple_step / 2f;

        cleanHand();

        int i = 0;
        foreach (Meeple mpl in meeples)
        {
            MeepleColliderStat mps = Instantiate<MeepleColliderStat>(meeple_collider_model, meeple_zone.transform);
            mps.transform.localPosition = meeple_origin + meeple_step * i;
            mps.Index = i;

            mpl.transform.parent = meeple_zone.transform;
            mpl.transform.localPosition = mps.transform.localPosition;
            mpl.model.layer = DisplaySystem.TableLayer;
            mpl.pivotPoint.rotation = unselected_angle;

            meeple_mapping[mpl] = mps;

            i++;
        }

        StartCoroutine(RawDrawTile());

    }

    void cleanHand()
    {
        int L = meeple_zone.transform.childCount;
        Transform c;
        for (int i = L - 1; i >= 0; i--)
        {
            c = meeple_zone.transform.GetChild(i);
            if (c.tag == "RayCollider")
                Destroy(c.gameObject);
            else
                Destroy(c.gameObject); //TODO destroy animation
        }
        L = tile_zone.transform.childCount;
        for (int i = L - 1; i >= 0; i--)
        {
            c = tile_zone.transform.GetChild(i);
            if (c.tag == "RayCollider")
                Destroy(c.gameObject);
            else
                Destroy(c.gameObject); //TODO destroy animation
        }

        act_tile_count = 0;
        tile_mapping.Clear();
        meeple_mapping.Clear();
    }

    public void setBaseState(TableState new_base_state)
    {

        if (act_table_state == valid_table_state || _focus)
        {
            prev_table_state = act_table_state;
            state_transition.Enqueue(TableState.ValidState);
            stateLeave(act_table_state, new_base_state);
            act_table_state = TableState.StateTransition;
        }
        valid_table_state = new_base_state;
    }

    void stateEnter(TableState new_state, TableState old_state)
    {
        if (new_state == TableState.ValidState)
        {
            new_state = valid_table_state;
        }
        switch (new_state)
        {
            case TableState.TileState:
                tile_zone.SetActive(true);
                break;
            case TableState.MeepleState:
                meeple_zone.SetActive(true);
                break;
            default:
                break;
        }
        act_table_state = new_state;
        if (new_state != old_state)
        {
            switch (old_state)
            {
                case TableState.TileState:
                    tile_zone.SetActive(false);
                    break;
                case TableState.MeepleState:
                    meeple_zone.SetActive(false);
                    break;
            }
        }
        else
        {
            Debug.Log("Change from state " + old_state.ToString() + " " + new_state.ToString());
        }
    }

    void stateLeave(TableState old_state, TableState new_state)
    {

    }

    public bool colliderHit(Transform hit)
    {
        // Debug.Log("Collided with " + hit.name + " in state " + act_table_state.ToString());
        switch (act_table_state)
        {
            case TableState.MeepleState:
                if (hit.parent != meeple_zone.transform)
                {
                    Debug.Log("Parent of hit is " + hit.parent.name + " instead of " + meeple_zone.name);
                    break;
                }
                display_system.setSelectedMeeple(hit.GetComponent<MeepleColliderStat>().Index);
                return true;
            case TableState.TileState:
                if (hit.parent != tile_zone.transform)
                {
                    Debug.Log("Parent of hit is " + hit.parent.name + " instead of " + tile_zone.name);
                    break;
                }
                display_system.setSelectedTile(hit.GetComponent<ColliderStat>().Index);
                return true;
            default:
                Debug.Log("Shouldn't have been an input in " + act_table_state.ToString());
                break;
        }
        return true;
    }

    public void activeTileChanged(Tuile old_tile, Tuile new_tile)
    {
        if (old_tile != null)
        {
            old_tile.pivotPoint.rotation = unselected_angle;
        }
        new_tile.pivotPoint.rotation = Quaternion.identity;

    }

    public void activeMeepleChanged(Meeple old_meeple, Meeple new_meeple)
    {
        if (old_meeple != null)
        {
            old_meeple.pivotPoint.rotation = unselected_angle;
        }
        new_meeple.pivotPoint.rotation = Quaternion.identity;
    }

    public void tilePositionChanged(Tuile tile)
    {
        if (tile.Pos == null)
        {
            ColliderStat tst = tile_mapping[tile];
            tile.transform.parent = tile_zone.transform;
            tile.transform.position = tst.transform.position;
            tile.model.layer = DisplaySystem.TableLayer;
            if (tile != display_system.act_tile)
                tile.pivotPoint.rotation = unselected_angle;
        }
        else
        {
            tile.transform.parent = null;
            tile.model.layer = DisplaySystem.BoardLayer;
        }
    }

    public void meeplePositionChanged(Meeple meeple)
    {
        if (meeple.Pos == null)
        {
            MeepleColliderStat mps = meeple_mapping[meeple];
            meeple.transform.position = mps.transform.position;
            meeple.transform.parent = meeple_zone.transform;
            meeple.model.layer = DisplaySystem.TableLayer;
            if (meeple != display_system.act_meeple)
                meeple.pivotPoint.rotation = unselected_angle;
        }
        else
        {
            meeple.transform.parent = null;
            meeple.model.layer = DisplaySystem.BoardLayer;
        }
    }
}
