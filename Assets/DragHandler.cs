using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragHandler : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public Image icon;        // Reference to the item icon
    public ItemData itemData; // Reference to the ScriptableObject data
    private Vector3 originalLocalPosition;
    private Canvas canvas;
    private RectTransform rectTransform;
    private Inventory inventory;
    private RectTransform parentRectTransform;
    private Vector3 originalWorldPosition;

    private ItemUI itemUI;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        inventory = FindObjectOfType<Inventory>();
        parentRectTransform = rectTransform.parent.GetComponent<RectTransform>();
        originalWorldPosition = rectTransform.position;

        // Fetch ItemUI component from the parent (ItemSlot)
        itemUI = GetComponentInParent<ItemUI>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemUI != null)
        {
            itemData = itemUI.GetItemData();
            Debug.Log($"Item Data assigned: {itemData?.itemName}");
        }
        else
        {
            Debug.LogError("ItemUI component is missing on the dragged item!");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 worldPosition = Input.mousePosition;
        Vector3 localPosition = parentRectTransform.InverseTransformPoint(worldPosition);
        rectTransform.localPosition = localPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"End drag for item: {itemData?.itemName}");

        // Check if the drop target is a valid drop hitbox or NPC
        Collider2D hit = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(eventData.position));
        if (hit != null)
        {
            if (hit.CompareTag("NPCDropHitbox"))
            {
                Debug.Log("Item dropped on DropHitbox.");
                InteractWithDropHitbox(hit.GetComponent<NpcDropHandler>());
            }
            else
            {
                ReplaceItemInInventory();
            }
        }
        else
        {
            ReplaceItemInInventory();
        }
    }

    private void InteractWithDropHitbox(NpcDropHandler dropHandler)
    {
        if (dropHandler != null && itemData != null)
        {
            if (dropHandler.TryDropItem(itemData))
            {
                Destroy(gameObject); // Destroy the dragged item after successful interaction
            }
            else
            {
                ReplaceItemInInventory(); // Return the item if the drop is invalid
            }
        }
        else
        {
            Debug.Log("Invalid DropHitbox or missing item data.");
            ReplaceItemInInventory();
        }
    }


    private void ReplaceItemInInventory()
    {
        Debug.Log("Dropped outside of a valid target. Returning item to original position.");
        GameObject itemSlot = Instantiate(inventory.itemSlotPrefab, inventory.inventoryPanel);

        ItemUI itemUI = itemSlot.GetComponent<ItemUI>();
        if (itemUI != null)
        {
            if (itemData != null)
            {
                itemUI.Initialize(itemData);
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

        Destroy(gameObject);
        LayoutRebuilder.ForceRebuildLayoutImmediate(inventory.inventoryPanel.GetComponent<RectTransform>());
    }
}
