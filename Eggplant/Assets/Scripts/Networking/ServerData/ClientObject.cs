using System;
using System.Net.Sockets;
using UnityEngine;

namespace Assets.Scripts.Networking.ServerData
{
    public class ClientObject
    {
        public int Id { get; set; }
        public ClientTCP Tcp { get; set; }

        public ClientObject(int id)
        {
            Id = id;
            Tcp = new ClientTCP(id);
        }
    }
}