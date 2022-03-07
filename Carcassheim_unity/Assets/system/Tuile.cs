using System.Collections.Generic;
using UnityEngine;

public partial class Tuile
{
    Slot[] _slots;
    int _nombreSlot;
    int[][] _lienSlotPosition;

    public int Id { get; set; }
    public Slot[] Slots => _slots;
    public int[][] LienSlotPosition => _lienSlotPosition;
    public int NombreSlot => _nombreSlot;

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
        Id = id;
        _slots = slots;
        _lienSlotPosition = lien;
        _nombreSlot = slots.Length;
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
}
