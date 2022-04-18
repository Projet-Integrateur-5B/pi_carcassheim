using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Net.Sockets;

namespace Assets.System
{
    public class Communication : MonoBehaviour
    {
        public static Communication Instance { get; private set; }
        private static Socket Socket;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        public void ConnectedToServer()
        {
            var error_value = Client.Connection(ref Socket, 19000);
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
            if (Socket != null)
                return;

            var error_value = Client.Disconnection(Socket);
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
            if (Socket == null)
                 ConnectedToServer();

            Packet original = new Packet();
            var error_value = Socket.Communication(ref original, typeMessage, values);
            bool res = false;

            switch (error_value)
            {
                case Tools.Errors.None:
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

    }

}
