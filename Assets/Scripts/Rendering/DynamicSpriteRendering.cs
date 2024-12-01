using UnityEngine;
using UnityEngine.Tilemaps;

// This component automatically adjusts the sorting order of a SpriteRenderer based on its position within a Tilemap
[RequireComponent(typeof(SpriteRenderer))] // Ensure that a SpriteRenderer is attached to the GameObject
public class TilemapPositionSorting : MonoBehaviour
{
    private Tilemap tilemap; // Reference to the Tilemap in the scene
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer of this object
    public int customSortingOrder = 0; // Optionally set a custom sorting order instead of the default calculation

    // Called when the object is initialized
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component attached to the same GameObject

        // Dynamically find the Tilemap in the scene (assumes only one Tilemap in the scene)
        tilemap = FindObjectOfType<Tilemap>();

        // If no Tilemap is found, log an error
        if (tilemap == null)
        {
            Debug.LogError("No Tilemap found in the scene! Ensure there is a Tilemap present.");
        }
    }

    // Called every frame after all Update functions
    private void LateUpdate()
    {
        if (tilemap == null) return; // If no Tilemap was found, exit early to avoid errors

        // Convert the world position of this object into grid coordinates relative to the Tilemap
        Vector3Int gridPosition = tilemap.WorldToCell(transform.position);

        // If no custom sorting order is set, calculate a sorting order based on the grid position
        if (customSortingOrder == 0)
        {
            // Calculate the sorting order: prioritize Y position for sorting, then X position
            // This ensures objects lower on the grid (further "down" in world space) appear in front of higher ones
            spriteRenderer.sortingOrder = -gridPosition.y * 2 - gridPosition.x * 1000;
        }
        else
        {
            // If a custom sorting order is provided, use that instead of the default calculation
            spriteRenderer.sortingOrder = customSortingOrder;
        }
    }

    // Returns the current sorting order of this object
    public int GetSortingOrder()
    {
        return spriteRenderer.sortingOrder; // Return the current sorting order of the SpriteRenderer
    }
}
