using UnityEngine;

public class GameTile : MonoBehaviour
{
    public enum TileType { Empty, Item, NPC, Obstacle } // Possible types of tiles

    public TileType Type;          // Type of the tile
    public GameObject Occupant;    // Reference to the object on this tile (item/NPC, etc.)
    public Vector2Int Position;    // Logical grid position of the tile

    // Constructor to initialize the tile
    public GameTile(TileType type, Vector2Int position)
    {
        Type = type;
        Position = position;
        Occupant = null; // Start with no occupant
    }
}