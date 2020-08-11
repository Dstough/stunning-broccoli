using System;
using System.Net.Sockets;
using UnityEngine;

namespace Assets.Scripts.Networking.ServerData
{
    public class ClientTCP
    {
        public static int DataBufferSize { get; } = 4096;
        public TcpClient Socket { get; set; }
        public int Id { get; }

        private NetworkStream Stream { get; set; }
        private byte[] ReceiveBuffer { get; set; }

        public ClientTCP(int id)
        {
            Id = id;
        }

        public void Connect(TcpClient socket)
        {
            Socket = socket;
            Socket.ReceiveBufferSize = DataBufferSize;
            Socket.SendBufferSize = DataBufferSize;

            Stream = Socket.GetStream();
            ReceiveBuffer = new byte[DataBufferSize];

            Stream.BeginRead(ReceiveBuffer, 0, DataBufferSize, ReceiveCallback, null);

            //TODO: Send welcome packet;
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

                Stream.BeginRead(ReceiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
            }
            catch (Exception ex)
            {
                Debug.Log($"Error receiving TCP data: {ex}");
                //TODO: Disconnect;
            }
        }
    }
}
