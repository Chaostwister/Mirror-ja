using Mirror;
using Player;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Items
{
    
    [RequireComponent(typeof(NetworkIdentity), typeof(NetworkTransformUnreliable), typeof(NetworkRigidbodyUnreliable))]
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Item: NetworkBehaviour
    {
        [SerializeField] public ItemData itemData;

        [ReadOnly] [SerializeField][SyncVar] private bool IsEquipped;
        public Rigidbody rb => GetComponent<Rigidbody>();

        public override void OnStartAuthority()
        {
            rb.isKinematic = IsEquipped;
        }

        public virtual void OnEquip(PlayerItemController itemController)
        {
            //default impl
        }
        public abstract void WhileEquipped();

        public void SetIsEquipped(bool isEquipped)
        {
            IsEquipped = isEquipped;
        }
    }
}
