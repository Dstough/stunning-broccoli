using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Assets.Scripts.Networking.Framework;
using UnityEngine;
using UnityEngine.UI;

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

        private bool isConnected = false;

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

            ConnectToServer();

            Debug.Log($"Connected to Server.");
        }

        private void OnApplicationQuit()
        {
            Disconnect();
        }

        #endregion

        #region Internals

        public class ClientTCP
        {
            public TcpClient Socket;

            private NetworkStream Stream;
            private Packet ReceivedData;
            private byte[] ReceiveBuffer;


            public void Connect()
            {
                Socket = new TcpClient
                {
                    ReceiveBufferSize = Config.DATA_BUFFER_SIZE,
                    SendBufferSize = Config.DATA_BUFFER_SIZE
                };

                ReceiveBuffer = new byte[Config.DATA_BUFFER_SIZE];
                Socket.BeginConnect(Client.Instance.Ip, Config.PORT, ConnectCallback, Socket);
            }

            private void ConnectCallback(IAsyncResult result)
            {
                Socket.EndConnect(result);

                if (!Socket.Connected)
                    return;

                Stream = Socket.GetStream();
                ReceivedData = new Packet();
                Stream.BeginRead(ReceiveBuffer, 0, Config.DATA_BUFFER_SIZE, ReceiveCallback, null);
            }

            private void ReceiveCallback(IAsyncResult result)
            {
                try
                {
                    var length = Stream.EndRead(result);

                    if (length <= 0)
                    {
                        Instance.Disconnect();
                        return;
                    }

                    var data = new byte[length];

                    Array.Copy(ReceiveBuffer, data, length);

                    ReceivedData.Reset(HandleData(data));

                    Stream.BeginRead(ReceiveBuffer, 0, Config.DATA_BUFFER_SIZE, ReceiveCallback, null);
                }
                catch (Exception ex)
                {
                    Debug.Log($"Error receiving TCP data: {ex}");
                    Disconnect();
                }
            }

            public void SendData(Packet packet)
            {
                try
                {
                    if (Socket != null)
                    {
                        Stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error sending data to server via TCP: {ex}");
                }
            }

            private bool HandleData(byte[] data)
            {
                var packetLength = 0;

                ReceivedData.SetBytes(data);

                if (ReceivedData.UnreadLength() >= 4)
                {
                    packetLength = ReceivedData.ReadInt();

                    if (packetLength <= 0)
                        return true;
                }

                while (packetLength > 0 && packetLength <= ReceivedData.UnreadLength())
                {
                    var packetBytes = ReceivedData.ReadBytes(packetLength);

                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (var packet = new Packet(packetBytes))
                        {
                            var packetId = packet.ReadInt();
                            Client.PacketHandlers[packetId](packet);
                        }
                    });

                    packetLength = 0;

                    if (ReceivedData.UnreadLength() >= 4)
                    {
                        packetLength = ReceivedData.ReadInt();

                        if (packetLength <= 0)
                            return true;
                    }
                }

                if (packetLength <= 1)
                    return true;

                return false;
            }

            private void Disconnect()
            {
                Instance.Disconnect();

                Stream = null;
                ReceivedData = null;
                ReceiveBuffer = null;
                Socket = null;
            }
        }
        public class ClientUDP
        {
            public UdpClient Socket;
            public IPEndPoint EndPoint;

            public ClientUDP()
            {
                EndPoint = new IPEndPoint(IPAddress.Parse(Client.Instance.Ip), Config.PORT);
            }

            public void Connect(int localPort)
            {
                Socket = new UdpClient(localPort);
                Socket.Connect(EndPoint);
                Socket.BeginReceive(ReceiveCallback, null);

                using (var packet = new Packet())
                {
                    SendData(packet);
                }
            }

            public void SendData(Packet packet)
            {
                try
                {
                    packet.InsertInt(Client.Instance.Id);
                    if (Socket != null)
                    {
                        Socket.BeginSend(packet.ToArray(), packet.Length(), null, null);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error sending UDP data: {ex}");
                }
            }

            private void ReceiveCallback(IAsyncResult result)
            {
                try
                {
                    var data = Socket.EndReceive(result, ref EndPoint);

                    Socket.BeginReceive(ReceiveCallback, null);

                    if (data.Length < 4)
                    {
                        Instance.Disconnect();
                        return;
                    }

                    HandleData(data);
                }
                catch(ObjectDisposedException)
                {
                    //Ignored;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error handling packet data: {ex}");
                    Disconnect();
                }
            }

            private void HandleData(byte[] data)
            {
                using (var packet = new Packet(data))
                {
                    var packetLength = packet.ReadInt();
                    data = packet.ReadBytes(packetLength);
                }

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (var packet = new Packet(data))
                    {
                        var packetId = packet.ReadInt();

                        Client.PacketHandlers[packetId](packet);
                    }
                });
            }

            private void Disconnect()
            {
                Instance.Disconnect();

                EndPoint = null;
                Socket = null;
            }
        }
        
        #endregion

        private void InitializeClient()
        {
            ClientHandle.InitializeClientPacketHandlers();

            Ip = GameObject.Find("Host").GetComponent<InputField>().text;
        }
        
        public void ConnectToServer()
        {
            Tcp = new ClientTCP();
            Udp = new ClientUDP();
            
            isConnected = true;
            Instance.Tcp.Connect();
        }

        private void Disconnect()
        {
            if (!isConnected)
                return;

            isConnected = false;
            Tcp.Socket.Close();
            Udp.Socket.Close();

            Debug.Log("Disconnected from server.");
        }
    }
}
