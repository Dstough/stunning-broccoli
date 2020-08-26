namespace Assets.Scripts.Networking.ServerData
{
    public class ClientObject
    {
        public int Id;
        public ClientTCP Tcp;
        public ClientUDP Udp;

        public ClientObject(int id)
        {
            Id = id;
            Tcp = new ClientTCP(id);
            Udp = new ClientUDP(id);
        }
    }
}