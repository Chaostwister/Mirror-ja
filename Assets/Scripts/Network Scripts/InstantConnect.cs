using Mirror;
using Mirror.Discovery;
using UnityEngine;

namespace Network_Scripts
{
    [RequireComponent(typeof(NetworkDiscovery))]
    public class InstantConnect : MonoBehaviour
    {
        private NetworkDiscovery networkDiscovery => GetComponent<NetworkDiscovery>();

        private void Start()
        {
            networkDiscovery.OnServerFound.AddListener(ConnectToFoundServer);
        }

        public void ConnectToFoundServer(ServerResponse info)
        {
            networkDiscovery.StopDiscovery();
            NetworkManager.singleton.StartClient(info.uri);
        }
    }
}
