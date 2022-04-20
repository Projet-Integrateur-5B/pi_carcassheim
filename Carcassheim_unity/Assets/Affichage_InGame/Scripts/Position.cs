using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Position
{
    public Vector3 pos;
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Rotation { get; private set; } = -1; // 0: N; 1 : E; 2 : W; 3 : S

    public Position(int x, int y, int rotation = -1)
    {
        X = x;
        Y = y;
        Rotation = rotation;
    }

    public Position()
    {
        X = 0;
        Y = 0;
        Rotation = -1;
    }

    public override string ToString()
    {
        return "(" + X.ToString() + ", " + Y.ToString() + ") rot : " + Rotation.ToString();
    }
}
