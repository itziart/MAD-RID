using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(SpriteRenderer))]
public class TilemapPositionSorting : MonoBehaviour
{
    private Tilemap tilemap; // Reference to the tilemap
    private SpriteRenderer spriteRenderer;

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

        // Calculate sorting order: Lower X first, then lower Y
        spriteRenderer.sortingOrder = -gridPosition.y - gridPosition.x*1000;
    }
}