using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName = "New Item"; // Name of the item
    public Sprite itemIcon;             // Icon for the inventory (optional)
}