using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System;

public class TilemapCaveGeneratorWithEvent : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap = null;
    [SerializeField] private TileBase wallTile = null;
    [SerializeField] private TileBase floorTile = null;
    [Range(0, 1)][SerializeField] private float randomFillPercent = 0.5f;
    [SerializeField] private int gridSize = 100;
    [SerializeField] private int simulationSteps = 20;
    [SerializeField] private float pauseTime = 0.1f;

    private CaveGenerator caveGenerator;
    private bool finished = false;

    public event Action OnGenerationComplete; // Event to notify completion

    public bool IsFinished() => finished;

    public Tilemap GetTilemap() => tilemap;

    void Start()
    {
        UnityEngine.Random.InitState(100); // Initialize random state for reproducibility
        caveGenerator = new CaveGenerator(randomFillPercent, gridSize);
        caveGenerator.RandomizeMap();
        StartCoroutine(SimulateCavePattern()); // Start the simulation coroutine
    }

    private IEnumerator SimulateCavePattern()
    {
        for (int i = 0; i < simulationSteps; i++)
        {
            yield return new WaitForSeconds(pauseTime);
            caveGenerator.SmoothMap();
            GenerateAndDisplayTexture(caveGenerator.GetMap());
        }
        finished = true;
        Debug.Log("Simulation completed!");
        OnGenerationComplete?.Invoke(); // Notify subscribers
    }

    private void GenerateAndDisplayTexture(int[,] data)
    {
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                TileBase tile = data[x, y] == 1 ? wallTile : floorTile;
                tilemap.SetTile(position, tile);
            }
        }
    }
}
