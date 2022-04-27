using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.system
{
    public partial class Tuile
    {
        private readonly Slot[] _slots;
        private readonly int _nombreSlot;
        private readonly int[][] _lienSlotPosition;
        private readonly ulong _id;
        private readonly int[,] _lienEntreSlots;

        public bool TuileFantome { get; set; } = false;
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
        public ulong Id => _id;
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

        public static Dictionary<ulong, Tuile> DicoTuiles { get; set; }

        public Tuile(ulong id, int nombreSlot, int[][] lien, TypeTerrain[] terrains)
        {
            _nombreSlot = nombreSlot;
            _slots = new Slot[nombreSlot];

            int s = 0;
            for (int i = 0; i < nombreSlot; i++)
            {
                _slots[i] = new Slot(terrains[i], new ulong[0]);
                s += lien[i].Length;
            }
            if (nombreSlot != terrains.Length || lien.Length != nombreSlot || s != 12)
                Debug.Log("Erreur tuile d'id: " + id);


            _lienSlotPosition = lien;
        }

        public Tuile(ulong id, Slot[] slots, int[][] lien, int[,] lienEntreSlots = null) : this(id, slots, lien)
        {
            if (lienEntreSlots != null)
                _lienEntreSlots = lienEntreSlots;
        }

        public Tuile(ulong id, Slot[] slots, int[][] lien)
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

        public static Tuile Copy(Tuile tuile)
        {
            Tuile result = new Tuile(tuile._id, tuile._slots, tuile._lienSlotPosition, tuile._lienEntreSlots);
            return result;
        }

        public ulong IdSlotFromPositionInterne(int pos)
        {
            for (ulong i = 0; i < (ulong)_nombreSlot; i++)
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

                ulong idSlot = IdSlotFromPositionInterne(positionInterneRecherchee);
                resultat[i] = _slots[idSlot].Terrain;
            }

            return resultat;
        }

        static Tuile()
        {
            DicoTuiles = new Dictionary<ulong, Tuile>();
        }

        //public static implicit operator Tuile(ulong id) => DicoTuiles[id];

        public override string ToString()
        {
            return "Tuile d'id : " + _id + " de position : (" + X + ", " + Y + ") R : " + Rotation;
        }

        public bool isARiver()
        {
            foreach (Slot s in _slots)
            {
                if (s.Terrain == TypeTerrain.Riviere)
                    return true;
            }
            return false;
        }
    }

}
