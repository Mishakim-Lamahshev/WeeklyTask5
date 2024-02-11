using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class TilemapCaveGenerator : MonoBehaviour
{
    [SerializeField] Tilemap tilemap = null;
    [SerializeField] TileBase wallTile = null;
    [SerializeField] TileBase floorTile = null;
    [Range(0, 1)]
    [SerializeField] float randomFillPercent = 0.5f;
    [SerializeField] int gridSize = 100;
    [SerializeField] int simulationSteps = 20;
    [SerializeField] float pauseTime = 0.1f; // Consider reducing this for quicker generation

    private CaveGenerator caveGenerator;
    private bool finished = false;

    public bool IsFinished() => finished;

    void Start()
    {
        Random.InitState(100); // Initialize random state for reproducibility
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
