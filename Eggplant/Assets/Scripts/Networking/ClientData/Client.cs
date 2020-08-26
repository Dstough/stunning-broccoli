using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Networking.ClientData
{
    public class Client : MonoBehaviour
    {
        public static Client Instance;

        public string Ip = "127.0.0.1";
        public int Id;
        public ClientTCP Tcp;
        public ClientUDP Udp;

        public delegate void PacketHandler(Packet packet);
        public static Dictionary<int, PacketHandler> PacketHandlers;

        #region Monobehaviour

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            Debug.Log($"Starting Client...");

            InitializeClient();

            Debug.Log($"Client Started.");

            Debug.Log($"Connecting to server...");

            Instance.Tcp.Connect();

            Debug.Log($"Connected to Server.");
        }

        #endregion

        private void InitializeClient()
        {
            Tcp = new ClientTCP();
            Udp = new ClientUDP();

            PacketHandlers = new Dictionary<int, PacketHandler>
            {
                {(int)PacketTypes.Welcome, ClientHandle.Welcome },
                {(int)PacketTypes.UdpTest, ClientHandle.UdpTest }
            };
        }
    }
}
