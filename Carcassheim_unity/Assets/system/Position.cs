using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.system
{
    public struct Position
    {
        int _x, _y, _rot;

        public int X => _x;
        public int Y => _y;
        public int ROT => _rot;

        public Position(int x, int y, int rot)
        {
            _x = x;
            _y = y;
            _rot = rot;
        }

        public override string ToString()
        {
            return _x.ToString() + _y.ToString() + _rot.ToString();
        }
    }
}
