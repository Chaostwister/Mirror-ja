using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using static System.String;
using static Helper;

[RequireComponent(typeof(PlayerInventoryController), typeof(Inventory))]
public class PlayerItemController : NetworkBehaviour
{
    private PlayerInventoryController playerInvContr => GetComponent<PlayerInventoryController>();
    private Inventory inv => GetComponent<Inventory>();

    [SerializeField] private Transform connectionPoint;
    [SerializeField] [SyncVar] private GameObject equippedItem;

    private void OnEnable()
    {
        print($"on enable ran on{(isClientOnly ? "Client" : "Server")}");
        
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

        if (Input.GetMouseButton(0))
        {
            item.OnLeftClick();
        }
    }
    
    [Server]
    private void ServerOnSlotChange()
    {
        print("changed slot");
        
        if(equippedItem != null) NetworkServer.Destroy(equippedItem);

        if (inv.GetItemIDs(playerInvContr.CurInvSlot) == Empty) return;
       equippedItem = NetworkInstantiate(ItemDatabase.GetItem(inv.GetItemIDs(playerInvContr.CurInvSlot)).prefab, connectionToClient);
       equippedItem.GetComponent<Item>().IsEquipped = true;
       
       RpcOnNewEquipped(equippedItem);
    }

    [ClientRpc]
    private void RpcOnNewEquipped(GameObject equipped)
    {
        equipped.GetComponent<Collider>().enabled = false;
    }
}