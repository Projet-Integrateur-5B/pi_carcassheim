using UnityEngine;
using System.Net.Sockets;
using ClassLibrary;
using Newtonsoft.Json;

namespace Assets.System
{
    public class Communication : MonoBehaviour
    {
        
        private static Communication _instance;

        private Socket socket;
        private bool isConnected = false;
        private int port;
        public ulong idClient = 0;

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

            socket = ClientAsync.clientSocket;
            isConnected = true;
        }

        public void LancementDeconnexion()
        {

            ClientAsync.Disconnection(socket);
            ClientAsync.connectDone.WaitOne();

            socket = null;
            isConnected = false;
        }

        public void StartListening(ClientAsync.OnPacketReceivedHandler pointeurFonction)
        {
            if(!isConnected)
                LancementConnexion();

            ClientAsync.OnPacketReceived += pointeurFonction;
            ClientAsync.Receive();
        }
        public void StopListening(ClientAsync.OnPacketReceivedHandler pointeurFonction)
        {
            ClientAsync.OnPacketReceived -= pointeurFonction;
            ClientAsync.StopListening(socket);
        }


        public void SendAsync(Packet packet)
        {
            ClientAsync.Send(packet);
        }
    }
}
