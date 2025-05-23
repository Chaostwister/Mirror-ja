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
        [SerializeField] [SyncVar] private GameObject equippedItem;
        

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
        
            if(equippedItem != null) NetworkServer.Destroy(equippedItem);

            if (inv.GetItemIDs(playerInvContr.CurInvSlot) == Empty) return;
            equippedItem = NetworkInstantiate(ItemDatabase.GetItem(inv.GetItemIDs(playerInvContr.CurInvSlot)).prefab, connectionToClient);
            equippedItem.GetComponent<Item>().SetIsEquipped(true);
            
            if(equippedItem == null) print("null huh what");
            RpcOnNewEquipped(equippedItem);
            TargetRpcSetupItem(equippedItem);
        }

        [ClientRpc]
        private void RpcOnNewEquipped(GameObject equipped)
        {
            if (equipped == null)
            {
                print("equipped null for some f reason");
                return;
            }
            equipped.GetComponent<Collider>().enabled = false;
        }

        [TargetRpc]
        private void TargetRpcSetupItem(GameObject equipped)
        {
            equipped.GetComponent<Item>().OnEquip(this);
        }
    }
}