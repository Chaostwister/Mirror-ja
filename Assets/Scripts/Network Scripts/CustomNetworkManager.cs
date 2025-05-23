using System.Collections.Generic;
using Mirror;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Serialization;
using static Helper;

namespace Network_Scripts
{
    public class CustomNetworkManager : NetworkManager
    {
        [SerializeField] private List<GameObject> spawns;
        [SerializeField] private GameObject serverCam;

        public GameObject ServerCam => serverCam;

        public override void OnStartServer()
        {
            ItemDatabase.Initialize();

            foreach (var o in spawns)
            {
                NetworkInstantiate(o);
            }
        

            base.OnStartServer();
        }

        public override void OnClientConnect()
        {
            ItemDatabase.Initialize();
        
            ServerCam.SetActive(false);
            base.OnClientConnect();
        }
    }
}