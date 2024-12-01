using UnityEngine;
using UnityEngine.UI; // Required for Image

public class ItemUI : MonoBehaviour
{
    public Image icon; // The UI Image component to display the item's icon
    private ItemData itemData; // item data (item Name and icon)

    public void Initialize(ItemData data)
    {
        if (data == null)
        {
            Debug.LogError("ItemData is null. Cannot initialize ItemUI.");
            return;
        }

        itemData = data;

        if (data.itemIcon != null)
        {
            icon.sprite = data.itemIcon; // Assign the sprite to the UI Image
        }
        else
        {
            Debug.LogError($"ItemIcon is missing for {data.itemName}");
        }
    }

    public ItemData GetItemData()
    {
        return itemData;
    }
}