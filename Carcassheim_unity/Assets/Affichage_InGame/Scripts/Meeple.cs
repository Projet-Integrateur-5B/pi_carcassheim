using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MeepleType
{
    DefaultMeeple,
    SlowMeeple,
    FastMeeple,
    GlowingMeeple,
    DrowsyMeeple
};

public class Meeple : MonoBehaviour
{

    // * LOOK *************************************************
    public Transform pivotPoint;
    public GameObject model;
    public Renderer color;

    public MeepleType meeple_type { get; set; } = MeepleType.DefaultMeeple;

    // * POSITION *********************************************
    public int Id { get; private set; }

    Tuile _parentTile = null;
    public Tuile ParentTile
    {
        set
        {
            if (value != _parentTile && (value == null || _parentTile == null))
            {
                if (value == null)
                {
                    transform.parent = null;
                    transform.localScale = new Vector3(1f, 1f, 1f);
                }
                else
                    transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            }
            _parentTile = value;
        }
        get => _parentTile;
    }
    public int SlotPos
    { set; get; } = -1; // pos
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setPos(Tuile tile, int slot_pos)
    {

    }
}
