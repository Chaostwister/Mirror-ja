using System;
using Mirror;
using Mirror.Discovery;
using UnityEngine;

public abstract class Item : NetworkBehaviour
{
    [SerializeField] public ItemData data;

    [SerializeField][SyncVar] public bool IsEquipped;
    private Rigidbody rb => GetComponent<Rigidbody>();

    public override void OnStartAuthority()
    {
        rb.isKinematic = IsEquipped;
    }

    public abstract void OnLeftClick();
    public abstract void OnRightClick();

}
