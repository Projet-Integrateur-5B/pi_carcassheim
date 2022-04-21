using System.Collections.Generic;

namespace system
{
    public class Slot
    {
        private readonly TypeTerrain _terrain;
        public TypeTerrain Terrain => _terrain;
        public ulong IdJoueur { get; set; }

        private static Dictionary<ulong, TypeTerrain> TerrainFromId;

        public Slot(TypeTerrain terrain)
        {
            _terrain = terrain;
            IdJoueur = 0;
        }

        public Slot(ulong idTerrain)
        {
            _terrain = TerrainFromId[idTerrain];
            IdJoueur = 0;
        }

        static Slot()
        {
            TerrainFromId = new Dictionary<ulong, TypeTerrain>
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
}
