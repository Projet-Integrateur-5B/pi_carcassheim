using System.Collections.Generic;

namespace system
{
    public class Plateau
    {
        public static readonly int[,] PositionAdjacentes;
        private Dictionary<ulong, Tuile> _dicoTuile;

        public Dictionary<ulong, Tuile> DicoTuile => _dicoTuile;

        private List<Tuile> _tuiles;
        public List<Tuile> Tuiles
        {
            get { return _tuiles; }
            set { _tuiles = value; }
        }

        public Plateau(Dictionary<ulong, Tuile> dicoTuiles)
        {
            _tuiles = new List<Tuile>();
            _dicoTuile = dicoTuiles;
            foreach(var tuile in dicoTuiles)
            {
                Console.WriteLine("Tuile ID :" + tuile.Key);
            }
        }

        public Plateau()
        {
            _tuiles = new List<Tuile>();
            //CompteurPoints.Init(this);
            _dicoTuile = new Dictionary<ulong, Tuile>();
        }

        static Plateau()
        {
            PositionAdjacentes = new int[,]
            {
            { 0, -1 },
            { 1, 0 },
            { 0, 1 },
            { -1, 0 }
            };
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

        public Tuile[] GetTuiles => _tuiles.ToArray();/*
    {
        return _tuiles.ToArray();
    }*/

        public void GenererRiviere()
        {
            List<Tuile> riviere = new List<Tuile>();

            foreach (var item in _dicoTuile.Values)
            {
                if (item.Riviere)
                    riviere.Add(item);
            }

            Riviere.Init(this, riviere.ToArray());
        }

        public void PoserTuile(Tuile tuile, Position pos)
        {
            PoserTuile(tuile, pos.X, pos.Y, pos.ROT);
        }

        public void PoserTuile(ulong idTuile, Position pos)
        {
            PoserTuile(_dicoTuile[idTuile], pos.X, pos.Y, pos.ROT);
        }


        public void PoserTuile(ulong idTuile, int x, int y, int rot)
        {
            PoserTuile(_dicoTuile[idTuile], x, y, rot);
        }
        public void PoserTuile(Tuile tuile, int x, int y, int rot)
        {
            tuile.X = x;
            tuile.Y = y;
            tuile.Rotation = rot;
            _tuiles.Add(tuile);
        }

        public Position[] PositionPlacementPossible(ulong idTuile)
        {
            return PositionsPlacementPossible(_dicoTuile[idTuile]);
        }

        public Position[] PositionsPlacementPossible(Tuile tuile)
        {
            List<Position> resultat = new List<Position>();

            int x, y, rot;
            List<int> checkedX = new List<int>(), checkedY = new List<int>();
            foreach (var t in _tuiles)
            {
                for (int i = 0; i < 4; i++)
                {
                    x = t.X + PositionAdjacentes[i, 0];
                    y = t.Y + PositionAdjacentes[i, 1];

                    if (checkedX.Contains(x) && checkedY.Contains(y))
                        continue;

                    checkedX.Add(x);
                    checkedY.Add(y);

                    for (rot = 0; rot < 4; rot++)
                    {
                        if (PlacementLegal(tuile, x, y, rot))
                        {
                            var p = new Position(x, y, rot);
                            //Debug.Log(p.ToString());
                            resultat.Add(p);
                        }
                    }
                }
            }

            return resultat.ToArray();
        }

        public bool PlacementLegal(ulong idTuile, int x, int y, int rotation)
        {
            return PlacementLegal(_dicoTuile[idTuile], x, y, rotation);
        }

        public bool PlacementLegal(Tuile tuile, int x, int y, int rotation)
        {
            if (GetTuile(x, y) != null)
            {
                return false;
            }

            Tuile[] tuilesAdjacentes = TuilesAdjacentes(x, y);

            bool auMoinsUne = false;
            for (int i = 0; i < 4; i++)
            {
                Tuile t = tuilesAdjacentes[i];

                if (t == null)
                {
                    continue;
                }
                auMoinsUne = true;

                TypeTerrain[] faceTuile1 = tuile.TerrainSurFace((rotation + i) % 4);
                TypeTerrain[] faceTuile2 = t.TerrainSurFace((t.Rotation + i + 2) % 4);

                if (!CorrespondanceTerrains(faceTuile1, faceTuile2))
                    return false;
                else
                {
                    //Debug.Log("Correspondance : " + ((t.Rotation + i + 2) % 4));
                }
                //Debug.Log("hello " + i + x + y + rotation);
            }

            return auMoinsUne;
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

        public bool ZoneFermee(ulong idTuile, ulong idSlot)
        {
            return ZoneFermee(_dicoTuile[idTuile], idSlot);
        }

        public bool ZoneFermee(Tuile tuile, ulong idSlot)
        {
            if (!_tuiles.Contains(tuile)) // ERROR
                return false;

            List<Tuile> tuilesFormantZone = new List<Tuile>
        {
            tuile
        };

            return ZoneFermeeAux(tuile, idSlot, tuilesFormantZone);
        }

        private bool ZoneFermeeAux(Tuile tuile, ulong idSlot, List<Tuile> tuilesFormantZone)
        {
            bool ferme = true, emplacementVide;
            int[] positionsInternes = new int[4];
            Tuile[] tuilesAdjSlot = TuilesAdjacentesAuSlot(tuile, idSlot, out emplacementVide, out positionsInternes);

            if (emplacementVide)
                return false;

            int c = 0;
            foreach (var item in tuilesAdjSlot)
            {
                if (!tuilesFormantZone.Contains(item))
                {
                    tuilesFormantZone.Add(item);

                    ulong idSlotProchaineTuile = item.IdSlotFromPositionInterne(positionsInternes[c++]);
                    ferme = ferme && ZoneFermeeAux(item, idSlotProchaineTuile, tuilesFormantZone);
                }
            }

            return ferme;
        }

        private Tuile[] TuilesAdjacentesAuSlot(Tuile tuile, ulong idSlot,
            out bool emplacementVide, out int[] positionsInternesProchainesTuiles)
        {
            emplacementVide = false;

            int[] positionsInternes = tuile.LienSlotPosition[idSlot];
            List<int> positionsInternesProchainesTuilesTemp = new List<int>();
            List<Tuile> resultat = new List<Tuile>();
            int x = tuile.X, y = tuile.Y;

            int direction;
            foreach (int position in positionsInternes)
            {
                //direction = (position + (3 * tuile.Rotation)) / 3;
                direction = (position / 3 + tuile.Rotation) % 3;

                Tuile elem = GetTuile(x + PositionAdjacentes[direction, 0],
                                      y + PositionAdjacentes[direction, 1]);

                if (elem == null)
                    emplacementVide = true;

                else if (!resultat.Contains(elem))
                {
                    resultat.Add(elem);
                    positionsInternesProchainesTuilesTemp.Add(
                        (position + 6 + (elem.Rotation - tuile.Rotation)) % 3);
                }
            }
            positionsInternesProchainesTuiles = positionsInternesProchainesTuilesTemp.ToArray();

            return resultat.ToArray();
        }

        public void PoserPion(ulong idJoueur, ulong idTuile, ulong idSlot)
        {
            PoserPion(idJoueur, _dicoTuile[idTuile], idSlot);
        }

        public void PoserPion(ulong idJoueur, Tuile tuile, ulong idSlot)
        {
            tuile.Slots[idSlot].IdJoueur = idJoueur;
        }

        public int[] EmplacementPionPossible(ulong idTuile, ulong idJoueur)
        {
            return EmplacementPionPossible(_dicoTuile[idTuile], idJoueur);
        }
        public int[] EmplacementPionPossible(Tuile tuile, ulong idJoueur)
        {
            List<int> resultat = new List<int>();
            List<Tuile> parcourues = new List<Tuile>();

            for (int i = 0; i < tuile.NombreSlot; i++)
            {
                if (ZoneAppartientAutreJoueur(tuile, (ulong)i, idJoueur, parcourues))
                    resultat.Add(i);
                parcourues.Clear();
            }

            return resultat.ToArray();
        }

        private bool ZoneAppartientAutreJoueur(Tuile tuile, ulong idSlot, ulong idJoueur, List<Tuile> parcourues)
        {
            bool vide, resultat = true;
            int[] positionsInternesProchainesTuiles;
            Tuile[] adj = TuilesAdjacentesAuSlot(tuile, idSlot, out vide, out positionsInternesProchainesTuiles);

            if (adj.Length == 0)
                return false;

            int c = 0;
            foreach (var t in adj)
            {
                if (t == null || parcourues.Contains(t))
                    continue;
                parcourues.Add(t);

                int pos = positionsInternesProchainesTuiles[c++];
                ulong nextSlot = t.IdSlotFromPositionInterne(pos);
                ulong idJ = t.Slots[nextSlot].IdJoueur;
                if (idJ != 0 && idJ != idJoueur)
                    return false;
                resultat = resultat && ZoneAppartientAutreJoueur(t, nextSlot, idJoueur, parcourues);
            }

            return resultat;
        }

        public bool PionPosable(ulong idTuile, ulong idSlot, ulong idJoueur)
        {
            Tuile tuile = _dicoTuile[idTuile];

            if (tuile == null || (ulong)tuile.NombreSlot < idSlot)
                return false;

            int[] tab = EmplacementPionPossible(tuile, idJoueur);
            for (int i = 0; i < tab.Length; i++)
            {
                if ((ulong)tab[i] == idSlot)
                    return true;
            }
            return false;
        }
    }
}
