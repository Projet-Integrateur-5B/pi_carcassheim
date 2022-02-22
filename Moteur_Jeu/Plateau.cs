using Tuile;
using System.Collections.Generic;

public class Plateau
{
    private List<Tuile> _tuiles;
    public List<Tuile> Tuiles
    {
        get { return _tuiles; }
        set { _tuiles = value; }
    }
    
    public Plateau()
    {
        _tuiles = new List<Tuile>();
    }
}

public Tuile GetTuile(int x, int y)
{
    foreach (var item in _tuiles)
    {
        if (item.X == x && item.Y == y)
        {
            return item;
        }
    }
    return null;
}

public Tuile[] GetTuiles()
{
    return _tuiles.ToArray();
}

public void Poser1ereTuile(Tuile tuile)
{
    PoserTuile(tuile, 0, 0, 0);
}

public void PoserTuile(Tuile tuile, int x, int y, Rotation rot)
{
    tuile.X = x;
    tuile.Y = y;
    tuile.Rotation = rot;
    _tuiles.Add(tuile);
}

public Position[] PositionsPlacementPossible(Tuile tuile)
{

}

public bool PlacementLegal(Tuile tuile, int x, int y, int rotation)
{
    if (GetTuile(x, y) == null)
        return false;
    
    Tuile[] tuilesAdjacentes = TuilesAdjacentes(x, y);

    for (int i = 0; i < 4; i++)
    {
        Tuile t = tuilesAdjacentes[i];

        if (tuile.TerrainSurFace((rotation + i + 2) % 4) ==
            t.TerrainSurFace((t.Rotation + i) % 4))
            return true;
    }

    return false;
}

private Tuile[4] TuilesAdjacentes(int x, int y)
{
    Tuile[] resultat = new Tuile[4];
    int[] tab = new int[,] { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 } };

    for (int i = 0; i < 4; i++)
    {
        resultat[i] = GetTuile(x + tab[i, 0], y + tab[i, 1]);
    }
    return resultat;
}

public struct Position
{
    int x, y, rot;
}