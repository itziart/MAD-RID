using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    // Class that handles player movement and core gameplay mechanics


    public float moveSpeed = 4f; // Speed of movement

    public Tilemap tilemap; // Reference to the Tilemap (set in the Inspector)
    private Vector3 targetPosition; // Target/current player position on the Tilemap

    private List<Vector3Int> currentPath = new List<Vector3Int>(); // List to store the current path for Click based movement

    private Collider2D targetNPC = null; // The NPC the player is targeting for interaction

    private PolygonCollider2D polygonCollider; // Reference to the Tilemaps polygon collider to detect map boundaries
    private Camera mainCamera; // Reference to the main camera

    private bool isMoving = false; // Flag to ensure no overlapping movement

    private Inventory inventory; // Reference to the player's inventory 
    private Animator animator; // Reference to Animator component


    private void Start()
    {
        // set current position to where the player was placed on the level
        targetPosition = transform.position;

        // Get the Polygon Collider 2D component attached to the Tilemap
        polygonCollider = tilemap.GetComponent<PolygonCollider2D>();
        mainCamera = Camera.main; // Load the main Camera

        inventory = GetComponent<Inventory>(); // Get the inventory component from the player object

        // Check for missing/unassigned components required to function properly
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


        // Get the Animator component to handle movement animations
        animator = GetComponentInChildren<Animator>(); // animator is stored in the child object (PlayerSprite)
        if (animator == null)
        {
            Debug.LogError("Animator not found on Player!");
        }
    }

    // Function to handle player input. Update is called every frame update.
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

    // Function that checks if a Tile is walkable (there is no NPC or Non-Pickable Item on the tile)
    private bool IsTileAccessible(Vector3Int targetTile)
    {
        Vector3 worldPosition = tilemap.GetCellCenterWorld(targetTile); // get the Tile position

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

        return true; // If none of the conditions are met, the tile is accessible
    }

    // Method to check if a position is within the bounds of the polygon collider (map boundaries)
    private bool IsWithinColliderBounds(Vector3 position)
    {
        // Check if the polygonCollider is properly assigned
        if (polygonCollider == null)
        {
            Debug.LogError("PolygonCollider2D is missing!");
            return false;
        }

        return polygonCollider.OverlapPoint(position); // if the position is inside the map boundaries, return true
    }


    // Method to handle keyboard input
    private void HandleKeyboardInput()
    {
        Vector3 movement = Vector3.zero; //Initialize the vector, where we store the movement information

        // Detect movement direction
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            movement = new Vector3(0, 1, 0); // Move up the Y axis of the Tilemap (NE)
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            movement = new Vector3(0, -1, 0); // Move down the Y axis of the Tilemap (SW)
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            movement = new Vector3(-1, 0, 0); // Move up the X axis of the Tilemap (NW)
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            movement = new Vector3(1, 0, 0); // Move down the X axis of the Tilemap (SE)
        }

        // If the movement is not zero, proceed to update the player's position
        if (movement != Vector3.zero)
        {

            Vector3Int currentTile = tilemap.WorldToCell(targetPosition); // Get the current tilemap position of the player
            Vector3Int newTile = currentTile + Vector3Int.FloorToInt(movement); // Calculate the desired tilemap position after the movement

            // If the new tile is blocked by an NPC, interact with the NPC
            if (IsTileBlockedByNPC(newTile, out Collider2D npcCollider))
            {
                InteractWithNPC(npcCollider);
                return;
            }

            // If the new tile is accessible, proceed to update the player's position in the MoveToTarget() method
            if (IsTileAccessible(newTile))
            {
                // Steps to update the player's position
                targetPosition = tilemap.GetCellCenterWorld(newTile);
                currentPath.Clear(); // Clear the current path 
                currentPath.Add(newTile); // Add the new tile to the Player's path
                isMoving = true; // Set the isMoving flag to True to inform the MoveToTarget() method about the player's movement
            }
        }
    }

    // Function to handle mouse click movement
    private void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0)) // If Left-Click
        {
            // Check if the Camera got properly assigned
            if (mainCamera == null)
            {
                Debug.LogError("Main camera is null!");
                return;
            }

            // Get the mouse position in the world space
            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0; // Make sure that the mouse position is flat (no Z axis)

            Vector3Int gridPosition = tilemap.WorldToCell(mouseWorldPosition); // Get the gridPosition of the mouse
            Vector3Int currentTile = tilemap.WorldToCell(transform.position); // Get the current tilemap position of the player

            

            // Check if the clicked tile contains an NPC
            if (IsTileBlockedByNPC(gridPosition, out Collider2D npcCollider))
            {
                // Save the target NPC
                targetNPC = npcCollider;

                // If the NPC is adjacent to the player, interact with the NPC and stop
                if (IsAdjacentToPlayer(currentTile, gridPosition))
                {
                    InteractWithNPC(targetNPC);
                    return;
                }

                // If the NPC is not adjacent to the player, find an accessible tile that is adjacent to the NPC and move there
                Vector3Int adjacentTile = FindAccessibleAdjacentTile(gridPosition, currentTile);

                // If any accessible Tile is found, move there
                if (adjacentTile != Vector3Int.zero)
                {
                    isMoving = true;
                }

                return;
            }



            // Normal movement if no NPC is clicked

            if (IsTileAccessible(gridPosition) && IsPathAccessible(currentTile, gridPosition)) // Check if the clicked Tile is accessible for the player
            {
                Debug.Log($"Valid path found to {gridPosition}. Starting movement.");
                targetNPC = null; // Clear any previously selected NPC
                isMoving = true; // Set the isMoving flag to True to inform the MoveToTarget() method about the player's movement
            }

            else
            {
                Debug.LogWarning($"No valid path to tile {gridPosition}.");
            }

        }
    }

    // Method that handles Player's movement set in the HandleMouseClick and HandleKeyboardInput methods
    private void MoveToTarget()
    {
        // If the player is not moving, stop the movement animations
        if (!isMoving)
        {
            animator.SetBool("IsMoving", false);
        }

        // If the player is moving, move towards the target position and adjust the movement animation
        if (isMoving && currentPath.Count > 0)
        {
            Vector3 nextTilePosition = tilemap.GetCellCenterWorld(currentPath[0]); // Get the position of the next tile in the path

            
            Vector3 direction = (nextTilePosition - transform.position).normalized; // Compute direction for movement

            // Update animator parameters to reflect the player's movement
            animator.SetBool("IsMoving", true);
            animator.SetFloat("MoveX", direction.x);
            animator.SetFloat("MoveY", direction.y);

            // Move toward the target Tile
            transform.position = Vector3.MoveTowards(transform.position, nextTilePosition, moveSpeed * Time.deltaTime);

            // After the player reaches the next tile, update the target position and remove the reached tile from the Path
            if (Vector3.Distance(transform.position, nextTilePosition) < 0.01f)
            {
                transform.position = nextTilePosition; // Set the player's position to the reached Tile
                targetPosition = nextTilePosition; // Update the current Tilemap position of player to the reached tile
                currentPath.RemoveAt(0); // Remove the reached tile from the Path

                // If the player has reached the last tile in the path, stop the movement
                if (currentPath.Count == 0)
                {
                    isMoving = false; // Set the isMoving flag to False to stop the movement
                    animator.SetBool("IsMoving", false); // Inform the animator to stop the movement animation

                    Vector3Int currentTile = tilemap.WorldToCell(transform.position);


                    CheckForItem(currentTile); // Check if the new Tile contains an Item to pick up
                }
            }
        }
    }



    // Methot to check if the Tile is blocked by an NPC
    private bool IsTileBlockedByNPC(Vector3Int gridPosition, out Collider2D npcCollider)
    {
        Vector3 worldPosition = tilemap.GetCellCenterWorld(gridPosition); // Get the Tile world Position
        float detectionRadius = 0.4f; // Define a small radius in which we look for NPCs

        Collider2D[] hits = Physics2D.OverlapCircleAll(worldPosition, detectionRadius); // Find all the colliders inside the radius
        foreach (Collider2D hit in hits)  // For each Collider inside the radius
        {
            if (hit.CompareTag("NPC")) // Check if the Collider belongs to an NPC
            {
                // If the NPC is found, save the NPC Collider and return true
                npcCollider = hit;
                return true;
            }
        }
        
        // If the NPC isn't found, return False
        npcCollider = null;
        return false;
    }


    // Method to check if the Tile is blocked by a Non-Pickable Item (Backgroud Items/Interactable Items)
    private bool IsTileBlockedByItem(Vector3Int gridPosition)
    {
        Vector3 worldPosition = tilemap.GetCellCenterWorld(gridPosition); // Get the Tile world position
        float detectionRadius = 0.4f; // Define a small radius in which we look for Items

        Collider2D[] hits = Physics2D.OverlapCircleAll(worldPosition, detectionRadius); //  Find all the colliders inside the radius
        foreach (Collider2D hit in hits) // For each Collider inside the radius
        {
            if (hit.CompareTag("NonPickableItem")) // Check if the Collider belongs to an Non-Pickable Item
            {
                return true;
            }
        }

        return false;
    }
    // Method to check for collectible items on the current tile
    private void CheckForItem(Vector3Int gridPosition)
    {
        Vector3 worldPosition = tilemap.GetCellCenterWorld(gridPosition); // Get world position of the tile
        float detectionRadius = 0.4f; // Define radius for item detection

        Collider2D[] hits = Physics2D.OverlapCircleAll(worldPosition, detectionRadius); // Detect all colliders within the radius
        foreach (var hit in hits) // Iterate through the detected colliders
        {
            if (hit.CompareTag("Item")) // Check if the collider belongs to an item
            {
                Item item = hit.GetComponent<Item>(); // Get the Item component
                if (item != null)
                {
                    if (item.itemData != null) // Ensure the item has valid data
                    {
                        inventory.AddItem(item.itemData); // Add the item to the inventory
                        item.Collect(); // Trigger item collection behavior
                        Debug.Log($"Item collected: {item.itemData.itemName}"); // Log the item name
                    }
                    else
                    {
                        Debug.LogError($"Item '{hit.name}' is missing its ItemData."); // Log missing ItemData error
                    }
                }
            }
        }
    }

    // Method to handle interaction with an NPC
    private void InteractWithNPC(Collider2D npcCollider)
    {
        NPC npc = npcCollider.GetComponent<NPC>(); // Get the NPC component
        if (npc != null)
        {
            npc.Interact(); // Trigger interaction behavior
            Debug.Log("Interacted with NPC."); // Log interaction
        }
    }

    // Method to determine if a path from start to target tile is accessible
    private bool IsPathAccessible(Vector3Int startTile, Vector3Int targetTile)
    {
        Debug.Log($"Starting pathfinding from {startTile} to {targetTile}."); // Log pathfinding initiation

        currentPath.Clear(); // Clear any existing path
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>(); // Track visited tiles
        Queue<Vector3Int> queue = new Queue<Vector3Int>(); // Queue for BFS traversal
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>(); // Track path

        queue.Enqueue(startTile); // Start BFS from the start tile
        visited.Add(startTile); // Mark start tile as visited
        cameFrom[startTile] = startTile; // Initialize the path tracking

        // Define possible movement directions (cardinal)
        Vector3Int[] directions = new Vector3Int[]
        {
            new Vector3Int(0, 1, 0), // Up
            new Vector3Int(0, -1, 0), // Down
            new Vector3Int(-1, 0, 0), // Left
            new Vector3Int(1, 0, 0) // Right
        };

        while (queue.Count > 0) // Process the queue
        {
            Vector3Int currentTile = queue.Dequeue(); // Get the next tile
            Debug.Log($"Processing tile: {currentTile}"); // Log the current tile

            if (currentTile == targetTile) // Path to target found
            {
                Debug.Log($"Path found to target tile: {targetTile}"); // Log success
                ReconstructPath(cameFrom, targetTile); // Reconstruct the path
                return true; // Path is accessible
            }

            foreach (Vector3Int direction in directions) // Check neighboring tiles
            {
                Vector3Int neighbor = currentTile + direction; // Get neighbor position

                if (visited.Contains(neighbor) || !IsTileAccessible(neighbor)) // Skip inaccessible/visited tiles
                {
                    Debug.Log($"Skipping inaccessible or already visited tile: {neighbor}"); // Log skipped tile
                    continue;
                }

                visited.Add(neighbor); // Mark as visited
                queue.Enqueue(neighbor); // Add to the queue
                cameFrom[neighbor] = currentTile; // Record the path
            }
        }

        Debug.LogWarning($"No path found from {startTile} to {targetTile}."); // Log failure
        return false; // No accessible path found
    }

    // Method to reconstruct the path from start to target tile
    private void ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int targetTile)
    {
        currentPath.Clear(); // Clear the current path
        Vector3Int currentTile = targetTile; // Start from the target tile

        while (cameFrom.ContainsKey(currentTile) && cameFrom[currentTile] != currentTile) // Trace back to start
        {
            currentPath.Add(currentTile); // Add the tile to the path
            currentTile = cameFrom[currentTile]; // Move to the previous tile
        }

        currentPath.Add(currentTile); // Add the starting tile
        currentPath.Reverse(); // Reverse to get path in correct order

        Debug.Log($"Reconstructed path: {string.Join(" -> ", currentPath)}"); // Log the reconstructed path
    }

    // Method to find an accessible tile adjacent to an NPC
    private Vector3Int FindAccessibleAdjacentTile(Vector3Int npcTile, Vector3Int startTile)
    {
        Vector3Int[] directions = new Vector3Int[]
        {
            new Vector3Int(0, 1, 0), // Up
            new Vector3Int(0, -1, 0), // Down
            new Vector3Int(-1, 0, 0), // Left
            new Vector3Int(1, 0, 0) // Right
        };

        foreach (Vector3Int direction in directions) // Check adjacent tiles
        {
            Vector3Int adjacentTile = npcTile + direction; // Calculate position of adjacent tile
            if (IsTileAccessible(adjacentTile) && IsPathAccessible(startTile, adjacentTile)) // Check if accessible
            {
                return adjacentTile; // Return accessible tile
            }
        }

        return Vector3Int.zero; // No accessible adjacent tile found
    }

    // Method to determine if a player is adjacent to an NPC
    private bool IsAdjacentToNPC(Vector3Int playerTile, Collider2D npcCollider)
    {
        if (npcCollider == null) return false; // Return false if no NPC collider

        Vector3Int npcTile = tilemap.WorldToCell(npcCollider.transform.position); // Get NPC tile position

        Vector3Int[] directions = new Vector3Int[]
        {
            new Vector3Int(0, 1, 0), // Up
            new Vector3Int(0, -1, 0), // Down
            new Vector3Int(-1, 0, 0), // Left
            new Vector3Int(1, 0, 0) // Right
        };

        foreach (Vector3Int direction in directions) // Check adjacency
        {
            if (playerTile + direction == npcTile) // Check if adjacent
            {
                return true; // Player is adjacent to NPC
            }
        }

        return false; // Player is not adjacent to NPC
    }

    // Method to determine if a tile is adjacent to the player
    private bool IsAdjacentToPlayer(Vector3Int playerTile, Vector3Int checkedTile)
    {
        Vector3Int[] directions = new Vector3Int[]
        {
            new Vector3Int(0, 1, 0), // Up
            new Vector3Int(0, -1, 0), // Down
            new Vector3Int(-1, 0, 0), // Left
            new Vector3Int(1, 0, 0) // Right
        };

        foreach (Vector3Int direction in directions) // Check adjacency
        {
            if (playerTile + direction == checkedTile) // Check if adjacent
            {
                return true; // Tile is adjacent to player
            }
        }

        return false; // Tile is not adjacent to player
    }


}
