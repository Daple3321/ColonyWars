using System.Diagnostics;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectGenerator : MonoBehaviour
{
    public Terrain terrain;
    [SerializeField] private ObjectGenSettings genSettings;

    void Awake()
    {
        enabled = false;
    }
    
    public void Init(ObjectGenSettings objectGenSettings)
    {
        genSettings = objectGenSettings;
        enabled = true;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Keypad3))
        {
            GenerateObjects().Forget();
        }
    }

    public async UniTask GenerateObjects()
    {
        Stopwatch watch = Stopwatch.StartNew();
        
        // UniTask genTask = UniTask.RunOnThreadPool(async () =>
        // {
        // });
        // await genTask;
        foreach(GenSettings obj in genSettings.objects)
        {
            await GenerateObject(obj);
        }
        
        watch.Stop();
        UnityEngine.Debug.Log($"Object gen took: {watch.ElapsedMilliseconds}ms");
    }
    
    public async UniTask GenerateObject(GenSettings obj)
    {
        float terrainHeight = terrain.terrainData.size.x;
        float terrainWidth = terrain.terrainData.size.z;
        
        for (int y = 0; y < terrainHeight; y++)
        {
            for (int x = 0; x < terrainWidth; x++)
            {
                HandleSpawnRule(obj, x, y);
            }
        }
    }
    
    private void HandleSpawnRule(GenSettings obj, int x, int y)
    {
        switch(obj.spawnRule)
        {
            case SpawnRule.Height:
                float height = terrain.SampleHeight(new Vector3(x, 0, y));
                float randRoll = Random.value;

                if (height >= obj.spawnRuleValue.x && height < obj.spawnRuleValue.y && randRoll < obj.spawnChance && obj.spawnAmount > 0)
                {
                    Vector3 spawnPos = new Vector3(x, height, y);
                    Instantiate(obj.prefab, spawnPos, Quaternion.AngleAxis(Random.Range(0, 360f), Vector3.up));
                    obj.spawnAmount--;
                }
            break;
            
            case SpawnRule.Steepness:
            
            break;
            
            case SpawnRule.Normal:
            
            break;
            
            case SpawnRule.Position:
            
            break;
            
        }
    }
}
