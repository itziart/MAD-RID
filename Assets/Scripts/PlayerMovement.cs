using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;  // Speed of movement
    public Vector3 tileSize = new Vector3(3f, 1.5f, 1f);  // Tile size for grid-based movement
    public Vector3 cellGap = new Vector3(-0.2f, -0.1f, 0); // Cell gap (for finer adjustments)

    public Tilemap tilemap;  // Reference to the Tilemap
    private Vector3 targetPosition;  // Target position for movement

    private PolygonCollider2D polygonCollider; // Reference to the Polygon Collider 2D
    private Camera mainCamera; // Reference to the main camera

    private bool isMoving = false;  // Flag to ensure no overlapping movement

    private Inventory inventory; // Reference to the player's inventory

    private void Start()
    {
        targetPosition = transform.position;

        // Get the Polygon Collider 2D component attached to the Tilemap
        polygonCollider = tilemap.GetComponent<PolygonCollider2D>();
        mainCamera = Camera.main; // Cache the main camera

        inventory = GetComponent<Inventory>();  // Get the inventory component from the player object

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

    private bool IsTileBlockedByNPC(Vector3Int gridPosition, out Collider2D npcCollider)
    {
        Vector3 worldPosition = tilemap.GetCellCenterWorld(gridPosition);

        // Define a small radius for detection (adjust if needed)
        float detectionRadius = 0.4f;

        // Use a LayerMask to exclude the tilemap if necessary
        LayerMask npcLayerMask = LayerMask.GetMask("Default"); // Adjust based on your NPC layer

        // Perform the overlap circle check
        Collider2D[] hits = Physics2D.OverlapCircleAll(worldPosition, detectionRadius, npcLayerMask);

        // Loop through all detected colliders
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("NPC"))
            {
                npcCollider = hit;
                return true; // Tile is blocked by an NPC
            }
        }

        npcCollider = null;
        return false; // No NPC found
    }

    private void HandleKeyboardInput()
    {
        Vector3Int movement = Vector3Int.zero;

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
            // Calculate the new grid position with movement
            Vector3Int newGridPosition = tilemap.WorldToCell(targetPosition) + movement;

            // Check if the new position is occupied by an NPC
            if (IsTileBlockedByNPC(newGridPosition, out Collider2D npcCollider))
            {
                InteractWithNPC(npcCollider); // Start interaction if blocked by NPC
                return;
            }

            // Check if the new position has an item
            CheckForItem(newGridPosition);

            // Get the world position for the new grid position (center of the tile)
            Vector3 targetWorldPosition = tilemap.GetCellCenterWorld(newGridPosition);

            // Check if the target position is within the Polygon Collider 2D bounds
            if (IsWithinColliderBounds(targetWorldPosition))
            {
                // Move to the new target position and set the isMoving flag
                targetPosition = targetWorldPosition;
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

            // Convert the mouse position to world space
            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0;  // Ensure it remains flat in 2D space

            Debug.Log($"Mouse World Position: {mouseWorldPosition}");

            // Convert world position to grid coordinates
            Vector3Int gridPosition = tilemap.WorldToCell(mouseWorldPosition);

            // Check if the grid position is occupied by an NPC
            if (IsTileBlockedByNPC(gridPosition, out Collider2D npcCollider))
            {
                InteractWithNPC(npcCollider); // Start interaction if blocked by NPC
                return;
            }

            // Check if the grid position is occupied by an item
            CheckForItem(gridPosition);

            // Get the center of the cell
            Vector3 targetWorldPosition = tilemap.GetCellCenterWorld(gridPosition);

            // Check if the target position is within collider bounds
            if (IsWithinColliderBounds(targetWorldPosition))
            {
                targetPosition = targetWorldPosition;
                isMoving = true;
                Debug.Log("Target position is within bounds. Moving to target.");
            }
            else
            {
                Debug.LogWarning("Target position is outside bounds. No movement.");
            }
        }
    }

    private void CheckForItem(Vector3Int gridPosition)
    {
        Vector3 worldPosition = tilemap.GetCellCenterWorld(gridPosition);

        // Use a small radius to check if there is an item on the tile
        float detectionRadius = 0.4f;

        Collider2D[] hits = Physics2D.OverlapCircleAll(worldPosition, detectionRadius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Item"))
            {
                Item item = hit.GetComponent<Item>();
                if (item != null)
                {
                    // Add item to inventory and remove it from the world
                    inventory.AddItem(item);
                    item.Collect();
                    Debug.Log($"Item collected: {item.itemName}");
                }
            }
        }
    }

    private void InteractWithNPC(Collider2D npcCollider)
    {
        NPC npc = npcCollider.GetComponent<NPC>();
        if (npc != null)
        {
            npc.Interact(); // Trigger interaction with the NPC
        }
    }

    private void MoveToTarget()
    {
        // If the player is moving, move towards the target position
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // If the player reaches the target position, stop moving
            if (transform.position == targetPosition)
            {
                isMoving = false;
            }
        }
    }

    private bool IsWithinColliderBounds(Vector3 position)
    {
        if (polygonCollider == null)
        {
            Debug.LogError("PolygonCollider2D is missing!");
            return false;
        }

        bool isInside = polygonCollider.OverlapPoint(position);
        return isInside;
    }
}
