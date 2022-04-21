using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileOnBoard
{
    public int Id { get; private set; }
    public Position Pos { get; private set; }
    public TuileRepre Tile { get; private set; }

    static private int id_gen = 0;

    public TileOnBoard()
    {
        Id = id_gen++;
        Pos = new Position();
        Tile = null;
    }

    public TileOnBoard(int id, Position pos, TuileRepre tile)
    {
        Id = id;
        Pos = pos;
        Tile = tile;
    }
}