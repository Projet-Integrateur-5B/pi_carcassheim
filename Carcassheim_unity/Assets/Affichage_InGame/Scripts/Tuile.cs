using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tuile : MonoBehaviour
{

    // * LOOK *************************************************
    public Transform pivotPoint;
    public GameObject model;

    [SerializeField] private TMPro.TMP_Text _id_repre;

    // * STAT *************************************************
    private int _id = 0;

    public int Id
    {
        set { _id = value; _id_repre.text = value.ToString(); if (value < 0) _id_repre.gameObject.SetActive(false); }
        get { return _id; }
    }

    // * POSITION *********************************************
    public Position Pos { set; get; } = null; // null = not played
    public List<Position> possibilitiesPosition = new List<Position>();

    void Start()
    {
        possibilitiesPosition = new List<Position>();
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {

    }
}