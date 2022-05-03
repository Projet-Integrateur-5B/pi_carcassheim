using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace system
{
    public class Player
    {
        /* Attributs */
        public ulong _id_player { get; }
        public uint _score { get; set; }
        public uint _triche { get; set; }
        public bool _is_ready { get; set; }
        public Socket? _socket_of_player { get; set; }

        public ulong _nbMeeples { get; set; }

        public Semaphore _s_player;

        public void AddPoints(uint points)
        {
            _s_player.WaitOne();
            Console.WriteLine("Gain de points ! Joueur " + _id_player.ToString() + " a gagné " + points.ToString() + " supplémentaires ! ("
                + _score.ToString() + "->" + (_score+points).ToString());
            _score = _score + points;
            _s_player.Release();
        }

        public Player(ulong id_player, Socket? playerSocket)
        {
            _id_player = id_player;
            _score = 0; 
            _triche = 0;
            _is_ready = false;
            _socket_of_player = playerSocket;
            _nbMeeples = 0;
            _s_player = new Semaphore(1, 1);
        }

        public Player(ulong id_player, Socket? playerSocket, ulong nbMeeples)
        {
            _id_player = id_player;
            _score = 0;
            _triche = 0;
            _is_ready = false;
            _socket_of_player = playerSocket;
            _nbMeeples = nbMeeples;
            _s_player = new Semaphore(1, 1);
        }

    }
}
