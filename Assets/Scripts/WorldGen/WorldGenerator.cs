using UnityEngine;
using UnityEngine.InputSystem;

public class WorldGenerator : MonoBehaviour
{
    public Terrain terrain;

    public float tiling = 10f;

    public bool randomizeSeed;
    public int seed;

    void Awake()
    {
        if (!randomizeSeed)
        {
            Random.InitState(seed);
        }
        else
        {
            Random.InitState((int)System.DateTime.Now.Ticks);
        }
    }

    void Start()
    {
        GenerateHeights(terrain, tiling);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GenerateHeights(terrain, tiling);
        }
    }


    public void GenerateHeights(Terrain terrain, float tileSize)
    {
        float[,] heights = new float[terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution];

        int maxOffset = 5;
        int xOffset = (int)Random.Range(0, maxOffset);
        int yOffset = (int)Random.Range(0, maxOffset);

        for (int y = 0; y < terrain.terrainData.heightmapResolution; y++)
        {
            for (int x = 0; x < terrain.terrainData.heightmapResolution; x++)
            {
                heights[x, y] = Mathf.PerlinNoise(
                    (x + xOffset / (float)terrain.terrainData.heightmapResolution) * tileSize, // Width?
                    (y + yOffset / (float)terrain.terrainData.heightmapResolution) * tileSize  // Heigth?
                ) / 10.0f;
            }
        }

        terrain.terrainData.SetHeights(0, 0, heights);
    }
}
