using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Networking.ServerData
{
    class ServerHandle
    {
        public static void WelcomeRecieved(int fromClient, Packet packet)
        {
            var Id = packet.ReadInt();
            var username = packet.ReadString();

            Debug.Log($"{Server.Slots[Id].Tcp.Socket.Client.RemoteEndPoint} connected successfully and is now player {fromClient}");

            if(fromClient != Id)
            {
                Debug.LogError($"Player \"{username}\" has assumed the wrong client ID: ({Id})");
            }

            //TODO: Send player into game.
        }
    }
}
