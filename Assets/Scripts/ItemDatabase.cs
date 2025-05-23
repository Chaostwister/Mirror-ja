using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

public static class ItemDatabase
{
    public static readonly Dictionary<string, ItemData> Items = new();

    public static void Initialize()
    {
        var loadedItems = Resources.LoadAll<ItemData>("Items");
        Items.Clear();
        var i = 0;
        foreach (var item in loadedItems)
        {
            if (Items.ContainsKey(item.id))
            {
                Debug.LogWarning($"Duplicate Item ID: {item.id}");
                continue;
            }

            Items[item.id] = item;
            i++;
        }
        
        //Debug.Log($"Initialized {i} Item{(i > 1 ? "s":"")} in Item Database");
    }

    public static ItemData GetItem(string id)
    {
        Items.TryGetValue(id, out var item);
        if (item == null)
        {
            Debug.LogWarning($"Invalid Item ID: {id}");
            return null;
        }
        
        //Debug.Log($"Fetched Item {id} from Database");
        return item;
    }
}