using System.Diagnostics;
using System.Linq;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;

public class WorldGenerator : MonoBehaviour
{
    public Terrain terrain;

    public WorldGenSettings worldGenSettings;

    // public float freq_1 = 3;
    // public float divideVal = 30;
    // public float freq_2 = 10;
    // public float divideVal_2 = 180;

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
        if (!randomizeSeed)
        {
            UnityEngine.Random.InitState(seed);
        }
        else
        {
            UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);
        }

        if (genSettings == null)
        {
            LoadGenSettings(worldGenSettings);
        }
        else
        {
            LoadGenSettings(genSettings);
        }

        enabled = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Stopwatch watch = Stopwatch.StartNew();

            GenerateHeights(terrain);
            AlphamapGeneration(terrain);

            watch.Stop();
            UnityEngine.Debug.Log($"Terrain generation took: {watch.ElapsedMilliseconds}ms; {(float)watch.ElapsedMilliseconds/1000} seconds");
        }
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


    public void GenerateHeights(Terrain t) // Maybe convert to jobs system later
    {
        Profiler.BeginSample("Terrain height gen");

        float[,] heights = new float[t.terrainData.heightmapResolution, t.terrainData.heightmapResolution];

        int maxOffset = 1000;
        int offset = UnityEngine.Random.Range(0, maxOffset);

        float halfRes = t.terrainData.heightmapResolution / 2f;

        for (int y = 0; y < t.terrainData.heightmapResolution; y++)
        { // HEIGHT
            for (int x = 0; x < t.terrainData.heightmapResolution; x++)
            { // WIDTH

                float amplitude = _amplitude;
                float frequency = _frequency;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfRes) / t.terrainData.heightmapResolution * frequency + offset * frequency;
                    float sampleY = (y - halfRes) / t.terrainData.heightmapResolution * frequency - offset * frequency;

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


        bool[,] holes = new bool[t.terrainData.heightmapResolution - 1, t.terrainData.heightmapResolution - 1];
        for (int i = 0; i < t.terrainData.heightmapResolution - 1; i++) // LEFT/RIGHT falloff
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
            for (int innerId = t.terrainData.heightmapResolution - 1 - fallOffInner; innerId < t.terrainData.heightmapResolution - 1; innerId++)
            {
                heights[i, innerId] -= subtractVal;
                subtractVal += calcVal;

                if (heights[i, innerId] <= -3f)
                {
                    holes[i, innerId] = true;
                }
            }
        }

        for (int i = 0; i < t.terrainData.heightmapResolution - 1; i++) // UP/BOTTOM falloff
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
            for (int innerId = t.terrainData.heightmapResolution - fallOffInner - 1; innerId < t.terrainData.heightmapResolution - 1; innerId++)
            {
                heights[innerId, i] -= subtractVal;
                subtractVal += calcVal;

                if (heights[innerId, i] <= -3f)
                {
                    holes[innerId, i] = true;
                }
            }
        }

        for (int y = 0; y < t.terrainData.heightmapResolution - 1; y++)
        {
            for (int x = 0; x < t.terrainData.heightmapResolution - 1; x++)
            {
                holes[x, y] = !holes[x, y];
            }
        }


        t.terrainData.SetHoles(0, 0, holes);


        for (int y = 0; y < t.terrainData.heightmapResolution; y++) // NORMALIZING HEIGHTS TO 0.0 - 1.0
        { // HEIGHT
            for (int x = 0; x < t.terrainData.heightmapResolution; x++)
            { // WIDTH
                heights[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, heights[x, y]);
            }
        }

        t.terrainData.SetHeights(0, 0, heights);

        Profiler.EndSample();
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

                if (height <= 0.8f)
                {
                    splatWeights[1] = 0.8f;
                }
                if (height > 0.6f && height < 4f)
                {
                    splatWeights[0] = 0.8f;
                }
                if (height > 3.5f)
                {
                    splatWeights[2] = 0.8f;
                }

                if (angle > 30 && height <= 15)
                {
                    splatWeights[0] = 1 - Mathf.Clamp01(angle / 90f);
                }

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
        t.terrainData.SetAlphamaps(0, 0, map);
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