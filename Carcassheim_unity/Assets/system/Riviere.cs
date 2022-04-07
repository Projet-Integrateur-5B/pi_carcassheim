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
        public Riviere(Plateau plateau)
        {
            _plateau = plateau;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tuilesRiviere">le 1er elem du tableau doit etre le debut de la riviere, le dernier doit en etre la fin</param>
        public void InitialiserRiviere(Tuile[] tuilesRiviere)
        {
            _plateau.Poser1ereTuile(tuilesRiviere[0]);

            var rand = new Random();

            int lastDirection = DirectionRiviereExtreme(tuilesRiviere[0]);

            for (int i = 1; i < tuilesRiviere.Length - 1; i++)
            {
                int slotR = SlotRiviere(tuilesRiviere[i]);
                int x = Plateau.PositionAdjacentes[lastDirection, 0] + tuilesRiviere[i - 1].X;
                int y = Plateau.PositionAdjacentes[lastDirection, 0] + tuilesRiviere[i - 1].Y;

                int randI = new Random().Next(1);
                int rot = tuilesRiviere[i].LienSlotPosition[slotR][randI] / 3;
                rot += ((lastDirection + 2) % 4);

                _plateau.PoserTuile(tuilesRiviere[i], x, y, rot);
                lastDirection = 1 - tuilesRiviere[i].LienSlotPosition[slotR][1 - randI] / 3;
            }

            // TODO poser derniere riviere
        }

        private int SlotRiviere(Tuile tuile)
        {
            for (int i = 0; i < tuile.Slots.Length; i++)
            {
                if (tuile.Slots[i].Terrain == TypeTerrain.Riviere)
                    return i;
            }

            return -1;
        }

        private int DirectionRiviereExtreme(Tuile tuile)
        {
            int slot = SlotRiviere(tuile);
            if (tuile.LienSlotPosition[slot].Length != 1)
                throw new Exception("tuile riviere incoherente");

            int posInterne = tuile.LienSlotPosition[slot][0];

            if (posInterne % 3 != 1)
                throw new Exception("tuile riviere incoherente");

            return tuile.Rotation + posInterne / 3;
        }
    }
}
