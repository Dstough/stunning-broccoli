using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }
    }
}
