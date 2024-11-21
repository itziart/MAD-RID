using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector2Int gridPosition; // Current position on the logical grid
    public float moveSpeed = 5f; // Movement speed for smooth transitions
    public Inventory inventory; // Player inventory to hold items
    public GridManager gridManager; // Reference to the GridManager
    public List<GameQuest> activeQuests; //List of active quests


    void Start()
    {
        // Initialize player position to match the grid
        gridPosition = gridManager.WorldToGridPosition(transform.position);
    }

    void Update()
    {
        HandleInput(); // Check for player input
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.W)) TryMove(Vector2Int.up); // Move up
        if (Input.GetKeyDown(KeyCode.S)) TryMove(Vector2Int.down); // Move down
        if (Input.GetKeyDown(KeyCode.A)) TryMove(Vector2Int.left); // Move left
        if (Input.GetKeyDown(KeyCode.D)) TryMove(Vector2Int.right); // Move right
    }

    void TryMove(Vector2Int direction)
    {
        Vector2Int targetPosition = gridPosition + direction;

        // Ensure the target position is within grid bounds
        if (!IsWithinBounds(targetPosition)) return;

        GameTile targetTile = gridManager.logicalGrid[targetPosition.x, targetPosition.y];

        if (targetTile.Type == GameTile.TileType.Empty || targetTile.Type == GameTile.TileType.Item)
        {
            // Move to the target position
            gridPosition = targetPosition;
            StartCoroutine(MoveToPosition(GridManager.GridToWorldPosition(targetPosition)));

            // If there's an item, pick it up
            if (targetTile.Type == GameTile.TileType.Item) PickUpItem(targetTile);
        }
        else if (targetTile.Type == GameTile.TileType.NPC)
        {
            // Interact with the NPC
            InteractWithNPC(targetTile);
        }

    }

    IEnumerator MoveToPosition(Vector3 targetWorldPosition)
    {
        while ((transform.position - targetWorldPosition).sqrMagnitude > 0.01f)
        {
            transform.position =
                Vector3.MoveTowards(transform.position, targetWorldPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetWorldPosition;
    }

    bool IsWithinBounds(Vector2Int position)
    {
        return position.x >= 0 && position.x < gridManager.gridWidth &&
               position.y >= 0 && position.y < gridManager.gridHeight;
    }

    void PickUpItem(GameTile tile)
    {
        GameObject item = tile.Occupant;

        if (item != null)
        {
            inventory.AddItem(item);
            Destroy(item); // Remove the item from the world
            tile.Type = GameTile.TileType.Empty;
            tile.Occupant = null;
        }
    }

    void InteractWithNPC(GameTile tile)
    {
        GameObject npcObject = tile.Occupant;

        if (npcObject != null)
        {
            NPC npc = npcObject.GetComponent<NPC>();
            if (npc != null)
            {
                return;
            }
        }
    }


    void UpdateSpriteDirection(Vector2Int direction)
    {
        if (direction == Vector2Int.up) transform.localRotation = Quaternion.Euler(0, 0, 0);     // Face up
        if (direction == Vector2Int.down) transform.localRotation = Quaternion.Euler(0, 0, 180); // Face down
        if (direction == Vector2Int.left) transform.localRotation = Quaternion.Euler(0, 0, 90);  // Face left
        if (direction == Vector2Int.right) transform.localRotation = Quaternion.Euler(0, 0, -90); // Face right
    }
}