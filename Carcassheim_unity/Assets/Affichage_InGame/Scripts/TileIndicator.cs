using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileIndicator
{
    public GameObject Cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

    public Color Color { get => _color; private set => _color = value; }
    // public Renderer color; ?
    public Position Pos { get => _pos; private set => _pos = value; }
    public ColliderStat TileCollider;

    private Position _pos;
    private Color _color;

    public TileIndicator()
    {
        _color = new Color(UnityEngine.Random.Range(0, 1.0f), UnityEngine.Random.Range(0, 1.0f), UnityEngine.Random.Range(0, 1.0f));
        _pos.pos.x = 0;
        _pos.pos.y = 0;
        _pos.pos.z = (float)1.5;
    }

    public TileIndicator(Position pos, Color color, ColliderStat coll_model)
    {
        Pos = pos;
        Color = color;
        //TileCollider = Instantiate<ColliderStat>(coll_model, Cube.transform);
    }
}
