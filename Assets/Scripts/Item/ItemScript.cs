using UnityEngine;

public class Item : MonoBehaviour
{
    // Class that handles the items collection and their data using itemData

    public ItemData itemData; // Reference to the data, which will persist

    // Upon collecting the item by Player
    public void Collect()
    {

        // Destroy only the visual representation (ItemImage)
        Destroy(gameObject); // This will destroy the visual object, but not the data
    }
}