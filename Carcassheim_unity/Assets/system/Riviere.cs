using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.system
{
    internal class Riviere
    {
        private Plateau _plateau;
        private Riviere(Plateau plateau)
        {
            _plateau = plateau;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tuilesRiviere">le 1er elem du tableau doit etre le debut de la riviere, le dernier doit en etre la fin</param>
        public static void Init(Plateau plateau, Tuile[] tuilesRiviere)
        {
            var obj = new Riviere(plateau);
            int length = tuilesRiviere.Length;
            int i1 = -1, i2 = -1;
            // faire en sorte que l'amont et l'avale soit aux extremites du tab
            for (int i = 0; i < length; i++)
            {
                if (RiviereExtreme(tuilesRiviere[i]))
                {
                    if (i1 == -1)
                        i1 = i;
                    else i2 = i;
                }
            }

            if (i2 == -1)
                throw new Exception("tuiles riviere extremes introuvable");

            (tuilesRiviere[0], tuilesRiviere[i1]) = (tuilesRiviere[i1], tuilesRiviere[0]);
            (tuilesRiviere[length - 1], tuilesRiviere[i1]) = (tuilesRiviere[i1], tuilesRiviere[length - 1]);
            obj.InitialiserRiviere(tuilesRiviere);
        }

        private void InitialiserRiviere(Tuile[] tuilesRiviere)
        {
            _plateau.PoserTuile(tuilesRiviere[0], 0, 0, 0);

            var rand = new Random();
            int x, y;
            Tuile current;

            int lastDirection = DirectionRiviereExtreme(tuilesRiviere[0]);

            for (int i = 1; i < tuilesRiviere.Length; i++)
            {
                current = tuilesRiviere[i];
                int slotR = SlotRiviere(current);
                x = Plateau.PositionAdjacentes[lastDirection, 0] + tuilesRiviere[i - 1].X;
                y = Plateau.PositionAdjacentes[lastDirection, 0] + tuilesRiviere[i - 1].Y;

                int randI;
                if (RiviereExtreme(current))
                {
                    randI = 0;
                }
                else
                    randI = rand.Next(1);
                int rot = tuilesRiviere[i].LienSlotPosition[slotR][randI] / 3;
                rot += ((lastDirection + 2) % 4);

                _plateau.PoserTuile(current, x, y, rot);
                lastDirection = 1 - current.LienSlotPosition[slotR][1 - randI] / 3;
            }
        }

        private static int SlotRiviere(Tuile tuile)
        {
            for (int i = 0; i < tuile.Slots.Length; i++)
            {
                if (tuile.Slots[i].Terrain == TypeTerrain.Riviere)
                    return i;
            }

            return -1;
        }

        private static int DirectionRiviereExtreme(Tuile tuile)
        {
            int slot = SlotRiviere(tuile);
            if (tuile.LienSlotPosition[slot].Length != 1)
                throw new Exception("tuile riviere incoherente");

            int posInterne = tuile.LienSlotPosition[slot][0];

            if (posInterne % 3 != 1)
                throw new Exception("tuile riviere incoherente");

            return tuile.Rotation + posInterne / 3;
        }

        private static bool RiviereExtreme(Tuile tuile)
        {
            int slot = SlotRiviere(tuile);
            if (tuile.LienSlotPosition[slot].Length == 1)
                return true;
            return false;
        }
    }
}
