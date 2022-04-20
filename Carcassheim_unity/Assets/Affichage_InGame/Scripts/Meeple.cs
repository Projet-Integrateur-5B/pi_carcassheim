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

    public Tuile ParentTile { set; get; } = null;
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
