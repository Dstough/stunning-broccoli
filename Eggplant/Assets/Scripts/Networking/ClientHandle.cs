using System.Collections.Generic;
using System.Net;
using Assets.Scripts.Networking.ClientData;
using Assets.Scripts.Networking.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Networking
{
    class ClientHandle : MonoBehaviour
    {
        public static void InitializeClientPacketHandlers()
        {
            Client.PacketHandlers = new Dictionary<int, Client.PacketHandler>
            {
                {(int)PacketTypes.Welcome, Welcome }
            };
        }

        public static void Welcome(Packet packet)
        {
            var message = packet.ReadString();
            var id = packet.ReadInt();

            Client.Instance.Id = id;

            Debug.Log($"{message}");

            var name = GameObject.Find("Username").GetComponent<InputField>().text;

            ClientSend.WelcomeRecieved(name);

            Client.Instance.Udp.Connect(((IPEndPoint)Client.Instance.Tcp.Socket.Client.LocalEndPoint).Port);

            SceneManager.LoadScene("Chat Room");
        }
    }
}
