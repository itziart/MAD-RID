using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int gridWidth = GridConstants.MAXWIDTH;
    public int gridHeight = GridConstants.MAXHEIGHT;
    public GameTile[,] logicalGrid;

    void Start()
    {
        InitializeGrid();
    }

    void InitializeGrid()
    {
        logicalGrid = new GameTile[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                logicalGrid[x, y] = new GameTile(GameTile.TileType.Empty, new Vector2Int(x, y));
            }
        }
    }

    public static Vector3 GridToWorldPosition(Vector2Int gridPosition)
    {
        // Convert grid coordinates to world space
        return new Vector3(gridPosition.x, gridPosition.y, 0); // Adjust based on isometric view
    }

    public Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        // Convert world space coordinates to grid indices
        return new Vector2Int(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.y));
    }
}