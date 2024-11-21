using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Item> items = new List<Item>(); // List to store collected items

    public void AddItem(Item item)
    {
        items.Add(item);
        Debug.Log($"Added {item.itemName} to inventory. Total items: {items.Count}");
    }

    public bool HasItem(string itemName)
    {
        return items.Exists(i => i.itemName == itemName);
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);
        Debug.Log($"Removed {item.itemName} from inventory. Total items: {items.Count}");
    }
}