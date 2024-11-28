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

        Collider2D hit = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(eventData.position));

        if (hit != null)
        {
            // Attempt to interact with any InteractableObject
            InteractableObject interactable = hit.GetComponent<InteractableObject>();
            if (interactable != null)
            {
                if (interactable.Interact(itemData))
                {
                    Destroy(gameObject); // Destroy the dragged item after successful interaction
                    return;
                }
            }
        }

        // Return the item to inventory if no valid interaction occurred
        ReplaceItemInInventory();
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
