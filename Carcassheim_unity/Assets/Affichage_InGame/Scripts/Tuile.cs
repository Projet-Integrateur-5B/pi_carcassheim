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
        set { _id = value; _id_repre.text = value.ToString(); }
        get { return _id; }
    }

    // * POSITION *********************************************
    public Position Pos { set; get; } = null; // null = not played

    void Start()
    {
        _id_repre.gameObject.SetActive(true);
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {

    }
}