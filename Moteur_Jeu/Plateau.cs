using Tuile;

public class Plateau
{
    private List<Tuile> _tuiles;
    public List<Tuile> Tuiles
    {
        get { return _tuiles; }
        set { _tuiles = value; }
    }
    
    public Plateau()
    {
        _tuiles = new List<Tuile>();
    }
}

public void PoserTuile(Tuile tuile, int x, int y, Rotation rot)
{
    tuile.X = x;
    tuile.Y = y;

    _tuiles.Add(tuile);
}