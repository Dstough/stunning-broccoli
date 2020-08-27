namespace Assets.Scripts.Networking.Framework
{
    public interface IPacket
    {
        PacketTypes PacketType { get; set; }
    }
}