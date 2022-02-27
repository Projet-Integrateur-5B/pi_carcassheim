using Tuile;
using System.Collections.Generic;

public class Plateau
{
    public static const int[,] PositionAdjacentes = new int[,]
        { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 } };
    
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

    public Tuile GetTuile(int x, int y)
    {
        foreach (var item in _tuiles)
        {
            if (item.X == x && item.Y == y)
                return item;
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
        List<Position> resultat = new();

        int x, y, rot;
        List<int> checkedX = new(), checkedY = new();
        foreach (var t in _tuiles)
        {
            for (int i = 0; i < 4; i++)
            {
                x = t.X + PositionAdjacentes[0, i];
                y = t.Y + PositionAdjacentes[1, i];

                if (checkedX.Contains(x) && checkedY.Contains(y))
                    break;
                
                checkedX.Add(x);
                checkedY.Add(y);

                for (rot = 0; rot < 4; rot++)
                {
                    if (PlacementLegal(tuile, x, y, rot))
                        resultat.Add(new Position(x, y, rot));
                }
            }
        }

        return resultat.ToArray();
    }

    public bool PlacementLegal(Tuile tuile, int x, int y, int rotation)
    {
        if (GetTuile(x, y) != null)
            return false;
        
        Tuile[] tuilesAdjacentes = TuilesAdjacentes(x, y);

        for (int i = 0; i < 4; i++)
        {
            Tuile t = tuilesAdjacentes[i];

            if (t == null)
                break;

            TypeTerrain[] faceTuile1 = tuile.TerrainSurFace((rotation + i) % 4);
            TypeTerrain[] faceTuile2 = t.TerrainSurFace((t.Rotation + i + 2) % 4);

            if (!CorrespondanceTerrains(faceTuile1, faceTuile2))
                return false;
        }

        return true;
    }

    private bool CorrespondanceTerrains(TypeTerrain[] t1, TypeTerrain[] t2)
    {
        for (int i = 0; i < 3; i++)
        {
            if (t1[i] != t2[2 - i])
                return false;
        }
        return true;
    }

    private Tuile[] TuilesAdjacentes(int x, int y)
    {
        Tuile[] resultat = new Tuile[4];

        var tab = PositionAdjacentes;

        for (int i = 0; i < 4; i++)
        {
            resultat[i] = GetTuile(x + tab[i, 0], y + tab[i, 1]);
        }
        return resultat;
    }

    private Tuile[] TuilesAdjacentes(Tuile t)
    {
        return TuilesAdjacentes(t.X, t.Y);
    }

    public bool ZoneFermee(Tuile tuile, int idSlot)
    {
        
    }
}

public struct Position
{
    int _x, _y, _rot;

    public int X => _x;
    public int Y => _y;
    public int ROT => _rot;

    public Position(int x, int y, int rot)
    {
        _x = x;
        _y = y;
        _rot = rot;
    }
}