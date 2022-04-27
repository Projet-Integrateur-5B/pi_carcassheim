using System.Collections.Generic;
using System;
using UnityEngine;

namespace Assets.system
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

        public void Poser1ereTuile(ulong idTuile)
        {
            var t = tuileDeModelId(idTuile);
            t.X = 0; t.Y = 0; t.Rotation = 0;
            _tuiles = new List<Tuile> { t };
        }

        private void PoserTuile(Tuile tuile, Position pos)
        {
            PoserTuile(tuile, pos.X, pos.Y, pos.ROT);
        }

        private void PoserTuile(ulong idTuile, Position pos)
        {
            PoserTuile(tuileDeModelId(idTuile), pos.X, pos.Y, pos.ROT);
        }

        private void PoserTuile(ulong idTuile, int x, int y, int rot)
        {
            PoserTuile(tuileDeModelId(idTuile), x, y, rot);
        }
        private void PoserTuile(Tuile tuile, int x, int y, int rot)
        {
            tuile.X = x;
            tuile.Y = y;
            tuile.Rotation = rot;
            _tuiles.Add(tuile);
        }

        public void PoserTuileFantome(ulong idTuile, Position pos)
        {
            PoserTuileFantome(idTuile, pos.X, pos.Y, pos.ROT);
        }

        public void PoserTuileFantome(ulong idTuile, int x, int y, int rot)
        {
            var t = tuileDeModelId(idTuile);
            if (!t.Riviere)
                t.TuileFantome = true;
            _tuiles.Remove(FindTuileFantome);
            PoserTuile(t, x, y, rot);
        }

        public void ValiderTour()
        {
            FindTuileFantome.TuileFantome = false;
        }

        private Tuile FindTuileFantome
        {
            get
            {
                foreach (var item in _tuiles)
                {
                    if (item.TuileFantome)
                        return item;
                }
                return null;
            }
        }

        //private void RemoveTuilesFantomes()
        //{
        //    int toRemove = int.MaxValue;
        //    bool founded = false;
        //    for (int i = 0; i < _tuiles.Count; i++)
        //    {
        //        var current = _tuiles[i];
        //        if (current.TuileFantome)
        //        {
        //            if (founded)
        //                throw new Exception("shouldn't be more than 1 TuileFantome");
        //            founded = true;
        //            toRemove = i;
        //        }
        //    }
        //    if (founded)
        //        _tuiles.RemoveAt(toRemove);
        //}

        public Tuile tuileDeModelId(ulong id_tuile)
        {
            return Tuile.Copy(_dicoTuile[id_tuile]);
        }

        public Position[] PositionPlacementPossible(ulong idTuile)
        {
            return PositionsPlacementPossible(tuileDeModelId(idTuile));
        }

        public Position[] PositionsPlacementPossible(Tuile tuile)
        {
            var listTuiles = new List<Tuile>();

            foreach (var item in _tuiles)
            {
                if (item.TuileFantome)
                    continue;
                listTuiles.Add(item);
            }

            if (listTuiles.Count == 0)
                return new Position[] { new Position(0, 0, 0) };

            List<Position> resultat = new List<Position>();

            int x, y, rot;

            List<Position> checked_pos = new List<Position>();
            foreach (var t in listTuiles)
            {
                for (int i = 0; i < 4; i++)
                {
                    x = t.X + PositionAdjacentes[i, 0];
                    y = t.Y + PositionAdjacentes[i, 1];

                    if (checked_pos.Contains(new Position(x, y, 0)))
                        continue;

                    checked_pos.Add(new Position(x, y, 0));

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
            return PlacementLegal(tuileDeModelId(idTuile), x, y, rotation);
        }

        public bool PlacementLegal(Tuile tuile, int x, int y, int rotation)
        {
            Tuile tl = GetTuile(x, y);
            if (tl != null && !tl.TuileFantome)
            {
                return false;
            }

            Tuile[] tuilesAdjacentes = TuilesAdjacentes(x, y);

            bool auMoinsUne = true;
            for (int i = 0; i < 4; i++)
            {
                Tuile t = tuilesAdjacentes[i];

                if (t == null)
                {
                    continue;
                }

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
                if (!(TerrainCompatible(t1[i], t2[2 - i])))
                    return false;
            }
            return true;
        }

        private bool TerrainCompatible(TypeTerrain t1, TypeTerrain t2)
        {
            if (t1 == t2)
                return true;
            if (t1 == TypeTerrain.VilleBlason && t2 == TypeTerrain.Ville)
                return true;
            if (t1 == TypeTerrain.Ville && t2 == TypeTerrain.VilleBlason)
                return true;
            return false;
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
            return ZoneFermee(tuileDeModelId(idTuile), idSlot);
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
            Debug.Log("Verif tuile " + tuile.Id.ToString() + " for slot " + idSlot.ToString());
            emplacementVide = false;
            int[] positionsInternes;
            try
            {
                positionsInternes = tuile.LienSlotPosition[idSlot];
            }
            catch (Exception e)
            {
                Debug.Log("idSlot = " + idSlot);
                throw e;
            }
            List<int> positionsInternesProchainesTuilesTemp = new List<int>();
            List<Tuile> resultat = new List<Tuile>();
            int x = tuile.X, y = tuile.Y;

            int direction;
            foreach (int position in positionsInternes)
            {
                //direction = (position + (3 * tuile.Rotation)) / 3;
                direction = (4 + position / 3 - tuile.Rotation) % 4;

                Debug.Log("Direction vers la prochaine Tuile de la zone = " + direction);

                Tuile elem = GetTuile(x + PositionAdjacentes[direction, 0],
                                      y + PositionAdjacentes[direction, 1]);

                if (elem == null)
                    emplacementVide = true;

                else if (!resultat.Contains(elem))
                {
                    resultat.Add(elem);
                    var trucComplique = ((position + 3 * elem.Rotation) + 18 - 3 * tuile.Rotation) % 12;
                    switch (trucComplique % 3)
                    {
                        case 0:
                            trucComplique += 2;
                            break;
                        case 2:
                            trucComplique -= 2;
                            break;
                        default:
                            break;
                    }
                    positionsInternesProchainesTuilesTemp.Add(trucComplique);
                    //positionsInternesProchainesTuilesTemp.Add((((direction + 2) % 4) * 3 + (position % 3) + 3 * elem.Rotation) % 12);
                }
            }
            positionsInternesProchainesTuiles = positionsInternesProchainesTuilesTemp.ToArray();

            return resultat.ToArray();
        }

        public void PoserPion(ulong idJoueur, ulong idTuile, ulong idSlot)
        {
            PoserPion(idJoueur, tuileDeModelId(idTuile), idSlot);
        }

        public void PoserPion(ulong idJoueur, Tuile tuile, ulong idSlot)
        {
            tuile.Slots[idSlot].IdJoueur = idJoueur;
        }

        public int[] EmplacementPionPossible(int x, int y, ulong idJoueur, ulong id_meeple)
        {
            return EmplacementPionPossible(x, y, idJoueur);
        }

        public int[] EmplacementPionPossible(int x, int y, ulong idJoueur)
        {
            Debug.Log("Debut fonction EmplacementPionPossible avec X = " + x + " Y = " + y);
            Tuile tuile = GetTuile(x, y);
            List<int> resultat = new List<int>();
            List<(Tuile, ulong)> parcourus = new List<(Tuile, ulong)>();
            Debug.Log("NombreSlot = " + tuile.NombreSlot);
            for (int i = 0; i < tuile.NombreSlot; i++)
            {
                if (!ZoneAppartientAutreJoueur(x, y, (ulong)i, idJoueur, parcourus))
                    resultat.Add(i);
                parcourus.Clear();
            }

            return resultat.ToArray();
        }

        private bool ZoneAppartientAutreJoueur(int x, int y, ulong idSlot, ulong idJoueur, List<(Tuile, ulong)> parcourus)
        {
            Debug.Log("debut methode ZoneAppartientAutreJoueur avec x=" + x + " y=" + y + " idslot=" + idSlot + " idJoueur=" + idJoueur
                + " liste des tuiles parcourues de longeur: " + parcourus.Count);
            bool vide, resultat = false;
            int[] positionsInternesProchainesTuiles;
            Tuile[] adj = TuilesAdjacentesAuSlot(GetTuile(x, y), idSlot, out vide, out positionsInternesProchainesTuiles);

            Debug.Log("methode TuilesAdjacentesAuSlot appelee, adj de longueur: " + adj.Length);
            foreach (var item in adj)
            {
                Debug.Log("Tuile dans adj :" + item.ToString());
            }

            if (adj.Length == 0)
                return false;
            int c = 0;
            foreach (var t in adj)
            {
                if (t == null || parcourus.Contains((t, idSlot)))
                    continue;
                parcourus.Add((t, idSlot));

                int pos = positionsInternesProchainesTuiles[c++];
                ulong nextSlot = t.IdSlotFromPositionInterne(pos);
                ulong idJ = t.Slots[nextSlot].IdJoueur;

                Debug.Log("Verification sur " + t.ToString() + ". idSlot : " + nextSlot + " " + t.Slots.ToString());

                if (idJ != ulong.MaxValue)
                    return true;
                resultat = resultat || ZoneAppartientAutreJoueur(t.X, t.Y, nextSlot, idJoueur, parcourus);
            }

            return resultat;
        }

        public bool PionPosable(int x, int y, ulong idSlot, ulong idJoueur, ulong idMeeple)
        {
            Tuile tuile = GetTuile(x, y);

            Debug.Log("LE PION EST IL POSABLE SUR LA TUILE " + tuile.ToString() + " SLOT :" + idSlot + " ?");

            if (tuile == null || (ulong)tuile.NombreSlot < idSlot)
                return false;

            int[] tab = EmplacementPionPossible(x, y, idJoueur);
            for (int i = 0; i < tab.Length; i++)
            {
                if ((ulong)tab[i] == idSlot)
                    return true;
            }
            return false;
        }

        public Dictionary<ulong, int> RemoveAllPawnInZone(int x, int y, ulong idSlot)
        {
            List<(Tuile, ulong)> parcourues = new List<(Tuile, ulong)>();
            var tuile = GetTuile(x, y);
            var result = new Dictionary<ulong, int>();
            RemoveAllPawnInZoneAux(tuile, idSlot, parcourues, ref result);
            return result;
        }
        private Dictionary<ulong, int> RemoveAllPawnInZoneAux(Tuile tuile, ulong idSlot,
            List<(Tuile, ulong)> parcourues, ref Dictionary<ulong, int> result)
        {
            bool vide;
            int[] positionsInternesProchainesTuiles;
            parcourues.Add((tuile, idSlot));
            Tuile[] adj = TuilesAdjacentesAuSlot(tuile, idSlot, out vide, out positionsInternesProchainesTuiles);

            ulong idCurrentJoueur = tuile.Slots[idSlot].IdJoueur;
            if (result.ContainsKey(idCurrentJoueur))
                result[idCurrentJoueur] += 1;
            else
                result.Add(idCurrentJoueur, 1);
            tuile.Slots[idSlot].IdJoueur = ulong.MaxValue;
            for (int i = 0; i < adj.Length; i++)
            {
                Tuile currentTuile = adj[i];
                ulong nextSlot = currentTuile.IdSlotFromPositionInterne(positionsInternesProchainesTuiles[i]);
                if (currentTuile == null || parcourues.Contains((currentTuile, nextSlot)))
                    continue;
                RemoveAllPawnInZoneAux(currentTuile,
                    nextSlot, parcourues, ref result);
            }
            return result;
        }
    }
}