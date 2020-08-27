using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Assets.Scripts.Networking.Framework;

namespace Assets.Scripts.Networking.ServerData
{
    public class Server : MonoBehaviour
    {
        public static Server Instance;
        public static Dictionary<int, ClientObject> Slots { get; set; }
        
        private static TcpListener TcpListener { get; set; }
        private static UdpClient UdpListener { get; set; }

        public delegate void PacketHandler(int Id, Packet packet);
        public static Dictionary<int, PacketHandler> PacketHandlers;

        #region Monobehaviour

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);

            DontDestroyOnLoad(transform.gameObject);
        }

        private void Start()
        {
            Debug.Log($"Starting Server...");

            InitalizeServer();

            Debug.Log($"Server started on port {Config.PORT}.");
        }

        private void OnApplicationQuit()
        {
            ShutdownServer();
        }

        #endregion

        private static void InitalizeServer()
        {
            Slots = new Dictionary<int, ClientObject>();

            ServerHandle.InitializeServerPacketHandlers();

            for (var i = 1; i <= Config.MAX_PLAYERS; i++)
                Slots.Add(i, new ClientObject(i));

            TcpListener = new TcpListener(IPAddress.Any, Config.PORT);
            TcpListener.Start();
            TcpListener.BeginAcceptTcpClient(new AsyncCallback(TcpConnectCallback), null);

            UdpListener = new UdpClient(Config.PORT);
            UdpListener.BeginReceive(UdpReceiveCallback, null);
        }

        private void ShutdownServer()
        {
            TcpListener.Stop();
            UdpListener.Close();

            foreach(var client in Slots)
                if(client.Value != null)
                    client.Value.Disconnect();
            
            TcpListener = null;
            UdpListener = null;
        }

        private static void UdpReceiveCallback(IAsyncResult result)
        {
            try
            {
                var clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                var data = UdpListener.EndReceive(result, ref clientEndPoint);

                UdpListener.BeginReceive(UdpReceiveCallback, null);

                if (data.Length < 4)
                    return;
                
                using (var packet = new Packet(data))
                {
                    var clientId = packet.ReadInt();

                    if (clientId == 0)
                        return;

                    if (Slots[clientId].Udp.EndPoint == null)
                    {
                        Slots[clientId].Udp.Connect(clientEndPoint);
                        return;
                    }

                    if (Slots[clientId].Udp.EndPoint.ToString() == clientEndPoint.ToString())
                        Slots[clientId].Udp.HandleData(packet);
                }
            }
            catch (ObjectDisposedException)
            {
                // Ignored
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error receiving UDP data: {ex}");
            }
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

        public static void SendUdpData(IPEndPoint clientEndPoint, Packet packet)
        {
            try
            {
                if (clientEndPoint == null)
                    return;

                UdpListener.BeginSend(packet.ToArray(), packet.Length(), clientEndPoint, null, null);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error sending data to {clientEndPoint} via UDP: {ex}");
            }
        }
    }
}