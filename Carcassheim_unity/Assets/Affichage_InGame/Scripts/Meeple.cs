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

    public MeepleType meeple_type { get; set; } = MeepleType.DefaultMeeple; //TODO set should be private

    // * POSITION *********************************************
    //TODO create appropriate position for meeple
    public Position Pos
    { set; get; } = null; // null = not played
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
