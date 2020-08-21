using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Assets.Scripts.Networking.ServerData
{
    public class Server : MonoBehaviour
    {
        public static Server Instance;
        public static Dictionary<int, ClientObject> Slots { get; set; }
        public delegate void PacketHandler(int Id, Packet packet);
        public static Dictionary<int, PacketHandler> PacketHandlers;

        private static TcpListener TcpListener { get; set; }

        #region Monobehaviour

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);

            DontDestroyOnLoad(transform.gameObject);
        }

        void Start()
        {
            Debug.Log($"Starting Server...");

            InitalizeServerData();

            Debug.Log($"Server started on port {Config.PORT}.");
        }

        #endregion

        private static void InitalizeServerData()
        {
            Slots = new Dictionary<int, ClientObject>();

            for (var i = 1; i <= Config.MAX_PLAYERS; i++)
                Slots.Add(i, new ClientObject(i));

            PacketHandlers = new Dictionary<int, PacketHandler> 
            {
                { (int)PacketTypes.WelcomeReceived, ServerHandle.WelcomeRecieved }
            };
            
            TcpListener = new TcpListener(IPAddress.Any, Config.PORT);
            TcpListener.Start();
            TcpListener.BeginAcceptTcpClient(new AsyncCallback(TcpConnectCallback), null);
        }

        private static void TcpConnectCallback(IAsyncResult result)
        {
            var client = TcpListener.EndAcceptTcpClient(result);

            TcpListener.BeginAcceptTcpClient(new AsyncCallback(TcpConnectCallback), null);

            Debug.Log($"Incoming connection from {client.Client.RemoteEndPoint}...");

            foreach (var slot in Slots) 
            {
                if (slot.Value.Tcp.Socket == null)
                {
                    slot.Value.Tcp.Connect(client);

                    Debug.Log($"Client {client.Client.RemoteEndPoint} connected.");

                    return;
                }
            }

            Debug.Log($"{client.Client.RemoteEndPoint} failed to connect: Server full.");
        }
    }
}