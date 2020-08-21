using System;
using System.Net.Sockets;
using UnityEngine;

namespace Assets.Scripts.Networking.ClientData
{
    public class ClientTCP
    {
        public TcpClient Socket;

        private NetworkStream Stream;
        private Packet RecievedData;
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
            RecievedData = new Packet();
            Stream.BeginRead(ReceiveBuffer, 0, Config.DATA_BUFFER_SIZE, ReceiveCallback, null);
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                var length = Stream.EndRead(result);

                if (length <= 0)
                    throw new Exception("No byte data was read in the packet;");

                var data = new byte[length];

                Array.Copy(ReceiveBuffer, data, length);

                RecievedData.Reset(HandleData(data));
                
                Stream.BeginRead(ReceiveBuffer, 0, Config.DATA_BUFFER_SIZE, ReceiveCallback, null);
            }
            catch (Exception ex)
            {
                Debug.Log($"Error receiving TCP data: {ex}");
                //TODO: Disconnect;
            }
        }

        public void SendData(Packet packet)
        {
            try
            {
                if(Socket != null)
                {
                    Stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                }
            }
            catch(Exception ex)
            {
                Debug.LogError($"Error sending data to server via TCP: {ex}");
            }
        }

        private bool HandleData(byte[] data)
        {
            var packetLength = 0;

            RecievedData.SetBytes(data);

            if (RecievedData.UnreadLength() >= 4)
            {
                packetLength = RecievedData.ReadInt();

                if (packetLength <= 0)
                    return true;
            }

            while(packetLength > 0 && packetLength <= RecievedData.UnreadLength())
            {
                var packetBytes = RecievedData.ReadBytes(packetLength);

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (var packet = new Packet(packetBytes))
                    {
                        var packetId = packet.ReadInt();
                        Client.PacketHandlers[packetId](packet);
                    }
                });

                packetLength = 0;

                if (RecievedData.UnreadLength() >= 4)
                {
                    packetLength = RecievedData.ReadInt();

                    if (packetLength <= 0)
                        return true;
                }
            }

            if (packetLength <= 1)
                return true;

            return false;
        }
    }
}
