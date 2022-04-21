using UnityEngine;
using System.Net.Sockets;

namespace Assets.System
{
    public class Communication : MonoBehaviour
    {
        private Socket Socket;
        private static Communication _instance;

        public static Communication Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<Communication>();

                    if (_instance == null)
                    {
                        GameObject container = new GameObject("Bicycle");
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

        public void ConnectedToServer()
        {
            var error_value = Client.Connection(ref Communication.Instance.Socket, 19000);
            switch (error_value)
            {
                case Tools.Errors.None:
                    break;
                case Tools.Errors.ConfigFile:
                    // TODO : handle case : config file is bad or issue while extracting the data
                    Debug.Log(string.Format("Errors.ConfigFile"));
                    break;
                case Tools.Errors.Socket:
                    // TODO : handle case : connection could not be established
                    Debug.Log(string.Format("Errors.Socket"));
                    break;
                case Tools.Errors.ToBeDetermined:
                    break;
                case Tools.Errors.Unknown:
                    break;
                case Tools.Errors.Format:
                    break;
                case Tools.Errors.Receive:
                    break;
                case Tools.Errors.Data:
                    break;
                case Tools.Errors.Permission:
                    break;
                case Tools.Errors.Success:
                    break;
                default:
                    // TODO : handle case : default
                    Debug.Log(string.Format("Errors.Unknown"));
                    break;
            }
        }

        public void DisconnectToServer()
        {
            if (Communication.Instance.Socket == null)
                return;

            var error_value = Client.Disconnection(Communication.Instance.Socket);
            switch (error_value)
            {
                case Tools.Errors.None:
                    break;
                case Tools.Errors.Socket:
                    // TODO : handle case : connection could not be closed
                    Debug.Log(string.Format("Errors.Socket"));
                    break;
                case Tools.Errors.Unknown:
                    break;
                case Tools.Errors.Format:
                    break;
                case Tools.Errors.ConfigFile:
                    break;
                case Tools.Errors.Receive:
                    break;
                case Tools.Errors.Data:
                    break;
                case Tools.Errors.ToBeDetermined:
                    break;
                case Tools.Errors.Permission:
                    break;
                case Tools.Errors.Success:
                    break;
                default:
                    // TODO : handle case : default
                    Debug.Log(string.Format("Errors.Unknown"));
                    break;
            }
        }

        public bool CommunicationWithoutResult(Tools.IdMessage typeMessage, string[] values)
        {
            if (Communication.Instance.Socket == null)
                 ConnectedToServer();

            Packet original = new Packet();
            var error_value = Communication.Instance.Socket.Communication(ref original, typeMessage, values);
            bool res = false;

            switch (error_value)
            {
                case Tools.Errors.None:
                    if (original.Error == Tools.Errors.None)
                        Debug.Log("None");
                        res = true;
                    break;
                case Tools.Errors.Format: // == Errors.Receive ?
                                          // TODO : handle case : wrong format
                    Debug.Log(string.Format("Errors.Format"));
                    break;
                case Tools.Errors.Socket:
                    // TODO : handle case : connection error
                    Debug.Log(string.Format("Errors.Socket"));
                    break;
                case Tools.Errors.Data:
                    // TODO : handle case : error while getting the packet ready
                    Debug.Log(string.Format("Errors.Data"));
                    break;
                case Tools.Errors.Receive:
                    // TODO : handle case : error while receiving an answer
                    Debug.Log(string.Format("Errors.Receive"));
                    break;
                case Tools.Errors.Unknown:
                    break;
                case Tools.Errors.ConfigFile:
                    break;
                case Tools.Errors.ToBeDetermined:
                    break;
                case Tools.Errors.Permission:
                    break;
                case Tools.Errors.Success:
                    if(original.Error == Tools.Errors.Success)
                        res = true;
                    break;
                default:
                    // TODO : handle case : default
                    Debug.Log(string.Format("Errors.Unknown"));
                    break;
            }

            return res;
        }

        public Packet CommunicationWithResult(Tools.IdMessage typeMessage, string[] values)
        {
            if (Communication.Instance.Socket == null)
                ConnectedToServer();

            Packet original = new Packet();
            Communication.Instance.Socket.Communication(ref original, typeMessage, values);

            return original;
        }

    }

}
