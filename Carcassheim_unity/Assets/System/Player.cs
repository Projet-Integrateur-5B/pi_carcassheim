using System.Net.Sockets;
using System.Threading;

namespace Assert.system
{
    public class Player
    {
        /* Attributs */
        public ulong id { get; }
        public string name { get; set; }
        public uint nbMeeples { get; set; }
        public uint score { get; set; }

        public Player(ulong player_id,string player_name,uint player_nbMeeples,uint player_score)
        {
            id = player_id;
            name = player_name;
            nbMeeples = player_nbMeeples;
            score = player_score;
        }
    }
}
