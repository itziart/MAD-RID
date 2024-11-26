using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of movement
    public Vector3 tileSize = new Vector3(3f, 1.5f, 1f); // Tile size for grid-based movement
    public Vector3 cellGap = new Vector3(-0.2f, -0.1f, 0); // Cell gap (for finer adjustments)

    public Tilemap tilemap; // Reference to the Tilemap
    private Vector3 targetPosition; // Target position for movement

    private List<Vector3Int> currentPath = new List<Vector3Int>(); // List to store the current path for Click based movement

    private Collider2D targetNPC = null; // The NPC the player is targeting for interaction


    private PolygonCollider2D polygonCollider; // Reference to the Polygon Collider 2D
    private Camera mainCamera; // Reference to the main camera

    private bool isMoving = false; // Flag to ensure no overlapping movement

    private Inventory inventory; // Reference to the player's inventory

    private void Start()
    {
        targetPosition = transform.position;

        // Get the Polygon Collider 2D component attached to the Tilemap
        polygonCollider = tilemap.GetComponent<PolygonCollider2D>();
        mainCamera = Camera.main; // Cache the main camera

        inventory = GetComponent<Inventory>(); // Get the inventory component from the player object

        if (polygonCollider == null)
        {
            Debug.LogError("PolygonCollider2D not found on Tilemap!");
        }

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found in the scene!");
        }

        if (inventory == null)
        {
            Debug.LogError("Inventory component not found on Player!");
        }
    }

    private void Update()
    {
        // Handle keyboard input for movement
        if (!isMoving)
        {
            HandleKeyboardInput();
        }

        // Handle mouse click movement
        HandleMouseClick();

        // Move the player towards the target position
        MoveToTarget();
    }

    private bool IsTileAccessible(Vector3Int targetTile)
    {
        Vector3 worldPosition = tilemap.GetCellCenterWorld(targetTile);

        // Check if the tile is within the bounds of the polygon collider
        if (!IsWithinColliderBounds(worldPosition))
        {
            Debug.Log("Tile is outside collider bounds.");
            return false;
        }

        // Check for NPC 
        if (IsTileBlockedByNPC(targetTile, out Collider2D npcCollider))
        {
            return false; // Tile is blocked by NPC
        }

        // Check for Non-Pickable Items
        if (IsTileBlockedByItem(targetTile))
        {
            return false; // Tile is blocked by Non-Pickable Item
        }

        return true; // Tile is accessible
    }


    private bool IsWithinColliderBounds(Vector3 position)
    {
        if (polygonCollider == null)
        {
            Debug.LogError("PolygonCollider2D is missing!");
            return false;
        }

        return polygonCollider.OverlapPoint(position);
    }

    private void HandleKeyboardInput()
    {
        Vector3Int movement = Vector3Int.zero;
        Debug.Log("Keyboard input detected.");

        // Detect movement direction (one tile per key press)
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            movement = new Vector3Int(0, 1, 0); // Move up (one tile)
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            movement = new Vector3Int(0, -1, 0); // Move down (one tile)
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            movement = new Vector3Int(-1, 0, 0); // Move left (one tile)
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            movement = new Vector3Int(1, 0, 0); // Move right (one tile)
        }

        if (movement != Vector3Int.zero)
        {
            Vector3Int currentTile = tilemap.WorldToCell(targetPosition);
            Vector3Int newTile = currentTile + movement;

            if (IsTileBlockedByNPC(newTile, out Collider2D npcCollider)) //If Tile is blocked by NPC
            {
                InteractWithNPC(npcCollider); //Interact with NPC
                return;
            }
            if (IsTileAccessible(newTile))
            {
                targetPosition = tilemap.GetCellCenterWorld(newTile);
                currentPath.Clear();
                currentPath.Add(newTile);
                isMoving = true;
            }
        }
    }

    private void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0)) // Left-click
        {
            if (mainCamera == null)
            {
                Debug.LogError("Main camera is null!");
                return;
            }

            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0;

            Vector3Int gridPosition = tilemap.WorldToCell(mouseWorldPosition);
            Vector3Int currentTile = tilemap.WorldToCell(transform.position);

            Debug.Log($"Mouse clicked at {mouseWorldPosition}, grid {gridPosition}.");

            // Check if the clicked tile contains an NPC
            if (IsTileBlockedByNPC(gridPosition, out Collider2D npcCollider))
            {
                // Save the target NPC
                targetNPC = npcCollider;

                if (IsAdjacentToPlayer(currentTile, gridPosition))
                {
                    InteractWithNPC(targetNPC);
                    return;
                }

                // Find an accessible adjacent tile to move to
                Vector3Int adjacentTile = FindAccessibleAdjacentTile(gridPosition, currentTile);

                if (adjacentTile != Vector3Int.zero)
                {
                    Debug.Log($"Moving to adjacent tile {adjacentTile} to interact with NPC.");
                    isMoving = true;
                }
                else
                {
                    Debug.LogWarning("No accessible adjacent tile to reach the NPC.");
                }

                return;
            }

            // Normal movement if no NPC is clicked
            if (IsTileAccessible(gridPosition) && IsPathAccessible(currentTile, gridPosition))
            {
                Debug.Log($"Valid path found to {gridPosition}. Starting movement.");
                targetNPC = null; // Clear any previously selected NPC
                isMoving = true;
            }
            else
            {
                Debug.LogWarning($"No valid path to tile {gridPosition}.");
            }
        }
    }

    private void MoveToTarget()
    {
        // Continue moving along the path if the player is in motion
        if (isMoving && currentPath.Count > 0)
        {
            // Get the next tile's world position from the path
            Vector3 nextTilePosition = tilemap.GetCellCenterWorld(currentPath[0]);

            // Smoothly move the player toward the next tile
            transform.position = Vector3.MoveTowards(transform.position, nextTilePosition, moveSpeed * Time.deltaTime);

            // Check if the player has reached the target tile
            if (Vector3.Distance(transform.position, nextTilePosition) < 0.01f)
            {
                // Snap the player to the center of the tile to avoid floating-point inaccuracies
                transform.position = nextTilePosition;

                // Remove the tile from the path as it's been reached
                currentPath.RemoveAt(0);

                // If there are no more tiles in the path
                if (currentPath.Count == 0)
                {
                    isMoving = false;

                    // Get the current grid position of the player
                    Vector3Int currentTile = tilemap.WorldToCell(transform.position);

                    // Log the player's stop action, showing the current tile position
                    Debug.Log($"Player stopped at tile {currentTile}. Target NPC: {targetNPC?.name ?? "None"}");

                    // If an NPC is targeted and the player is now adjacent, interact with the NPC
                    if (targetNPC != null && IsAdjacentToNPC(currentTile, targetNPC))
                    {
                        InteractWithNPC(targetNPC);
                        targetNPC = null; // Clear the NPC target after interaction
                    }

                    // Check for items to pick up on the current tile
                    CheckForItem(currentTile);
                }
            }
        }
    }

    private bool IsTileBlockedByNPC(Vector3Int gridPosition, out Collider2D npcCollider)
    {
        Vector3 worldPosition = tilemap.GetCellCenterWorld(gridPosition);
        float detectionRadius = 0.4f;
        LayerMask npcLayerMask = LayerMask.GetMask("Default");

        Collider2D[] hits = Physics2D.OverlapCircleAll(worldPosition, detectionRadius, npcLayerMask);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("NPC"))
            {
                npcCollider = hit;
                return true;
            }
        }

        npcCollider = null;
        return false;
    }

    private bool IsTileBlockedByItem(Vector3Int gridPosition)
    {
        Vector3 worldPosition = tilemap.GetCellCenterWorld(gridPosition);
        float detectionRadius = 0.4f;
        LayerMask itemLayerMask = LayerMask.GetMask("Default");

        Collider2D[] hits = Physics2D.OverlapCircleAll(worldPosition, detectionRadius, itemLayerMask);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("NonPickableItem"))
            {
                return true;
            }
        }

        return false;
    }

    private void CheckForItem(Vector3Int gridPosition)
    {
        Vector3 worldPosition = tilemap.GetCellCenterWorld(gridPosition);
        float detectionRadius = 0.4f;

        Collider2D[] hits = Physics2D.OverlapCircleAll(worldPosition, detectionRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Item"))
            {
                Item item = hit.GetComponent<Item>();
                if (item != null)
                {
                    if (item.itemData != null)
                    {
                        inventory.AddItem(item.itemData);
                        item.Collect();
                        Debug.Log($"Item collected: {item.itemData.itemName}");
                    }
                    else
                    {
                        Debug.LogError($"Item '{hit.name}' is missing its ItemData.");
                    }
                }
            }
        }
    }

    private void InteractWithNPC(Collider2D npcCollider)
    {
        NPC npc = npcCollider.GetComponent<NPC>();
        if (npc != null)
        {
            npc.Interact();
            Debug.Log("Interacted with NPC.");
        }
    }

    private bool IsPathAccessible(Vector3Int startTile, Vector3Int targetTile)
    {
        Debug.Log($"Starting pathfinding from {startTile} to {targetTile}.");

        currentPath.Clear(); // Clear any existing path
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();

        queue.Enqueue(startTile);
        visited.Add(startTile);
        cameFrom[startTile] = startTile; // Start points to itself

        Vector3Int[] directions = new Vector3Int[]
        {
            new Vector3Int(0, 1, 0), // Up
            new Vector3Int(0, -1, 0), // Down
            new Vector3Int(-1, 0, 0), // Left
            new Vector3Int(1, 0, 0) // Right
        };

        while (queue.Count > 0)
        {
            Vector3Int currentTile = queue.Dequeue();
            Debug.Log($"Processing tile: {currentTile}");

            if (currentTile == targetTile)
            {
                Debug.Log($"Path found to target tile: {targetTile}");
                ReconstructPath(cameFrom, targetTile);
                return true;
            }

            foreach (Vector3Int direction in directions)
            {
                Vector3Int neighbor = currentTile + direction;

                if (visited.Contains(neighbor) || !IsTileAccessible(neighbor))
                {
                    Debug.Log($"Skipping inaccessible or already visited tile: {neighbor}");
                    continue;
                }

                visited.Add(neighbor);
                queue.Enqueue(neighbor);
                cameFrom[neighbor] = currentTile; // Record the path
            }
        }

        Debug.LogWarning($"No path found from {startTile} to {targetTile}.");
        return false;
    }

    private void ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int targetTile)
    {
        currentPath.Clear();
        Vector3Int currentTile = targetTile;

        while (cameFrom.ContainsKey(currentTile) && cameFrom[currentTile] != currentTile)
        {
            currentPath.Add(currentTile);
            currentTile = cameFrom[currentTile];
        }

        currentPath.Add(currentTile); // Add the starting tile
        currentPath.Reverse(); // Reverse to get the path from start to target

        Debug.Log($"Reconstructed path: {string.Join(" -> ", currentPath)}");
    }

    private Vector3Int FindAccessibleAdjacentTile(Vector3Int npcTile, Vector3Int startTile)
    {
        Vector3Int[] directions = new Vector3Int[]
        {
            new Vector3Int(0, 1, 0), // Up
            new Vector3Int(0, -1, 0), // Down
            new Vector3Int(-1, 0, 0), // Left
            new Vector3Int(1, 0, 0) // Right
        };

        foreach (Vector3Int direction in directions)
        {
            Vector3Int adjacentTile = npcTile + direction;
            if (IsTileAccessible(adjacentTile) && IsPathAccessible(startTile, adjacentTile))
            {
                return adjacentTile;
            }
        }

        return Vector3Int.zero; // No accessible adjacent tile found
    }

    private bool IsAdjacentToNPC(Vector3Int playerTile, Collider2D npcCollider)
    {
        if (npcCollider == null) return false;

        Vector3Int npcTile = tilemap.WorldToCell(npcCollider.transform.position);

        // Check adjacency in all four cardinal directions
        Vector3Int[] directions = new Vector3Int[]
        {
            new Vector3Int(0, 1, 0), // Up
            new Vector3Int(0, -1, 0), // Down
            new Vector3Int(-1, 0, 0), // Left
            new Vector3Int(1, 0, 0) // Right
        };

        foreach (Vector3Int direction in directions)
        {
            if (playerTile + direction == npcTile)
            {
                return true; // Player is adjacent to the NPC
            }
        }

        return false;
    }

    private bool IsAdjacentToPlayer(Vector3Int playerTile, Vector3Int checkedTile)
    {
        Vector3Int[] directions = new Vector3Int[]
        {
            new Vector3Int(0, 1, 0), // Up
            new Vector3Int(0, -1, 0), // Down
            new Vector3Int(-1, 0, 0), // Left
            new Vector3Int(1, 0, 0) // Right
        };

        foreach (Vector3Int direction in directions)
        {
            if (playerTile + direction == checkedTile)
            {
                return true; // Player is adjacent to the checked tile
            }
        }

        return false;
    }

}
