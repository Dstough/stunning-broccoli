using Assets.Scripts.Networking.Framework;
using System;

namespace Assets.Scripts.Networking.Packets
{
    [Serializable]
    class Welcome : IPacket
    {
        public PacketTypes PacketType { get; set; }

        public Welcome()
        {
            PacketType = PacketTypes.Welcome;
        }
    }
}
