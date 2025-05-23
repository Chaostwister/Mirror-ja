using System;
using Items;
using Mirror;
using ScriptableObjects;
using UnityEngine;
using static Helper;

namespace Player
{
    [RequireComponent(typeof(PlayerManager), typeof(Inventory))]
    public class PlayerInventoryController : NetworkBehaviour
    {
        private Inventory inv => GetComponent<Inventory>();
        private PlayerManager PlayerManager => GetComponent<PlayerManager>();
        private PlayerItemController itemController => GetComponent<PlayerItemController>();
        private Transform cam => PlayerManager.Cam;

        [SerializeField] [SyncVar] private int curInvSlot;

        public int CurInvSlot => curInvSlot;

        [SerializeField] private float dropForceMultiplier = 4f;

        public Action ChangedInvSlot;

        // Update is called once per frame
        private void Update()
        {
            if (!isLocalPlayer) return;

            if (Input.GetKeyDown(KeyCode.E) && Physics.Raycast(cam.position, cam.forward, out var hit))
            {
                hit.transform.TryGetComponent(out Item item);
                if (item == null) return;
                CmdPickUpItem(item.gameObject);
            }
            else if (Input.GetKeyDown(KeyCode.Q) && inv.GetItemCount(curInvSlot) > 0)
            {
                CmdDropItem(cam.forward);
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                CmdChangeInvSlot(curInvSlot - 1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                CmdChangeInvSlot(curInvSlot + 1);
            }
        }

        [Command]
        private void CmdChangeInvSlot(int slot)
        {
            ServerChangeInvSlot(slot);
        }

        [Server]
        private void ServerChangeInvSlot(int slot)
        {
            //print("changing inv slot");
            curInvSlot = slot;

            if (curInvSlot < 0) curInvSlot = inv.MaxSlots - 1;
            else if (curInvSlot >= inv.MaxSlots) curInvSlot = 0;

            ChangedInvSlot.Invoke();
        }

        [Command]
        public void CmdPickUpItem(GameObject hit)
        {
            var itemID = hit.GetComponent<Item>().itemData.id;
        
            //when inv is full switch items  
            if (inv.IsFull(itemID))
            {
                print("inv full");
                while (inv.IsFull(itemID))
                {
                    DropItem(transform.forward);
                }
            }
            else curInvSlot = inv.FindSlotForItem(curInvSlot,itemID);

        
            //assign to slot
            inv.SetItemIDs(curInvSlot, itemID);
            inv.SetItemCount(curInvSlot, inv.GetItemCount(curInvSlot) + 1);
            NetworkServer.Destroy(hit);
        
        
            ChangedInvSlot.Invoke();
        }

        [Command]
        public void CmdDropItem(Vector3 dir)
        {
            DropItem(dir);
        }

        [Server]
        private void DropItem(Vector3 dir)
        {
            // var item = NetworkInstantiate(ItemDatabase.GetItem(inv.GetItemIDs(curInvSlot)).prefab, connectionToClient,
            //     cam.position + dir);
            
            itemController.equippedItem.GetComponent<Item>().SetIsEquipped(false);
            itemController.equippedItem.GetComponent<Collider>().enabled = true;
            
            RpcAddForce(itemController.equippedItem, dir * dropForceMultiplier, ForceMode.VelocityChange);
            itemController.equippedItem = null;

            inv.SetItemCount(curInvSlot, inv.GetItemCount(curInvSlot) - 1);
            if (inv.GetItemCount(curInvSlot) < 1) inv.SetItemIDs(curInvSlot, String.Empty);
        
            ChangedInvSlot.Invoke();
        }

        [TargetRpc]
        public void RpcAddForce(GameObject obj, Vector3 force, ForceMode forceMode)
        {
            var rb = obj.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.AddForce(force, forceMode);
        }
    }
}