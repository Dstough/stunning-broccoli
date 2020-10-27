using System.Linq;
using Assets.Scripts.Networking.Framework;

namespace Assets.Scripts.Networking.ServerData
{
    class ServerSend
    {
        #region Core

        public static void SendTcpData(int toClient, Packet packet)
        {
            packet.WriteLength();
            Server.Slots[toClient].Tcp.SendData(packet);
        }
        public static void SendUdpData(int toClient, Packet packet)
        {
            packet.WriteLength();
            Server.Slots[toClient].Udp.SendData(packet);
        }

        public static void SendTcpDataToAll(Packet packet)
        {
            packet.WriteLength();

            foreach (var slot in Server.Slots)
                slot.Value.Tcp.SendData(packet);
        }
        public static void SendUdpDataToAll(Packet packet)
        {
            packet.WriteLength();

            foreach (var slot in Server.Slots)
                slot.Value.Udp.SendData(packet);
        }

        public static void SendTcpDataToAll(int exceptClient, Packet packet)
        {
            packet.WriteLength();

            foreach (var slot in Server.Slots.Where(slot => slot.Key != exceptClient))
                slot.Value.Tcp.SendData(packet);
        }
        public static void SendUdpDataToAll(int exceptClient, Packet packet)
        {
            packet.WriteLength();

            foreach (var slot in Server.Slots.Where(slot => slot.Key != exceptClient))
                slot.Value.Udp.SendData(packet);
        }

        #endregion

        #region packets

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