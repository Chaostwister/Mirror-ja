using UnityEngine;
using Mirror;
using UnityEditor;

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
    public static GameObject NetworkInstantiate(GameObject original, NetworkConnectionToClient conn)
    {
        var instance = GameObject.Instantiate(original);
        instance.transform.position = new Vector3(0,2,0);
        NetworkServer.Spawn(instance, conn);
        return instance;
    }
    
    public static GameObject NetworkInstantiate(GameObject original, NetworkConnectionToClient conn, Vector3 position)
    {
        var instance = GameObject.Instantiate(original);
        instance.transform.position = position;
        NetworkServer.Spawn(instance, conn);
        return instance;
    }

    public static void Y(this Vector3 vec, float newY)
    {
        vec = new Vector3(vec.x, newY, vec.z);
    }
}