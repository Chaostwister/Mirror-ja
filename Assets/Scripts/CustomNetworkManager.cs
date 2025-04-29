using System;
using System.Collections.Generic;
using Mirror;
using Telepathy;
using UnityEngine;
using static Helper;

public class CustomNetworkManager : NetworkManager
{
    private List<GameObject> publicObjects = new();
    [SerializeField] private GameObject ServerCam;

    public override void OnStartServer()
    {
        var item = NetworkInstantiate(spawnPrefabs.Find(prefab => prefab.name == "item"));
        publicObjects.Add(item);

        base.OnStartServer();
    }

    public override void OnClientConnect()
    {
        ServerCam.SetActive(false);
        base.OnClientConnect();
    }
}