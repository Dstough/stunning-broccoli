using System;
using System.Net.Sockets;
using UnityEngine;

namespace Assets.Scripts.Networking.ServerData
{
    public class ClientTCP
    {
        public TcpClient Socket;
        public int Id;

        private NetworkStream Stream;
        private Packet ReceivedData;
        private byte[] ReceiveBuffer;

        public ClientTCP(int id)
        {
            Id = id;
        }

        public void Connect(TcpClient socket)
        {
            Socket = socket;
            Socket.ReceiveBufferSize = Config.DATA_BUFFER_SIZE;
            Socket.SendBufferSize = Config.DATA_BUFFER_SIZE;

            Stream = Socket.GetStream();
            ReceiveBuffer = new byte[Config.DATA_BUFFER_SIZE];
            ReceivedData = new Packet();

            Stream.BeginRead(ReceiveBuffer, 0, Config.DATA_BUFFER_SIZE, ReceiveCallback, null);

            ServerSend.Welcome(Id, "Welcome to the server");
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

                ReceivedData.Reset(HandleData(data));

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
                Debug.LogError($"Error sending data to player {Id} via TCP: {ex}");
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
                        Server.PacketHandlers[packetId](Id,packet);
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
    }
}
