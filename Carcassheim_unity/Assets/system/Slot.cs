using System.Collections.Generic;

public class Slot
{
    private readonly TypeTerrain _terrain;
    public TypeTerrain Terrain => _terrain;
    public int IdJoueur { get; set; }

    private static Dictionary<int, TypeTerrain> TerrainFromId;

    public Slot(TypeTerrain terrain)
    {
        _terrain = terrain;
        IdJoueur = 0;
    }

    public Slot(int idTerrain)
    {
        _terrain = TerrainFromId[idTerrain];
        IdJoueur = 0;
    }

    static Slot()
    {
        TerrainFromId = new Dictionary<int, TypeTerrain>
        {
            { 0, TypeTerrain.Ville },
            { 1, TypeTerrain.Route },
            { 2, TypeTerrain.Pre },
            { 3, TypeTerrain.Abbaye },
            { 4, TypeTerrain.Auberge },
            { 5, TypeTerrain.Cathedrale }
        };
    }
}