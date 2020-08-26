using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Assets.Scripts.Networking.ClientData
{
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
                    //TODO: disconnect
                    return;
                }

                HandleData(data);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error handling packet data: {ex}");
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
    }
}
