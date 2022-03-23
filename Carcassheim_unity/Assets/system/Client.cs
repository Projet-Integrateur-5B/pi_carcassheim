using System;
using System.Net;
using System.Net.Sockets;
using Assets;
using UnityEngine;

public class Client
{
    //private static Packet packet = new();
    public static void StartClient(byte number, string data)
    {
        // Data buffer for incoming data.
        var bytes = new byte[Packet.MaxPacketSize];

        // Connect to a remote device.
        try
        {
            Debug.Log(string.Format("Client is setting up..."));

            // Establish the remote endpoint for the socket.
            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = ipHostInfo.AddressList[0];
            var remoteEP = new IPEndPoint(ipAddress, Packet.PortPrincipale);

            // Create a TCP/IP  socket.
            var sender = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint. Catch any errors.
            try
            {
                sender.Connect(remoteEP);
                Debug.Log(string.Format("Client is connected to {0}", sender.RemoteEndPoint));

                var original = new Packet(false, ipAddress.ToString(), Packet.PortPrincipale, 0, number, 999, data);
                var packets = original.Prepare();

                foreach (var packet in packets)
                {
                    var packetAsBytes = packet.Serialize();
                    bytes = new byte[packetAsBytes.Length];

                    // Send the data through the socket.
                    var bytesSent = sender.Send(packet.Serialize());
                    Debug.Log(string.Format("Sent {0} bytes =>\t" + packet, bytesSent));
                }

                // Receive the response from the remote device.
                var bytesRec = sender.Receive(bytes);
                var packetAsBytes2 = new byte[bytesRec];
                Array.Copy(bytes, packetAsBytes2, bytesRec);
                var recv = Packet.Deserialize(packetAsBytes2);
                Debug.Log(string.Format("Read {0} bytes => permission : {1} \n", bytesRec,recv.Status));

                //Release the socket.
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                Debug.Log(string.Format("Closing connection..."));
            }
            catch (ArgumentNullException ane)
            {
                Debug.Log(string.Format("ArgumentNullException : {0}", ane));
            }
            catch (SocketException se)
            {
                Debug.Log(string.Format("SocketException : {0}", se));
            }
            catch (Exception e)
            {
                Debug.Log(string.Format("Unexpected exception : {0}", e));
            }
        }
        catch (Exception e)
        {
            Debug.Log(string.Format(e.ToString()));
        }
    }

}
