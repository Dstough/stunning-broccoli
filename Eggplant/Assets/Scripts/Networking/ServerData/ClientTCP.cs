﻿using System;
using System.Net.Sockets;
using UnityEngine;

namespace Assets.Scripts.Networking.ServerData
{
    public class ClientTCP
    {
        public TcpClient Socket;
        public int Id;

        private NetworkStream Stream { get; set; }
        private byte[] ReceiveBuffer { get; set; }

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

            Stream.BeginRead(ReceiveBuffer, 0, Config.DATA_BUFFER_SIZE, ReceiveCallback, null);

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