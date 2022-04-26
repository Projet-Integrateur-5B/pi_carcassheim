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
            string r = "";
            switch (ROT)
            {
                case 0:
                    r = "nord";
                    break;
                case 1:
                    r = "est";
                    break;
                case 2:
                    r = "sud";
                    break;
                case 3:
                    r = "ouest";
                    break;
                default:
                    r = "lol";
                    break;

            }
            return "("+ _x.ToString()+", " + _y.ToString()+", " + r + ")";
        }
    }
}
