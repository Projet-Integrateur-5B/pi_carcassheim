using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Plateau : MonoBehaviour
{
    public event Action OnBoardExpanded;
    public float BoardRadius { get => _board_radius; private set { OnBoardExpanded?.Invoke(); _board_radius = value; } }
    private float _board_radius;
    void Start()
    {
    }

    private void Update()
    {

    }
}