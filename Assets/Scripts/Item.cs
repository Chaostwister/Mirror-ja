using System;
using Mirror;
using UnityEngine;

public class Item : NetworkBehaviour
{
    private Rigidbody rb => GetComponent<Rigidbody>();

    private void FixedUpdate()
    {
        print(netIdentity.isOwned);
    }
}
