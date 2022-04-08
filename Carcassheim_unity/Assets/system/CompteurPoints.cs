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

        public static int Compter(int idJoueur)
        {
            return instance.Points(idJoueur);
        }

        private int Points(int idJoueur)
        {
            return 0;
        }
    }
}
