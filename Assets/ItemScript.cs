using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName = "Default Item"; // Name of the item
    public Sprite itemIcon;                 // Icon for the inventory (optional)
    public bool isCollectible = true;       // Whether the item can be picked up

    public void Collect()
    {
        Debug.Log($"Item Collected: {itemName}");
        // Add logic for collection, if needed (e.g., animations, sound)
        Destroy(gameObject); // Remove the item from the map
    }
}
