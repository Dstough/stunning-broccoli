using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Assets.Scripts.Networking.ServerData
{
    public class Server : MonoBehaviour
    {
        public static int MaxPlayers { get; set; }
        public static int Port { get; set; }
        public static Dictionary<int, ClientObject> Slots { get; set; }

        private static TcpListener TcpListener { get; set; }

        void Start()
        {
            DontDestroyOnLoad(transform.gameObject);

            MaxPlayers = 6;
            Port = 451;

            Debug.Log($"Starting Server...");

            InitalizeServerData();

            Debug.Log($"Server started on port {Port}.");
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
                    return;
                }
            }

            Debug.Log($"{client.Client.RemoteEndPoint} failed to connect: Server full.");
        }

        private static void InitalizeServerData()
        {
            Slots = new Dictionary<int, ClientObject>();

            for (var i = 1; i <= MaxPlayers; i++)
                Slots.Add(i, new ClientObject(i));            
            
            TcpListener = new TcpListener(IPAddress.Any, Port);
            TcpListener.Start();
            TcpListener.BeginAcceptTcpClient(new AsyncCallback(TcpConnectCallback), null);
        }
    }
}
