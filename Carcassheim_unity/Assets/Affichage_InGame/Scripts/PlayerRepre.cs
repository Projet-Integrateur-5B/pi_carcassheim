using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerRepre
{

    public Color color;

    public event Action<uint, uint> OnScoreUpdate;
    public event Action<uint> OnMeepleUpdate;


    public int Id { get => _id; private set => _id = value; }
    public string Name { get => _name; private set => _name = value; }
    public uint Score
    {
        get => _score;
        set
        {
            OnScoreUpdate?.Invoke(_score, value);
            _score = value;
        }
    }
    public uint NbMeeple
    {
        get => _nb_meeple;
        set
        {
            OnMeepleUpdate?.Invoke(value);
            _nb_meeple = value;
        }
    }

    static private int id_gen = 0;
    private int _id;
    private uint _score;
    private uint _nb_meeple;
    private string _name;


    public PlayerRepre()
    {
        color = new Color(UnityEngine.Random.Range(0, 1.0f), UnityEngine.Random.Range(0, 1.0f), UnityEngine.Random.Range(0, 1.0f));

        _id = id_gen++;
        _nb_meeple = 0;
        _score = 0;
        _name = "Zorglub";
    }

    public PlayerRepre(string name, int id, Color col)
    {
        Id = id;
        Name = name;
        color = col;
        _score = 0;
        _nb_meeple = 0;
    }
}
