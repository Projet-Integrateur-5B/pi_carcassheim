using System.Collections.Generic;

namespace system
{
    public class Slot
    {
        private readonly TypeTerrain _terrain;
        private readonly ulong[] _linkOtherSlots;
        public ulong[] LinkOtherSlots => _linkOtherSlots;
        public TypeTerrain Terrain => _terrain;
        public ulong IdJoueur { get; set; }

        private static Dictionary<ulong, TypeTerrain> TerrainFromId;

        public Slot(TypeTerrain terrain, ulong[] link)
        {
            _linkOtherSlots = link;
            _terrain = terrain;
            IdJoueur = ulong.MaxValue;
        }

        public Slot(ulong idTerrain, ulong[] link)
        {
            _linkOtherSlots = link;
            _terrain = TerrainFromId[idTerrain];
            IdJoueur = ulong.MaxValue;
        }

        public Slot(ulong idTerrain) { }

        public override string ToString()
        {
            return "Slot appartenant au joueur: " + IdJoueur;
        }

        public static Slot[] CoypArray(Slot[] src)
        {
            Slot[] result = new Slot[src.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new Slot(src[i].Terrain, src[i].LinkOtherSlots);
            }
            return result;
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
