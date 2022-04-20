using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileOnBoard
{
    public int Id { get => _id; private set => _id = value; }
    public Position Pos { get => _pos; private set => _pos = value; }
    public Tuile Tile { get => _tile; private set => _tile = value; }

    static private int id_gen = 0;
    private int _id;
    private Position _pos;
    private Tuile _tile;

    public TileOnBoard()
    {
        _id = id_gen++;
        _pos.pos.x = 0;
        _pos.pos.y = 0;
        _pos.pos.z = (float)1.5;
        _tile = null;
    }

    public TileOnBoard(int id, Position pos, Tuile tile)
    {
        Id = id;
        Pos = pos;
        Tile = tile;
    }
}