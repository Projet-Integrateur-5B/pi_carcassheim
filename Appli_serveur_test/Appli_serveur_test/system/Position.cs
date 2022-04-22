namespace system
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

        public bool IsExisting()
        {
            if(_x == -1 && _y == -1 && _rot == -1)
            {
                return false;
            }

            return true;
        }

        public void SetNonExistent()
        {
            _x = -1;
            _y = -1;
            _rot = -1;
        }

        public override string ToString()
        {
            return _x.ToString() + _y.ToString() + _rot.ToString();
        }
    }
}
