using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Networking.ServerData
{
    class ServerSend
    {
        #region Core

        public static void SendTcpData(int toClient, object data)
        {

        }
        public static void SendTcpData(int toClient, Packet packet)
        {
            packet.WriteLength();
            Server.Slots[toClient].Tcp.SendData(packet);
        }

        public static void SendTcpDataToAll(object data)
        {

        }
        public static void SendTcpDataToAll(Packet packet)
        {
            packet.WriteLength();

            foreach (var slot in Server.Slots)
                slot.Value.Tcp.SendData(packet);
        }

        public static void SendTcpDataToAll(int exceptClient, object data)
        {

        }
        public static void SendTcpDataToAll(int exceptClient, Packet packet)
        {
            foreach (var slot in Server.Slots.Where(slot => slot.Key != exceptClient))
                slot.Value.Tcp.SendData(packet);
        }

        #endregion

        #region messages

        public static void Welcome(int toClient, string message)
        {
            using (var packet = new Packet((int)PacketTypes.Welcome))
            {
                packet.Write(message);
                packet.Write(toClient);

                SendTcpData(toClient, packet);
            }
        }

        #endregion
    }
}