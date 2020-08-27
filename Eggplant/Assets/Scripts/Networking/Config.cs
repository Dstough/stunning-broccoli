namespace Assets.Scripts.Networking
{
    class Config
    {
        public const int DATA_BUFFER_SIZE = 4096;
        public const int MAX_PLAYERS = 8;
        public const int PORT = 451;
    }
    
    public enum PacketTypes
    {
        Welcome,
        WelcomeReceived,
        UdpTest
    }
}
