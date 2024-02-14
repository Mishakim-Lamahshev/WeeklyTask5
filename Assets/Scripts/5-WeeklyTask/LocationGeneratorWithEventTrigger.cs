using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;

public class LocationGeneratorWithEventTrigger : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap = null;
    [SerializeField] private AllowedTiles allowedTiles = null;
    [SerializeField] private int numOfMoves = 5;

    private bool isDone = false;
    public bool shouldUpdate = true;

    private TilemapCaveGeneratorWithEvent caveGenerator;

    private void Start()
    {
        caveGenerator = tilemap.GetComponent<TilemapCaveGeneratorWithEvent>();
        if (caveGenerator != null)
        {
            caveGenerator.OnGenerationComplete += HandleGenerationComplete;
        }
    }

    private void OnDestroy()
    {
        if (caveGenerator != null)
        {
            caveGenerator.OnGenerationComplete -= HandleGenerationComplete;
        }
    }

    private void HandleGenerationComplete()
    {
        tilemap = caveGenerator.GetTilemap();
        GenerateLocation();
    }

    private void Update()
    {
        Debug.Log("LocationGenerator Update");
        if (shouldUpdate)
        {
            Debug.Log("LocationGenerator shouldUpdate");
            GenerateLocation();
        }
    }

    public bool IsDone()
    {
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
            shouldUpdate = false; // Assuming you want to stop updating after generation
        }
        else
        {
            Debug.LogError("Failed to generate a valid path.");
            GenerateLocation();
        }
    }

    private Vector3Int FindStartPosition()
    {
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
        if (currentPath.Count > maxDepth) return;

        if (currentPath.Count > bestPath.Count)
        {
            bestPath = new List<Vector3Int>(currentPath);
        }

        List<Vector3Int> neighbors = GetAllowedTileNeighbors(currentPosition);
        foreach (Vector3Int neighbor in neighbors)
        {
            if (!currentPath.Contains(neighbor))
            {
                List<Vector3Int> newPath = new List<Vector3Int>(currentPath) { neighbor };
                DFS(neighbor, newPath, ref bestPath, maxDepth);
            }
        }
    }

    private List<Vector3Int> GetAllowedTileNeighbors(Vector3Int position)
    {
        List<Vector3Int> allowedNeighbors = new List<Vector3Int>();
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
