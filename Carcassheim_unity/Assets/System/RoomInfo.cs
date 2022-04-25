using Assert.system;
using ClassLibrary;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Assets.System
{
    
    public class RoomInfo : MonoBehaviour
    { 
        private static RoomInfo _instance;
        public static RoomInfo Instance
        {
            get
            {

                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<RoomInfo>();

                    if (_instance == null)
                    {
                        GameObject container = new GameObject("RoomInfo");
                        _instance = container.AddComponent<RoomInfo>();
                    }
                }

                return _instance;

            }
        }


        /* Les Informations de la Room */
        private ulong idPartie { get; set; }

        private ulong idModerateur { get; set; }
        private int nbJoueur { get; set; }
        private int nbJoueurMax { get; set; }

        private int meeples { get; set; }
        private int scoreMax { get; set; }

        private Tools.Timer timerJoueur { get; set; }
        private Tools.Timer timerPartie { get; set; }

        private Dictionary<ulong,Player> Players;

        private Semaphore s_RoomInfo;

        private void Start()
        {
            nbJoueur = 0;

            Players = new Dictionary<ulong, Player>();
            s_RoomInfo = new Semaphore(1, 1);
        }

        void Awake()
        {
            //_instance = this;
            if (_instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
                _instance = this;
            }
        }
        /*
        public Player[] GetPlayes()
        {
            int taille = Players.Count;
            Player[] result = new Player[taille];
            return Players.Values.CopyTo(result,0);
        }

        public void AddPlayer(Player player)
        {
            if (!Players.Contains(player))
            {
                Players.Add(player);
                nbJoueur++;
            }
        }
        public void RemovePlayer(Player player)
        {
            if(Players.Remove(player))
                nbJoueur--;
        }

        public void SetValues(string[] values)
        {
            try
            {
                s_RoomInfo.WaitOne();
                idModerateur = ulong.Parse(values[0]);
                nbJoueur = int.Parse(values[1]);
                nbJoueurMax = int.Parse(values[2]);
                //privé 3
                //mode 4
                //nbtuile 5
                meeples = int.Parse(values[6]);
                timerPartie = (Tools.Timer)int.Parse(values[7]);
                timerJoueur = (Tools.Timer)int.Parse(values[8]);
                scoreMax = int.Parse(values[9]);

                s_RoomInfo.Release();
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        */
    }
}

    
        
