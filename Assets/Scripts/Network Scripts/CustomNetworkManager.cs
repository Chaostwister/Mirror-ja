using Mirror;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private GameObject ServerCam;

    public override void OnStartServer()
    {
        ItemDatabase.Initialize();
        
        var instance = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Pistol"));
        NetworkServer.Spawn(instance);
        

        base.OnStartServer();
    }

    public override void OnClientConnect()
    {
        ItemDatabase.Initialize();
        
        ServerCam.SetActive(false);
        base.OnClientConnect();
    }
}