using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using static System.String;

public class Inventory : NetworkBehaviour
{
    [SerializeField] private int maxSlots = 9;

    public int MaxSlots => maxSlots;

    private readonly SyncList<string> itemIDs = new();
    private readonly SyncList<int> itemCount = new();

    [SerializeField] private List<string> ShownItemIDs = new();
    [SerializeField] private List<int> ShownItemCount = new();

    private void Update()
    {
        ShownItemIDs.Clear();
        ShownItemCount.Clear();
        for (int i = 0; i < itemIDs.Count; i++)
        {
            ShownItemIDs.Add(itemIDs[i]);
            ShownItemCount.Add(itemCount[i]);
        }
    }

    public void Start()
    {
        if (!isServer) return;
        ServerSetupInv(maxSlots);
    }

    [Server]
    public void ServerSetupInv(int maxSlots)
    {
        for (var i = 0; i < maxSlots; i++)
        {
            itemIDs.Add("");
            itemCount.Add(0);
        }
    }

    public bool IsFull(string id)
    {
        if (itemIDs.Any(itemID => itemID == Empty))
        {
            return false;
        }

        if (!ItemDatabase.GetItem(id).isStackable) return true;

        return itemIDs.All(itemID => itemID != id);
    }

    public string GetItemIDs(int i)
    {
        print("called GetItemIds");
        if (i < 0 || i > itemIDs.Count)
        {
            Debug.LogError($"GetItemIDs failed bc i:{i} is invalid");
            return null;
        }

        return itemIDs[i];
    }

    public int GetItemCount(int i)
    {
        print("called GetItemCount");
        if (i < 0 || i > itemIDs.Count)
        {
            Debug.LogError($"GetItemCount failed bc i:{i} is invalid");
            return 0;
        }

        return itemCount[i];
    }

    public void SetItemIDs(int i, string value)
    {
        print("called SetItemIDs");
        if (i < 0 || i > itemIDs.Count)
        {
            Debug.LogError($"SetItemIDs failed bc i:{i} is invalid");
            return;
        }

        itemIDs[i] = value;
    }

    public void SetItemCount(int i, int value)
    {
        print("called SetItemCount");
        if (i < 0 || i > itemIDs.Count)
        {
            Debug.LogError($"GetItemCount failed bc i:{i} is invalid");
            return;
        }

        itemCount[i] = value;
    }

    public int FirstClearSlot()
    {
        for (var i = 0; i < MaxSlots; i++)
        {
            if (GetItemIDs(i) == Empty) return i;
        }

        Debug.LogError("Searched for clear slot but inv was full.Pls check for full inv first. returning invalid value");
        return -1;
    }

    public bool HasItem(string itemID, out int slot)
    {
        for (slot = 0; slot < MaxSlots; slot++)
        {
            if (GetItemIDs(slot) == itemID) return true;
        }

        return false;
    }

    public int FindSlotForItem(int curInvSlot, string itemID)
    {
        var itemData = ItemDatabase.GetItem(itemID);
        
        //if item is stackable and we find an existing item slot -> use that slot
        if (itemData.isStackable && HasItem(itemID, out var existingSlot))
        {
            return existingSlot;
        }
        
        //else either use cur slot if clear or find first clear slot
        return GetItemIDs(curInvSlot) == Empty ? curInvSlot: FirstClearSlot();
    }
}