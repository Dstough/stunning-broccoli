using System;
using System.Net.Sockets;
using UnityEngine;

namespace Assets.Scripts.Networking.ClientData
{
    public class Client : MonoBehaviour
    {
        public static Client Instance;

        public string Ip = "127.0.0.1";
        public int Id;
        public ClientTCP Tcp;

        #region Monobehaviour

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            Debug.Log($"Starting Client...");

            InitializeClient();

            Debug.Log($"Client Started.");
        }

        #endregion

        private void InitializeClient()
        {
            Tcp = new ClientTCP();
        }
    }
}
