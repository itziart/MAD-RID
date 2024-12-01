using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragHandler : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public Image icon;        // Reference to the item icon for visual representation
    public ItemData itemData; // Reference to the ScriptableObject that holds item data
    private Vector3 originalLocalPosition; // Original position of the item in its parent
    private Canvas canvas; // Reference to the canvas this item is part of
    private RectTransform rectTransform; // RectTransform of the item, used for positioning and manipulation
    private Inventory inventory; // Reference to the Inventory where the item is stored
    private RectTransform parentRectTransform; // RectTransform of the parent object (ItemSlot)
    private Vector3 originalWorldPosition; // Original world position of the item when dragging starts

    private ItemUI itemUI; // Reference to the ItemUI component from the parent (ItemSlot)

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>(); // Get the RectTransform component
        canvas = GetComponentInParent<Canvas>(); // Get the parent Canvas for proper UI handling
        inventory = FindObjectOfType<Inventory>(); // Find the Inventory in the scene
        parentRectTransform = rectTransform.parent.GetComponent<RectTransform>(); // Get the RectTransform of the parent (ItemSlot)
        originalWorldPosition = rectTransform.position; // Save the original world position

        // Fetch the ItemUI component from the parent (ItemSlot)
        itemUI = GetComponentInParent<ItemUI>();
    }

    // Called when the user begins dragging the item
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemUI != null)
        {
            itemData = itemUI.GetItemData(); // Assign the item data from the parent ItemUI
            Debug.Log($"Item Data assigned: {itemData?.itemName}"); // Log the item name for debugging
        }
        else
        {
            Debug.LogError("ItemUI component is missing on the dragged item!"); // Log an error if the ItemUI is missing
        }
    }

    // Called during the drag to update the position of the item
    public void OnDrag(PointerEventData eventData)
    {
        Vector3 worldPosition = Input.mousePosition; // Get the current world position of the mouse
        Vector3 localPosition = parentRectTransform.InverseTransformPoint(worldPosition); // Convert world position to local position relative to the parent
        rectTransform.localPosition = localPosition; // Update the item’s position to follow the mouse
    }

    // Called when the drag ends (mouse release)
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"End drag for item: {itemData?.itemName}"); // Log the end of the drag for the item

        // Check if the item was dropped on a valid interactable object
        Collider2D hit = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(eventData.position)); // Detect the object under the pointer

        if (hit != null)
        {
            // Attempt to interact with any InteractableObject the item was dropped on
            InteractableObject interactable = hit.GetComponent<InteractableObject>();
            if (interactable != null)
            {
                if (interactable.Interact(itemData)) // If interaction is successful
                {
                    Destroy(gameObject); // Destroy the dragged item after successful interaction
                    return; // Exit the method
                }
            }
        }

        // If no valid interaction occurred, return the item to the inventory
        ReplaceItemInInventory();
    }

    // Method to return the item to its original position in the inventory if no valid interaction occurs
    private void ReplaceItemInInventory()
    {
        Debug.Log("Dropped outside of a valid target. Returning item to original position."); // Log the action of returning the item

        // Instantiate a new item slot in the inventory panel
        GameObject itemSlot = Instantiate(inventory.itemSlotPrefab, inventory.inventoryPanel);

        ItemUI itemUI = itemSlot.GetComponent<ItemUI>(); // Get the ItemUI component of the newly created item slot
        if (itemUI != null)
        {
            if (itemData != null)
            {
                itemUI.Initialize(itemData); // Initialize the item slot with the dragged item's data
            }
            else
            {
                Debug.LogError("ItemData is null in DragHandler when returning item to inventory."); // Log an error if itemData is missing
            }
        }
        else
        {
            Debug.LogError("ItemUI component missing on the item slot prefab!"); // Log an error if ItemUI is missing
        }

        Destroy(gameObject); // Destroy the dragged item object
        LayoutRebuilder.ForceRebuildLayoutImmediate(inventory.inventoryPanel.GetComponent<RectTransform>()); // Rebuild layout to ensure the new item is properly positioned
    }
}
