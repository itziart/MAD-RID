using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(SpriteRenderer))]
public class TilemapPositionSorting : MonoBehaviour
{
    private Tilemap tilemap; // Reference to the tilemap
    private SpriteRenderer spriteRenderer;
    public int customSortingOrder = 0; 

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Dynamically find the Tilemap in the scene
        tilemap = FindObjectOfType<Tilemap>();

        if (tilemap == null)
        {
            Debug.LogError("No Tilemap found in the scene! Ensure there is a Tilemap present.");
        }
    }

    private void LateUpdate()
    {
        if (tilemap == null) return;

        // Get the grid position of the object
        Vector3Int gridPosition = tilemap.WorldToCell(transform.position);

        if (customSortingOrder == 0)
        {
            // Calculate sorting order: Lower X first, then lower Y
            spriteRenderer.sortingOrder = -gridPosition.y * 2 - gridPosition.x * 1000;
        }

        else
        {
            spriteRenderer.sortingOrder = customSortingOrder;
        }
        
    }

    // Expose sorting order for other objects to use
    public int GetSortingOrder()
    {
        return spriteRenderer.sortingOrder;
    }
}