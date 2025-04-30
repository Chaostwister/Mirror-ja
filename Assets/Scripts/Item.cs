using System;
using Mirror;
using Mirror.Discovery;
using UnityEngine;

public class Item : NetworkBehaviour
{
    [SerializeField] public ItemData data;
    private Rigidbody rb => GetComponent<Rigidbody>();
}
