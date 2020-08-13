using System;
using System.Net.Sockets;
using UnityEngine;

namespace Assets.Scripts.Networking.ClientData
{
    public class ClientTCP
    {
        public TcpClient Socket;

        private NetworkStream Stream;
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

                //TODO: Handle the data

                Stream.BeginRead(ReceiveBuffer, 0, Config.DATA_BUFFER_SIZE, ReceiveCallback, null);
            }
            catch (Exception ex)
            {
                Debug.Log($"Error receiving TCP data: {ex}");
                //TODO: Disconnect;
            }
        }
    }
}
