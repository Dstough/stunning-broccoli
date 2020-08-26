using System.Net;
using UnityEngine;

namespace Assets.Scripts.Networking.ClientData
{
    class ClientHandle : MonoBehaviour
    {
        public static void Welcome(Packet packet)
        {
            var message = packet.ReadString();
            var id = packet.ReadInt();

            Client.Instance.Id = id;

            Debug.Log($"{message}");

            ClientSend.WelcomeRecieved();

            Client.Instance.Udp.Connect(((IPEndPoint)Client.Instance.Tcp.Socket.Client.LocalEndPoint).Port);
        }

        public static void UdpTest(Packet packet)
        {
            var message = packet.ReadString();

            Debug.Log($"{message}");

            ClientSend.UpdTest();
        }
    }
}
