using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Networking.ServerData
{
    public class ClientUDP
    {
        public IPEndPoint EndPoint;

        private int Id;

        public ClientUDP(int id)
        {
            Id = id;
        }

        public void Connect(IPEndPoint endPoint)
        {
            EndPoint = endPoint;
            ServerSend.UdpTest(Id);
        }

        public void SendData(Packet packet)
        {
            Server.SendUdpData(EndPoint, packet);
        }

        public void HandleData(Packet packetData)
        {
            var packetLength = packetData.ReadInt();
            var packetBytes = packetData.ReadBytes(packetLength);

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (var packet = new Packet(packetBytes))
                {
                    var packetId = packet.ReadInt();
                    Server.PacketHandlers[packetId](Id, packet);
                }
            });
        }
    }
}
