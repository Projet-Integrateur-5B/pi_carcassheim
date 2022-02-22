using Slot;

public class Tuile
{
    Slot[] _slots;
    int _nombreSlot;
    int[][] _lienSlotPosition;

    public int X { get; set; }
    public int Y { get; set; }
    public Rotation Rotation { get; set; }

    public Tuile()
    {
        _slots = new Slot[_nombreSlot];
        _lienSlotPosition = new int[_nombreSlot][];

    }
}
