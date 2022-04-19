using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            instance = new CompteurPoints(plateau);
        }

        public static int CompterZoneFerme(int idTuile, int idSlot, int idJoueur = -1)
        {
            Tuile tuile = idTuile;
            if (tuile.NombreSlot <= idSlot)
                throw new ArgumentException("idSlot trop grand");

            idJoueur = tuile.Slots[idSlot].IdJoueur;

            List<Tuile> parcourue = new List<Tuile> { tuile };
            int result = 0;
            instance.PointsZone(tuile, idSlot, parcourue, ref result);

            return 0;
        }

        private void PointsZone(Tuile tuile, int idSlot, List<Tuile> parcourue, ref int result)
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
                int nextSlot = item.IdSlotFromPositionInterne(pos);
                int idJ = item.Slots[nextSlot].IdJoueur;

                result += PointTerrain(item.Slots[nextSlot].Terrain);
                PointsZone(item, nextSlot, parcourue, ref result);
            }
        }

        private static int PointTerrain(TypeTerrain terrain)
        {
            if (terrain == TypeTerrain.Ville)
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
                direction = (position / 3 + tuile.Rotation) % 3;

                Tuile elem = _plateau.GetTuile(x + Plateau.PositionAdjacentes[direction, 0],
                                      y + Plateau.PositionAdjacentes[direction, 1]);

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
