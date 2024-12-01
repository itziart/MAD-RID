using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject itemSlotPrefab; // Prefab for the item slot UI
    public Transform inventoryPanel; // Panel where items will be displayed
    private List<ItemData> items = new List<ItemData>(); // List of inventory items

    // Add an item to the inventory
    public void AddItem(ItemData itemData)
    {
        if (itemData == null)
        {
            Debug.LogError("Cannot add item to inventory: ItemData is null.");
            return;
        }

        // Instantiate the ItemSlotPrefab in the inventory panel
        GameObject itemSlot = Instantiate(itemSlotPrefab, inventoryPanel);

        // Initialize the ItemUI component
        ItemUI itemUI = itemSlot.GetComponent<ItemUI>();
        if (itemUI != null)
        {
            if (itemData.itemIcon != null)
            {
                itemUI.Initialize(itemData); // Pass the ItemData to the UI
                Debug.Log($"Adding ItemData to Inventory: {itemData.itemName}, Icon: {itemData.itemIcon.name}");
            }
            else
            {
                Debug.LogError($"ItemIcon is missing for {itemData.itemName}");
            }
        }
        else
        {
            Debug.LogError("ItemUI component is missing on ItemSlotPrefab.");
        }
    }





    public void RemoveItem(Item item)
    {
        // Remove the item from the internal list
        if (items.Remove(item.itemData))
        {
            // Find the corresponding item UI in the panel
            foreach (Transform child in inventoryPanel)
            {
                ItemUI itemUI = child.GetComponent<ItemUI>();
                if (itemUI != null && itemUI.GetItemData() == item.itemData)
                {
                    // Destroy the item's UI and stop searching
                    Destroy(child.gameObject);
                    break;
                }
            }
        }
    }

    public void ClearInventory()
    {
        // Clear all items from the internal list
        items.Clear();

        // Destroy all children of the inventory panel
        foreach (Transform child in inventoryPanel)
        {
            Destroy(child.gameObject);
        }
    }
}