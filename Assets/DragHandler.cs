using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragHandler : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public Image icon;        // Reference to the item icon
    public ItemData itemData; // Reference to the ScriptableObject data
    private Vector3 originalLocalPosition; // Store the initial local position of the item
    private Canvas canvas;
    private RectTransform rectTransform;
    private Inventory inventory;
    private RectTransform parentRectTransform; // The RectTransform of the parent container (inventory)
    private Vector3 originalWorldPosition;

    private ItemUI itemUI; // Reference to the ItemUI component for initializing item data.

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        inventory = FindObjectOfType<Inventory>(); // Make sure you have a reference to Inventory
        parentRectTransform = rectTransform.parent.GetComponent<RectTransform>(); // Get the inventory's parent RectTransform

        // Store the world position at the start of the drag
        originalWorldPosition = rectTransform.position;

        Debug.Log($"Original World Position: {originalWorldPosition}"); // Log the world position

        // Fetch ItemUI component from the parent (ItemSlot)
        itemUI = GetComponentInParent<ItemUI>();
    }

    // This is where you assign the ItemData when the drag starts.
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemUI != null)
        {
            itemData = itemUI.GetItemData(); // Fetch the ItemData from the ItemUI component.
            Debug.Log($"Item Data assigned: {itemData?.itemName}");
        }
        else
        {
            Debug.LogError("ItemUI component is missing on the dragged item!");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Get the world position of the pointer
        Vector3 worldPosition = Input.mousePosition;

        // Convert the world position to local position relative to the parent container (Inventory Panel)
        Vector3 localPosition = parentRectTransform.InverseTransformPoint(worldPosition);

        // Apply the local position to the item’s RectTransform
        rectTransform.localPosition = localPosition;

        Debug.Log($"Dragging item: {itemData?.itemName} at local position: {rectTransform.localPosition}"); // Log the dragging position
    }

    // Called when the user finishes dragging the item
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"End drag for item: {itemData?.itemName}");

        // Check if the drop target is valid (e.g., an NPC or another valid target)
        if (IsDroppedOnValidTarget(eventData))
        {
            Debug.Log("Dropped on valid target (NPC).");

            // Logic for interacting with an NPC or completing a quest
            InteractWithNPC();
        }
        else
        {
            Debug.Log("Dropped outside of a valid target. Returning item to original position.");

            // Instantiate a new item slot and place it into the inventory again.
            GameObject itemSlot = Instantiate(inventory.itemSlotPrefab, inventory.inventoryPanel);

            ItemUI itemUI = itemSlot.GetComponent<ItemUI>();
            if (itemUI != null)
            {
                if (itemData != null) // Ensure itemData is not null before initializing
                {
                    itemUI.Initialize(itemData);  // Initialize the item with its data
                }
                else
                {
                    Debug.LogError("ItemData is null in DragHandler when returning item to inventory.");
                }
            }
            else
            {
                Debug.LogError("ItemUI component missing on the item slot prefab!");
            }

            // Destroy the dragged slot to avoid duplicates
            Destroy(gameObject);

            // Trigger layout rebuild to ensure the layout group repositions correctly
            LayoutRebuilder.ForceRebuildLayoutImmediate(inventory.inventoryPanel.GetComponent<RectTransform>());
        }
    }

    // Checks if the item is dropped on a valid target (e.g., an NPC)
    private bool IsDroppedOnValidTarget(PointerEventData eventData)
    {
        // For this example, we're using a simple check for NPCs.
        // You can extend this to other targets as needed.
        Collider2D hit = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(eventData.position));

        if (hit != null && hit.CompareTag("NPC"))
        {
            Debug.Log("Item dropped on NPC.");
            return true;
        }

        return false;
    }

    // Example interaction with an NPC (triggering quest completion)
    private void InteractWithNPC()
    {
        // Assuming the NPC is the target, handle the item interaction here
        // For example, complete a quest or give the item to the NPC
        NPC npc = FindObjectOfType<NPC>(); // Find the NPC (make sure there's one in the scene)

        if (npc != null && itemData != null)
        {
            Debug.Log($"Giving {itemData.itemName} to NPC {npc.name} to complete quest.");
            npc.TryCompleteQuest(itemData); // Use the ItemData to complete the quest
        }
        else
        {
            Debug.Log("NPC or item data is null, cannot interact.");
        }
    }
}
