using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    //Class to store the data of an Item 

    public string itemName = "New Item"; // Name of the item
    public Sprite itemIcon;             // Icon for the inventory (optional)
}