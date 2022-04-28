using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Assets.system
{
    internal class CompteurPoints
    {
        Plateau _plateau;
        static CompteurPoints instance;
        private CompteurPoints(Plateau plateau)
        {
            _plateau = plateau;
        }

        public static void Init(Plateau plateau)
        {
            if (instance == null)
                instance = new CompteurPoints(plateau);
            else
                instance._plateau = plateau;
        }

        public static int CompterZoneFerme(int x, int y, int idSlot, out ulong[] idJoueur)
        {
            Tuile tuile = instance._plateau.GetTuile(x, y);
            if (tuile.NombreSlot <= idSlot)
                throw new ArgumentException("idSlot trop grand");

            //idJoueur = tuile.Slots[idSlot].IdJoueur;

            List<Tuile> parcourue = new List<Tuile> { tuile };
            int result = 0;
            Dictionary<ulong, int> pionParJoueur = new Dictionary<ulong, int>();
            instance.PointsZone(tuile, idSlot, parcourue, ref result, pionParJoueur);

            Debug.Log("POINTS : " + result);

            ulong playerWithMostPawn = ulong.MaxValue;
            int mostPawn = -1;
            List<ulong> playerGainingPoints = new List<ulong>();
            foreach (var item in pionParJoueur)
            {
                Debug.Log(item);
                if (item.Value > mostPawn)
                {
                    mostPawn = item.Value;
                    playerWithMostPawn = item.Key;
                }
            }
            Debug.Log("PION " + mostPawn);
            foreach (var item in pionParJoueur)
            {
                if (item.Value == mostPawn)
                {
                    playerGainingPoints.Add(item.Key);
                    Debug.Log("JOUEUR " + item.Key);
                }
            }
            idJoueur = playerGainingPoints.ToArray();

            return result;
        }

        private void PointsZone(Tuile tuile, int idSlot,
            List<Tuile> parcourue, ref int result, Dictionary<ulong, int> pionParJoueur)
        {
            bool vide, resultat = true;
            int[] positionsInternesProchainesTuiles;
            Tuile[] adj = TuilesAdjacentesAuSlot(tuile, idSlot, out vide, out positionsInternesProchainesTuiles);

            if (adj.Length == 0)
                return;

            int c = 0;
            foreach (var item in adj)
            {
                if (item == null || parcourue.Contains(item))
                    continue;
                parcourue.Add(item);

                int pos = positionsInternesProchainesTuiles[c++];
                int nextSlot = (int)item.IdSlotFromPositionInterne(pos);
                ulong idJ = item.Slots[nextSlot].IdJoueur;

                if (idJ != ulong.MaxValue)
                {
                    if (pionParJoueur.ContainsKey(idJ))
                        pionParJoueur[idJ]++;
                    else
                        pionParJoueur.Add(idJ, 1);
                }

                result += PointTerrain(item.Slots[nextSlot].Terrain);
                PointsZone(item, nextSlot, parcourue, ref result, pionParJoueur);
            }
        }

        private static int PointTerrain(TypeTerrain terrain)
        {
            if (terrain == TypeTerrain.VilleBlason)
                return 2;
            return 1;
        }


        private Tuile[] TuilesAdjacentesAuSlot(Tuile tuile, int idSlot,
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
                direction = (4 + (position / 3) - tuile.Rotation) % 4;

                Tuile elem = _plateau.GetTuile(x + Plateau.PositionAdjacentes[direction, 0],
                                      y + Plateau.PositionAdjacentes[direction, 1]);

                if (elem == null)
                    emplacementVide = true;

                else if (!resultat.Contains(elem))
                {
                    resultat.Add(elem);
                    var trucComplique = ((position + 3 * tuile.Rotation) + 18 - 3 * elem.Rotation) % 12;
                    switch (trucComplique % 3)
                    {
                        case 0:
                            trucComplique = (trucComplique + 2) % 12;
                            break;
                        case 2:
                            trucComplique = (trucComplique + 10) % 12;
                            break;
                        default:
                            break;
                    }
                    positionsInternesProchainesTuilesTemp.Add(trucComplique);
                }
            }
            positionsInternesProchainesTuiles = positionsInternesProchainesTuilesTemp.ToArray();

            return resultat.ToArray();
        }

        /*
        public int[] EmplacementPionPossible(Tuile tuile, int idJoueur)
        {
            List<int> resultat = new List<int>();
            List<Tuile> parcourues = new List<Tuile>();

            for (int i = 0; i < tuile.NombreSlot; i++)
            {
                if (ZoneAppartientAutreJoueur(tuile, i, idJoueur, parcourues))
                    resultat.Add(i);
                parcourues.Clear();
            }

            return resultat.ToArray();
        }

        private bool ZoneAppartientAutreJoueur(Tuile tuile, int idSlot, int idJoueur, List<Tuile> parcourues)
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
                int nextSlot = t.IdSlotFromPositionInterne(pos);
                int idJ = t.Slots[nextSlot].IdJoueur;
                if (idJ != 0 && idJ != idJoueur)
                    return false;
                resultat = resultat && ZoneAppartientAutreJoueur(t, nextSlot, idJoueur, parcourues);
            }

            return resultat;
        }*/
    }
}
