using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public GameObject inventoryPanel; // Reference to the inventory UI panel
    public GameObject itemSlotPrefab; // Prefab for individual item slots

    private List<Item> items = new List<Item>(); // Player's inventory

    // Optionally, you could add variables for layout control (Spacing, Padding, etc.)
    public float itemSlotWidth = 64f;  // Width of each item slot
    public float itemSlotHeight = 64f; // Height of each item slot

    public void AddItem(Item item)
    {
        items.Add(item);
        UpdateInventoryUI();
    }

    private void UpdateInventoryUI()
    {
        // Clear existing items in the inventory panel
        foreach (Transform child in inventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Populate the inventory with current items
        int row = 0;
        int col = 0;

        foreach (Item item in items)
        {
            // Create a new slot for the item
            GameObject itemSlot = Instantiate(itemSlotPrefab, inventoryPanel.transform);

            // Get the Image component of the item slot
            Image iconImage = itemSlot.GetComponentInChildren<Image>(); // Get Image component in the child (which is the Icon slot)

            if (iconImage != null)
            {
                // Set the icon of the item
                iconImage.sprite = item.itemIcon;  // Assign the item icon to the Image component
                iconImage.preserveAspect = true;   // Ensure aspect ratio is maintained
            }

            // Set the size of the item slot
            RectTransform rt = itemSlot.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(itemSlotWidth, itemSlotHeight); // Set size of the item slot

            // Position the item slot within the inventory panel
            rt.anchoredPosition = new Vector2(col * (itemSlotWidth + 10), -row * (itemSlotHeight + 10));

            // Update column and row for next slot
            col++;
            if (col >= 5)  // Example: Max items per row (adjust this as needed)
            {
                col = 0;
                row++;
            }
        }

        // Ensure the InventoryPanel resizes based on content
        RectTransform inventoryPanelRect = inventoryPanel.GetComponent<RectTransform>();
        float totalHeight = (row + 1) * (itemSlotHeight + 10);  // Height for the total number of rows
        inventoryPanelRect.sizeDelta = new Vector2(inventoryPanelRect.sizeDelta.x, totalHeight);
    }

}
