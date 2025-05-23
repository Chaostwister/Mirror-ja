using Items;
using Mirror;
using ScriptableObjects;
using UnityEngine;
using static System.String;
using static Helper;

namespace Player
{
    [RequireComponent(typeof(PlayerInventoryController))]
    public class PlayerItemController : NetworkBehaviour
    {
        public PlayerManager manager => GetComponent<PlayerManager>();
        private PlayerInventoryController playerInvContr => GetComponent<PlayerInventoryController>();
        private Inventory inv => GetComponent<Inventory>();
        public PlayerMovementController movementController => GetComponent<PlayerMovementController>();

        [SerializeField] private Transform connectionPoint;
        [SerializeField] [SyncVar] public GameObject equippedItem;
        

        private void OnEnable()
        {
            playerInvContr.ChangedInvSlot += ServerOnSlotChange;
        
            if(isServer) ServerOnSlotChange();
        }

        private void Update()
        {
            if (!isLocalPlayer) return;

            if (equippedItem == null) return;
        
            equippedItem.transform.position = connectionPoint.position;
            equippedItem.transform.forward = connectionPoint.forward;

            var item = equippedItem.GetComponent<Item>();
        
            item.WhileEquipped();
        }
    
        [Server]
        private void ServerOnSlotChange()
        {
            //print("changed slot");

            if (inv.GetItemIDs(playerInvContr.CurInvSlot) == Empty) return;
            equippedItem = NetworkInstantiate(ItemDatabase.GetItem(inv.GetItemIDs(playerInvContr.CurInvSlot)).prefab, connectionToClient, connectionPoint.position);
            equippedItem.GetComponent<Item>().SetIsEquipped(true);
            
            RpcOnNewEquipped(equippedItem);
            TargetRpcSetupItem(equippedItem);
        }

        [ClientRpc]
        private void RpcOnNewEquipped(GameObject equipped)
        {
            equipped.GetComponent<Collider>().enabled = false;
        }

        [TargetRpc]
        private void TargetRpcSetupItem(GameObject equipped)
        {
            equipped.GetComponent<Item>().OnEquip(this);
        }
    }
}