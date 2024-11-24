using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData itemData; // Reference to the data, which will persist

    public void Collect()
    {

        // Destroy only the visual representation (ItemImage)
        Destroy(gameObject); // This will destroy the visual object, but not the data
    }
}