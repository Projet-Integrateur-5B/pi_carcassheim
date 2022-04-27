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

        /*public static explicit operator(PositionRepr) (Position p)
        {
            return new PositionRepr(p._x, p._y, p._rot);
        }

        public static explicit operator(Position) (PositionRepr p)
        {
            return new Position(p._x, p._y, p._rot);
        }*/
    }
}
