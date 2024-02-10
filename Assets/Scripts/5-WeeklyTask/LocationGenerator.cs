using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class LocationGenerator : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap = null;
    [SerializeField] private AllowedTiles allowedTiles = null;
    [SerializeField] private int numOfMoves = 5;

    private bool isDone = false;

    public bool shouldUpdate = true;

    private void Update()
    {
        Debug.Log("LocationGenerator Update");
        if (shouldUpdate)
        {
            Debug.Log("LocationGenerator shouldUpdate");
            GenerateLocation();
        }
    }

    private void Start()
    {
        GenerateLocation();
    }

    public bool IsDone()
    {
        if (isDone)
        {
            isDone = false;
            return true;
        }
        return isDone;
    }

    private void GenerateLocation()
    {
        Vector3Int startPosition = FindStartPosition();
        List<Vector3Int> path = GeneratePath(startPosition, numOfMoves);

        if (path.Count >= numOfMoves)
        {

            Debug.Log($"Generated path starting at {startPosition} with {path.Count} moves for object {gameObject.name}.");
            transform.position = tilemap.GetCellCenterWorld(startPosition);
            isDone = true;

        }
        else
        {
            Debug.LogError("Failed to generate a valid path.");
            GenerateLocation();
        }
    }

    private Vector3Int FindStartPosition()
    {
        // Find a random allowed tile to start the path from
        Vector3Int startPosition = Vector3Int.zero;
        while (true)
        {
            int x = Random.Range(tilemap.cellBounds.xMin, tilemap.cellBounds.xMax);
            int y = Random.Range(tilemap.cellBounds.yMin, tilemap.cellBounds.yMax);
            startPosition = new Vector3Int(x, y, 0);
            if (IsAllowedTile(startPosition)) break;
        }
        return startPosition;
    }

    private List<Vector3Int> GeneratePath(Vector3Int startPosition, int moves)
    {
        List<Vector3Int> bestPath = new List<Vector3Int>();
        DFS(startPosition, new List<Vector3Int> { startPosition }, ref bestPath, moves);
        return bestPath;
    }

    private void DFS(Vector3Int currentPosition, List<Vector3Int> currentPath, ref List<Vector3Int> bestPath, int maxDepth)
    {
        if (currentPath.Count > maxDepth) return; // Limit depth to avoid excessively long paths

        // Update bestPath if the current path is longer
        if (currentPath.Count > bestPath.Count)
        {
            bestPath = new List<Vector3Int>(currentPath);
        }

        // Get neighbors and explore each one
        List<Vector3Int> neighbors = GetAllowedTileNeighbors(currentPosition);
        foreach (Vector3Int neighbor in neighbors)
        {
            if (!currentPath.Contains(neighbor)) // Avoid backtracking
            {
                List<Vector3Int> newPath = new List<Vector3Int>(currentPath) { neighbor };
                DFS(neighbor, newPath, ref bestPath, maxDepth);
            }
        }
    }

    private List<Vector3Int> GetAllowedTileNeighbors(Vector3Int position)
    {
        List<Vector3Int> allowedNeighbors = new List<Vector3Int>();
        // Check adjacent tiles (up, down, left, right) for allowed tiles
        Vector3Int[] directions = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

        foreach (var direction in directions)
        {
            Vector3Int neighborPosition = position + direction;
            if (IsAllowedTile(neighborPosition))
            {
                allowedNeighbors.Add(neighborPosition);
            }
        }

        return allowedNeighbors;
    }

    private bool IsAllowedTile(Vector3Int cellPosition)
    {
        TileBase tile = tilemap.GetTile(cellPosition);
        return allowedTiles.Contains(tile);
    }
}
