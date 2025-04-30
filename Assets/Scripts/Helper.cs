using UnityEngine;
using Mirror;

public static class Helper
{
    public static GameObject NetworkInstantiate(GameObject original)
    {
        var instance = GameObject.Instantiate(original);
        NetworkServer.Spawn(instance);
        return instance;
    }
    
    public static GameObject NetworkInstantiate(GameObject original, Vector3 position)
    {
        var instance = GameObject.Instantiate(original);
        instance.transform.position = position;
        NetworkServer.Spawn(instance);
        return instance;
    }
}