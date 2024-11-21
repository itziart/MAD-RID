using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour 
{
    public List<GameObject> items = new List<GameObject>();

    public void AddItem(GameObject item)
    {
        items.Add(item);
        Debug.Log($"Picked up: {item.name}");
    }

    public bool HasItem(GameObject item)
    {
        return items.Contains(item);
    }

    public void RemoveItem(GameObject item)
    {
        if (items.Contains(item)) items.Remove(item);
    }
}