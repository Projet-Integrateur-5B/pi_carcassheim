using System.Collections.Generic;
using UnityEngine;

public partial class Tuile
{
    private readonly Slot[] _slots;
    private readonly int _nombreSlot;
    private readonly int[][] _lienSlotPosition;
    private readonly int _id;
    public bool Riviere
    {
        get
        {
            foreach (var item in _slots)
            {
                if (item.Terrain == TypeTerrain.Riviere)
                    return true;
            }
            return false;
        }
    }
    public int Id => _id;
    public Slot[] Slots => _slots;
    public int[][] LienSlotPosition => _lienSlotPosition;
    public int NombreSlot => _nombreSlot;
    private readonly int _proba;
    public int Pobabilite => _proba;

    public int X { get; set; }
    public int Y { get; set; }
    public int Rotation { get; set; }

    public static Dictionary<string, int> PointsCardPos = new Dictionary<string, int>()
    {
        {"NNO", 0},
        {"N", 1},
        {"NNE", 2},
        {"NEE", 3},
        {"E", 4},
        {"SEE", 5},
        {"SSE", 6},
        {"S", 7},
        {"SSO", 8},
        {"SOO", 9},
        {"O", 10},
        {"NOO", 11}
    };

    public static Dictionary<int, Tuile> DicoTuiles { get; set; }
    public Tuile(int id, int nombreSlot, int[][] lien, TypeTerrain[] terrains)
    {
        _nombreSlot = nombreSlot;
        _slots = new Slot[nombreSlot];

        int s = 0;
        for (int i = 0; i < nombreSlot; i++)
        {
            _slots[i] = new Slot(terrains[i]);
            s += lien[i].Length;
        }
        if (nombreSlot != terrains.Length || lien.Length != nombreSlot || s != 12)
            Debug.Log("Erreur tuile d'id: " + id);


        _lienSlotPosition = lien;
    }

    public Tuile(int id, Slot[] slots, int[][] lien)
    {
        _nombreSlot = slots.Length;
        _id = id;
        _slots = slots;

        int[][] actualLink = new int[_nombreSlot][];

        for (int i = 0; i < actualLink.Length; i++)
        {
            var temp = new List<int>();
            int[] tab = lien[i];

            for (int j = 0; j < tab.Length; j++)
            {
                if (tab[j] == -1)
                {
                    break;
                }
                temp.Add(tab[j]);
            }
            actualLink[i] = temp.ToArray();
        }

        _lienSlotPosition = actualLink;
    }

    public int IdSlotFromPositionInterne(int pos)
    {
        for (int i = 0; i < _nombreSlot; i++)
        {
            foreach (int p in _lienSlotPosition[i])
            {
                if (p == pos)
                    return i;
            }
        }
        return 0;
    }
/*
    public TypeTerrain[] TerrainSurFace(int rot)
    {
        TypeTerrain[] resultat = new TypeTerrain[3];

        int[] positionInterneRecherchee = new int[3];
        for (int i = 0; i < 3; i++)
        {
            positionInterneRecherchee[i] = rot * 3 + i;
        }
        
        int compteur = 0;
        for (int i = 0; i < _nombreSlot; i++)
        {
            foreach (int position in _lienSlotPosition[i])
            {
                if (position == positionInterneRecherchee[0] ||
                    position == positionInterneRecherchee[1] ||
                    position == positionInterneRecherchee[2])
                {
                    resultat[compteur++] = _slots[i].Terrain;

                    if (X == Y)
                        Debug.Log(position);
                }
            }
        }

        return resultat;
    }*/

    public TypeTerrain[] TerrainSurFace(int rot)
    {
        TypeTerrain[] resultat = new TypeTerrain[3];

        int positionInterneRecherchee;
        for (int i = 0; i < 3; i++)
        {
            positionInterneRecherchee = rot * 3 + i;

            int idSlot = IdSlotFromPositionInterne(positionInterneRecherchee);
            resultat[i] = _slots[idSlot].Terrain;
        }

        return resultat;
    }

    static Tuile()
    {
        DicoTuiles = new Dictionary<int, Tuile>();
    }

    public static implicit operator Tuile(int id) => DicoTuiles[id];
}
