using UnityEngine;
using Mirror;

public static class Helper
{
    public static GameObject NetworkInstantiate(GameObject original)
    {
        var instant = GameObject.Instantiate(original);
        NetworkServer.Spawn(instant);
        return instant;
    }
}