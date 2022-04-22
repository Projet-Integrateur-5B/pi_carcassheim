using UnityEngine;
using System.Net.Sockets;
using ClassLibrary;
using Newtonsoft.Json;
using System;

namespace Assets.System
{
    public class Communication : MonoBehaviour
    {
        
        private static Communication _instance;

        private int port;
        public ulong idClient = 0;

        private Socket[] lesSockets = {null,null};
        private bool[] isConnected = {false,false};
        public int isInRoom = 0;

        public static Communication Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<Communication>();

                    if (_instance == null)
                    {
                        GameObject container = new GameObject("Communication");
                        _instance = container.AddComponent<Communication>();
                    }
                }

                return _instance;
            }
        }

        void Awake()
        {
            _instance = this;
        }

        public void LancementConnexion()
        {
            TextAsset contents = Resources.Load<TextAsset>("network/config");
            Parameters parameters = JsonConvert.DeserializeObject<Parameters>(contents.ToString());

            ClientAsync.Connection(parameters);
            ClientAsync.connectDone.WaitOne();

            isConnected[isInRoom] = true;
        }

        public void LancementDeconnexion()
        {

            ClientAsync.Disconnection(lesSockets[isInRoom]);
            ClientAsync.connectDone.WaitOne();

            lesSockets[isInRoom] = null;
            isConnected[isInRoom] = false;
        }

        public void StartListening(ClientAsync.OnPacketReceivedHandler pointeurFonction)
        {
            if(!isConnected[isInRoom])
                LancementConnexion();

            ClientAsync.OnPacketReceived += pointeurFonction;
            ClientAsync.Receive(lesSockets[isInRoom]);
        }
        public void StopListening(ClientAsync.OnPacketReceivedHandler pointeurFonction)
        {
            ClientAsync.OnPacketReceived -= pointeurFonction;
            ClientAsync.StopListening(lesSockets[isInRoom]);
        }


        public void SendAsync(Packet packet)
        {
            ClientAsync.Send(lesSockets[isInRoom],packet);
        }

        public void SetSocket(Socket socket)
        {
            lesSockets[isInRoom] = socket;
        }
    }
}
