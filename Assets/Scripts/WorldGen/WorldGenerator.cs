using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using UnityEngine.Profiling;

public class WorldGenerator : MonoBehaviour
{
    public Terrain terrain;
    
    public WorldGenSettings worldGenSettings;

    // public float freq_1 = 3;
    // public float divideVal = 30;
    // public float freq_2 = 10;
    // public float divideVal_2 = 180;

    public int treeGenSkip;
    public float treeSpawnChance;
    public float treeSpawnHeight;
    public int spawnAmount;
    public GameObject treePrefab;

    public float perstistance = 0.5f;
    public float lacunarity = 1.2f;
    public float _amplitude = 1f;
    public float _frequency = 3f;
    public int octaves = 1;

    public float maxNoiseHeight = 35f;
    public float minNoiseHeight = -3f;

    public int fallOffInner = 35;
    public int fallOffStrength = 10;

    public bool randomizeSeed;
    public int seed;


    void Awake()
    {
        enabled = false;
    }

    public void Init(WorldGenSettings genSettings = null)
    {
        if (genSettings == null)
        {
            LoadGenSettings(worldGenSettings);
        }
        else
        {
            LoadGenSettings(genSettings);
        }

        if (!randomizeSeed)
        {
            //UnityEngine.Random.InitState(seed);
            rnd = new System.Random(seed);
        }
        else
        {
            //UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);
            rnd = new System.Random();
        }

        //UnityEngine.Debug.Log("Size: " + terrain.terrainData.size);
        enabled = true;
    }

    float[,] heights;
    bool[,] holes;
    public System.Random rnd;
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            //Stopwatch watch = Stopwatch.StartNew();

            //GenerateHeights(terrain);
            // int res = terrain.terrainData.heightmapResolution;
            // heights = new float[res, res];
            // bool[,] holes = new bool[res - 1, res - 1];
            // UniTask task = UniTask.RunOnThreadPool(() =>
            // {
            //     heights = GenerateHeights(res);
            //     UnityEngine.Debug.Log("Height gen complete");
            //     terrain.terrainData.SetHeights(0, 0, heights);

            //     holes = GenerateFalloff(res);
            //     UnityEngine.Debug.Log("Hole gen complete");
            //     terrain.terrainData.SetHoles(0, 0, holes);
            // });
            GenerateTerrain().Forget();

            //int res = terrain.terrainData.heightmapResolution;
            //heights = new float[res, res];
            //heights = GenerateHeights(res);
            //terrain.terrainData.SetHeights(0, 0, heights);
            //AlphamapGeneration(terrain);

            //watch.Stop();
            //UnityEngine.Debug.Log($"Terrain generation took: {watch.ElapsedMilliseconds}ms; {(float)watch.ElapsedMilliseconds / 1000} seconds");
        }
    }

    public async UniTask GenerateTerrain()
    {
        Stopwatch watch = Stopwatch.StartNew();

        int res = terrain.terrainData.heightmapResolution;
        heights = new float[res, res];
        holes = new bool[res - 1, res - 1];

        UniTask genTask = UniTask.RunOnThreadPool(() =>
        {
            heights = GenerateHeights(res);
            UnityEngine.Debug.Log("Height gen complete");

            //holes = GenerateFalloff(res);
            //UnityEngine.Debug.Log("Hole gen complete");
        });
        await genTask;

        terrain.terrainData.SetHeights(0, 0, heights);
        //terrain.terrainData.SetHoles(0, 0, holes);
        AlphamapGeneration(terrain);
        //DetailMapCutoff(terrain, 0.2f);
        //GenerateTrees();

        RemoveDetail();
        GenerateDetailMap();

        terrain.Flush();

        watch.Stop();
        UnityEngine.Debug.Log($"Terrain generation took: {watch.ElapsedMilliseconds}ms");
    }

    public void LoadGenSettings(WorldGenSettings settings = null)
    {
        if (settings != null)
        {
            perstistance = settings.perstistance;
            lacunarity = settings.lacunarity;
            _amplitude = settings._amplitude;
            _frequency = settings._frequency;
            octaves = settings.octaves;
            maxNoiseHeight = settings.maxNoiseHeight;
            minNoiseHeight = settings.minNoiseHeight;
            fallOffInner = settings.fallOffInner;
            fallOffStrength = settings.fallOffStrength;
        }
        else
        {
            UnityEngine.Debug.Log("No worldGen settings to load.");
        }
    }


    public float[,] GenerateHeights(int heightmapResolution) // Maybe convert to jobs system later
    {
        //Profiler.BeginSample("Terrain height gen");

        float[,] heights = new float[heightmapResolution, heightmapResolution];

        int maxOffset = 1000;
        int offset = rnd.Next(0, maxOffset);

        float halfRes = heightmapResolution / 2f;

        for (int y = 0; y < heightmapResolution; y++)
        { // HEIGHT
            for (int x = 0; x < heightmapResolution; x++)
            { // WIDTH

                float amplitude = _amplitude;
                float frequency = _frequency;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfRes) / heightmapResolution * frequency + offset * frequency;
                    float sampleY = (y - halfRes) / heightmapResolution * frequency - offset * frequency;

                    //float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    float perlinValue = NoiseGen.GetVoronoiNoiseValue(sampleX, sampleY);
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= perstistance;
                    frequency *= lacunarity;
                }

                // if (noiseHeight > maxNoiseHeight)
                // {
                //     maxNoiseHeight = noiseHeight;
                // }
                // else if (noiseHeight < minNoiseHeight) {
                //     minNoiseHeight = noiseHeight;
                // }

                heights[x, y] = noiseHeight;

                // heights[x, y] = Mathf.PerlinNoise(
                //     ((float)(x + offset) / (float)t.terrainData.heightmapResolution) * freq_1,
                //     ((float)(y + offset) / (float)t.terrainData.heightmapResolution) * freq_1
                // ) / divideVal;

                // heights[x, y] += Mathf.PerlinNoise(
                //     ((float)(x + offset) / (float)t.terrainData.heightmapResolution) * freq_2,
                //     ((float)(y + offset) / (float)t.terrainData.heightmapResolution) * freq_2
                // ) / divideVal_2;

            }
        }


        // bool[,] holes = new bool[heightmapResolution - 1,heightmapResolution - 1];
        // for (int i = 0; i < heightmapResolution - 1; i++) // LEFT/RIGHT falloff
        // {
        //     float subtractVal = fallOffStrength;
        //     float calcVal = subtractVal / fallOffInner;

        //     for (int innerId = 0; innerId < fallOffInner; innerId++)
        //     {
        //         heights[i, innerId] -= subtractVal;
        //         subtractVal -= calcVal;

        //         if (heights[i, innerId] <= -3f) // Учитываем, действительно ли точка стала дыркой
        //         {
        //             holes[i, innerId] = true;
        //         }
        //     }

        //     subtractVal = calcVal;
        //     for (int innerId = heightmapResolution - 1 - fallOffInner; innerId < heightmapResolution - 1; innerId++)
        //     {
        //         heights[i, innerId] -= subtractVal;
        //         subtractVal += calcVal;

        //         if (heights[i, innerId] <= -3f)
        //         {
        //             holes[i, innerId] = true;
        //         }
        //     }
        // }

        // for (int i = 0; i < heightmapResolution - 1; i++) // UP/BOTTOM falloff
        // {
        //     float subtractVal = fallOffStrength;
        //     float calcVal = subtractVal / fallOffInner;
        //     for (int innerId = 0; innerId < fallOffInner; innerId++)
        //     {
        //         heights[innerId, i] -= subtractVal;
        //         subtractVal -= calcVal;

        //         if (heights[innerId, i] <= -3f)
        //         {
        //             holes[innerId, i] = true;
        //         }
        //     }

        //     subtractVal = calcVal;
        //     for (int innerId = heightmapResolution - fallOffInner - 1; innerId < heightmapResolution - 1; innerId++)
        //     {
        //         heights[innerId, i] -= subtractVal;
        //         subtractVal += calcVal;

        //         if (heights[innerId, i] <= -3f)
        //         {
        //             holes[innerId, i] = true;
        //         }
        //     }
        // }

        // for (int y = 0; y < heightmapResolution - 1; y++)
        // {
        //     for (int x = 0; x < heightmapResolution - 1; x++)
        //     {
        //         holes[x, y] = !holes[x, y];
        //     }
        // }
        //t.terrainData.SetHoles(0, 0, holes);


        for (int y = 0; y < heightmapResolution; y++) // NORMALIZING HEIGHTS TO 0.0 - 1.0
        { // HEIGHT
            for (int x = 0; x < heightmapResolution; x++)
            { // WIDTH
                heights[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, heights[x, y]);
            }
        }

        //t.terrainData.SetHeights(0, 0, heights);
        //Profiler.EndSample();
        return heights;
    }

    private bool[,] GenerateFalloff(int heightmapResolution)
    {
        bool[,] holes = new bool[heightmapResolution - 1, heightmapResolution - 1];
        for (int i = 0; i < heightmapResolution - 1; i++) // LEFT/RIGHT falloff
        {
            float subtractVal = fallOffStrength;
            float calcVal = subtractVal / fallOffInner;

            for (int innerId = 0; innerId < fallOffInner; innerId++)
            {
                heights[i, innerId] -= subtractVal;
                subtractVal -= calcVal;

                if (heights[i, innerId] <= -3f) // Учитываем, действительно ли точка стала дыркой
                {
                    holes[i, innerId] = true;
                }
            }

            subtractVal = calcVal;
            for (int innerId = heightmapResolution - 1 - fallOffInner; innerId < heightmapResolution - 1; innerId++)
            {
                heights[i, innerId] -= subtractVal;
                subtractVal += calcVal;

                if (heights[i, innerId] <= -3f)
                {
                    holes[i, innerId] = true;
                }
            }
        }

        for (int i = 0; i < heightmapResolution - 1; i++) // UP/BOTTOM falloff
        {
            float subtractVal = fallOffStrength;
            float calcVal = subtractVal / fallOffInner;
            for (int innerId = 0; innerId < fallOffInner; innerId++)
            {
                heights[innerId, i] -= subtractVal;
                subtractVal -= calcVal;

                if (heights[innerId, i] <= -3f)
                {
                    holes[innerId, i] = true;
                }
            }

            subtractVal = calcVal;
            for (int innerId = heightmapResolution - fallOffInner - 1; innerId < heightmapResolution - 1; innerId++)
            {
                heights[innerId, i] -= subtractVal;
                subtractVal += calcVal;

                if (heights[innerId, i] <= -3f)
                {
                    holes[innerId, i] = true;
                }
            }
        }

        for (int y = 0; y < heightmapResolution - 1; y++)
        {
            for (int x = 0; x < heightmapResolution - 1; x++)
            {
                holes[x, y] = !holes[x, y];
            }
        }

        return holes;
    }

    // Set all pixels in a detail map below a certain threshold to zero.
    private void DetailMapCutoff(Terrain t, float threshold)
    {
        // Get all of layer zero.
        int[,] map = t.terrainData.GetDetailLayer(0, 0, t.terrainData.detailWidth, t.terrainData.detailHeight, 0);

        // For each pixel in the detail map...
        for (int y = 0; y < t.terrainData.detailHeight; y++)
        {
            for (int x = 0; x < t.terrainData.detailWidth; x++)
            {
                // If the pixel value is below the threshold then
                // set it to zero.
                if (map[x, y] < threshold)
                {
                    map[x, y] = 0;
                }
            }
        }

        // Assign the modified map back.
        t.terrainData.SetDetailLayer(0, 0, 0, map);
    }

    private void AlphamapGeneration(Terrain t)
    {
        float[,,] map = new float[t.terrainData.alphamapWidth, t.terrainData.alphamapHeight, t.terrainData.alphamapLayers];
        int alphamapWidth = terrain.terrainData.alphamapWidth;
        int alphamapHeight = terrain.terrainData.alphamapHeight;
        int numLayers = terrain.terrainData.alphamapLayers;
        float heightmapToAlphamapRatioX = (float)terrain.terrainData.heightmapResolution / alphamapWidth;
        float heightmapToAlphamapRatioY = (float)terrain.terrainData.heightmapResolution / alphamapHeight;

        // For each point on the alphamap...
        for (int y = 0; y < t.terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < t.terrainData.alphamapWidth; x++)
            {
                // Get the normalized terrain coordinate that corresponds to the point.
                float normX = (float)x / (float)t.terrainData.alphamapWidth;
                float normY = (float)y / (float)t.terrainData.alphamapHeight;

                // Get the steepness value at the normalized coordinate.
                var angle = t.terrainData.GetSteepness(normX, normY);

                // Sample the height at this location (note GetHeight expects int coordinates corresponding to locations in the heightmap array)
                float height = t.terrainData.GetHeight(
                    Mathf.RoundToInt(normY * t.terrainData.heightmapResolution), Mathf.RoundToInt(normX * t.terrainData.heightmapResolution));
                //float height = t.terrainData.GetHeight(Mathf.RoundToInt(y*t.terrainData.heightmapResolution), Mathf.RoundToInt(x*t.terrainData.heightmapResolution));

                // Calculate the normal of the terrain (note this is in normalised coordinates relative to the overall terrain dimensions)
                Vector3 normal = t.terrainData.GetInterpolatedNormal(normY, normX);

                // Calculate the steepness of the terrain
                float steepness = t.terrainData.GetSteepness(normY, normX);

                // Setup an array to record the mix of texture weights at this point
                float[] splatWeights = new float[t.terrainData.alphamapLayers];


                // CHANGE THE RULES BELOW TO SET THE WEIGHTS OF EACH TEXTURE ON WHATEVER RULES YOU WANT
                // Texture[1] has constant influence
                //splatWeights[1] = 0.5f; // GRASS

                // Texture[0] is stronger at lower altitudes
                //splatWeights[0] = Mathf.Clamp01((height)); // DIRT

                // Texture[2] increases with height but only on surfaces facing positive Z axis 
                //splatWeights[2] = height * Mathf.Clamp01(normal.z); // GRAVEL

                // Texture[2] stronger on flatter terrain
                // Note "steepness" is unbounded, so we "normalise" it by dividing by the extent of heightmap height and scale factor
                // Subtract result from 1.0 to give greater weighting to flat surfaces
                //splatWeights[2] = Mathf.Clamp01(steepness*steepness/(t.terrainData.heightmapResolution/tiling)+gravelWeight);

                //splatWeights[1] = 1.0f - Mathf.Clamp01(steepness*steepness/(t.terrainData.heightmapResolution/tiling)+grassWeight);

                //splatWeights[0] = Mathf.Clamp(height, 0, 0.3f) * dirtWeight; // DIRT

                // ------------------- OLD ------------------
                // if (height <= 4f)
                // {
                //     splatWeights[1] = 0.8f;
                // }
                // if (height > 3.5f && height < 8)
                // {
                //     splatWeights[0] = 0.8f;
                // }
                // if (height > 7.5f)
                // {
                //     splatWeights[2] = 0.8f;
                // }

                // if (angle > 30 && height <= 15)
                // {
                //     splatWeights[0] = 1 - Mathf.Clamp01(angle / 90f);
                // }
                // ----------------------------------------------

                // -------------- GEMINI ------------- 
                float heightmapX = x * heightmapToAlphamapRatioX;
                float heightmapY = y * heightmapToAlphamapRatioY;
                // Используем интерполированную высоту для гладкости
                //float height = terrain.terrainData.GetInterpolatedHeight(heightmapX /
                //    terrain.terrainData.heightmapResolution, heightmapY /
                //    terrain.terrainData.heightmapResolution);
                // Нормализованная высота (0..1) относительно размера террейна
                float normalizedHeight = height / terrain.terrainData.size.y;

                // Базовый слой - трава
                splatWeights[1] = 1.0f; // Начнем с травы

                // Камень на крутых склонах
                float rockBlend = Mathf.Clamp01(((steepness - 15.0f) / 5.0f) + height - 10f); // Плавный переход к камню при наклоне > 25 градусов
                splatWeights[2] = rockBlend;
                splatWeights[1] *= (1.0f - rockBlend); // Уменьшаем вес травы

                // Песок на низких высотах (например, < 0.1)
                float sandBlend = Mathf.Clamp01((2 - height) / 2f);
                if (numLayers > 2)
                {
                    splatWeights[0] = sandBlend;
                    splatWeights[1] *= (1.0f - sandBlend); // Уменьшаем вес предыдущих слоев
                    splatWeights[2] *= (1.0f - sandBlend);
                }

                // Снег на высоких высотах (например, > 0.7)
                // float snowBlend = Mathf.Clamp01((normalizedHeight - 0.15f) / 0.1f);
                // if (numLayers > 2)
                // {
                //     splatWeights[2] = snowBlend;
                //     splatWeights[0] *= (1.0f - snowBlend); // Уменьшаем вес всех нижних слоев
                //     splatWeights[1] *= (1.0f - snowBlend);
                //     //splatWeights[2] *= (1.0f - snowBlend);
                // }

                // Sum of all textures weights must add to 1, so calculate normalization factor from sum of weights
                float z = splatWeights.Sum();

                // Loop through each terrain texture
                for (int i = 0; i < t.terrainData.alphamapLayers; i++)
                {
                    // Normalize so that sum of all texture weights = 1
                    splatWeights[i] /= z;

                    // Assign this point to the splatmap array
                    map[x, y, i] = splatWeights[i];
                }

                // Steepness is given as an angle, 0..90 degrees. Divide
                // by 90 to get an alpha blending value in the range 0..1.
                //var frac = angle / 90.0;
                //map[x, y, 1] = (float)frac;
                //map[x, y, 0] = (float)(1 - frac);
            }
        }
        //return map;
        t.terrainData.SetAlphamaps(0, 0, map);
    }

    public void GenerateTrees()
    {
        float terrainHeight = terrain.terrainData.size.x;
        float terrainWidth = terrain.terrainData.size.z;
        for (int y = 0; y < terrainHeight; y++)
        {
            for (int x = 0; x < terrainWidth; x++)
            {
                //float height = terrain.terrainData.GetHeight(x, y);
                float height = terrain.SampleHeight(new Vector3(x, 0, y));
                float randRoll = UnityEngine.Random.value;

                if (height < 1.5f && randRoll < treeSpawnChance && spawnAmount > 0)
                {
                    Vector3 spawnPos = new Vector3(x, height, y);
                    Instantiate(treePrefab, spawnPos, Quaternion.AngleAxis(UnityEngine.Random.Range(0, 360f), Vector3.up));
                    spawnAmount--;
                }
            }
        }
    }
    
    private void RemoveDetail()
    {
        int[,] map = terrain.terrainData.GetDetailLayer(0, 0, terrain.terrainData.detailWidth, terrain.terrainData.detailHeight, 0);
 
        for (int y = 0; y < terrain.terrainData.detailHeight; y++)
        {
            for (int x = 0; x < terrain.terrainData.detailWidth; x++)
            {
                map[x,y] = 0;
            }
        }

        terrain.terrainData.SetDetailLayer(0, 0, 0, map);
        terrain.terrainData.SetDetailLayer(0, 0, 1, map);
        terrain.terrainData.SetDetailLayer(0, 0, 2, map);
        terrain.terrainData.SetDetailLayer(0, 0, 3, map);
    }
    
    private void GenerateDetailMap()
    {
        float terrainHeight = terrain.terrainData.size.x;
        float terrainWidth = terrain.terrainData.size.z;
        int[,] grassMap = terrain.terrainData.GetDetailLayer(0, 0, terrain.terrainData.detailWidth, terrain.terrainData.detailHeight, 0);
        int[,] pebbleMap = terrain.terrainData.GetDetailLayer(0, 0, terrain.terrainData.detailWidth, terrain.terrainData.detailHeight, 1);
        int[,] logMap = terrain.terrainData.GetDetailLayer(0, 0, terrain.terrainData.detailWidth, terrain.terrainData.detailHeight, 2);
        int[,] stickMap = terrain.terrainData.GetDetailLayer(0, 0, terrain.terrainData.detailWidth, terrain.terrainData.detailHeight, 3);
        for (int y = 0; y < terrainHeight; y++)
        {
            for (int x = 0; x < terrainWidth; x++)
            {
                float height = terrain.SampleHeight(new Vector3(x, 0, y));

                float randOffset = UnityEngine.Random.Range(-3f, 3f);
                float xPos = x + randOffset;
                float yPos = y + randOffset;
                float xPosClamped = Mathf.Clamp(xPos, 0, terrainWidth-1);
                float yPosClamped = Mathf.Clamp(yPos, 0, terrainHeight-1);
                WorldPosToTerrainDetailPos(new Vector3(xPosClamped, 0, yPosClamped), out Vector3 terrainDetailPos);
                //Debug.Log("TerrainDetailPos: " + terrainDetailPos);
                //detailMap = terrain.terrainData.GetDetailLayer(0, 0, terrain.terrainData.detailWidth, terrain.terrainData.detailHeight, 0);

                if (height > 2f && height < 7f)
                {
                    grassMap[(int)terrainDetailPos.z, (int)terrainDetailPos.x] = 550;
                }
                
                if(height >= 0 && height < 1.2f)
                {
                    pebbleMap[(int)terrainDetailPos.z, (int)terrainDetailPos.x] = 25;
                    stickMap[(int)terrainDetailPos.z, (int)terrainDetailPos.x] = 45;
                    logMap[(int)terrainDetailPos.z, (int)terrainDetailPos.x] = 30;
                }
                if(height > 11f)
                {
                    pebbleMap[(int)terrainDetailPos.z, (int)terrainDetailPos.x] = 25;
                }
                
            }
        }


        terrain.terrainData.SetDetailLayer(0, 0, 0, grassMap);
        terrain.terrainData.SetDetailLayer(0, 0, 1, pebbleMap);
        terrain.terrainData.SetDetailLayer(0, 0, 2, logMap);
        terrain.terrainData.SetDetailLayer(0, 0, 3, stickMap);
        terrain.Flush();
    }
    
    public bool WorldPosToTerrainDetailPos(Vector3 worldPos, out Vector3 terrainDetailPos)
    {
        terrainDetailPos = Vector3.zero;

        terrainDetailPos = Vector3.zero;
        Vector3 terrainLocation = terrain.GetPosition();
        Vector3 terrainPos = Vector3.zero;
        terrainPos.x = (worldPos.x - terrainLocation.x) / terrain.terrainData.size.x;
        terrainPos.y = 0;
        terrainPos.z = (worldPos.z - terrainLocation.z) / terrain.terrainData.size.z;

        terrainDetailPos.x = Mathf.FloorToInt(terrainPos.x * terrain.terrainData.detailResolution);
        terrainDetailPos.z = Mathf.FloorToInt(terrainPos.z * terrain.terrainData.detailResolution);

        return true;
    }
}

[BurstCompile]
public static class NoiseGen
{

    [BurstCompile]
    public static float GetVoronoiNoiseValue(float x, float y)
    {
        float voronoiValue = noise.cellular(new float2(x, y)).x * 2 - 1;

        return voronoiValue;
    }
    
    [BurstCompile]
    public static float GetPerlinNoiseValue(float x, float y)
    {
        float perlinValue = Mathf.PerlinNoise(x, y) * 2 - 1;

        return perlinValue;
    }
}