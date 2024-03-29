﻿using Assets.Scripts.Networking.ClientData;
using Assets.Scripts.Networking.Framework;
using UnityEngine;

namespace Assets.Scripts.Networking
{
    class ClientSend : MonoBehaviour
    {
        #region Core

        public static void SendTcpData(Packet packet)
        {
            packet.WriteLength();
            Client.Instance.Tcp.SendData(packet);
        }
        public static void SendUdpData(Packet packet)
        {
            packet.WriteLength();
            Client.Instance.Udp.SendData(packet);
        }

        #endregion

        #region Packets

        public static void WelcomeRecieved(string userName)
        {
            using (var packet = new Packet((int)PacketTypes.WelcomeReceived))
            {
                packet.Write(Client.Instance.Id);
                packet.Write(userName);

                SendTcpData(packet);
            }
        }

        #endregion
    }
}
