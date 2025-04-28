using System;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectGenerator : MonoBehaviour
{
    public LayerMask raycastMask;
    public float raycastHeight = 50f;
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
        
        foreach(GenSettings obj in genSettings.objects)
        {
            await GenerateObject(obj);
        }
        
        watch.Stop();
        UnityEngine.Debug.Log($"Object gen took: {watch.ElapsedMilliseconds}ms");
    }
    
    public async UniTask GenerateObject(GenSettings obj)
    {
        int spawnAmount = obj.spawnAmount;
        float terrainHeight = terrain.terrainData.size.x;
        float terrainWidth = terrain.terrainData.size.z;
        
        switch(obj.spawnType)
        {
            case SpawnType.Linear:
                
                for (int y = 0; y < terrainHeight; y++)
                {
                    for (int x = 0; x < terrainWidth; x++)
                    {
                        // if(CreateObject_Linear(obj, x, y, spawnAmount)){
                        //     spawnAmount--;
                        // }
                    }
                    //await UniTask.Delay(25);
                }
            break;
            
            case SpawnType.Raycast:
                while(spawnAmount > 0)
                {
                    if(CreateObject_Raycast(obj, terrainHeight, terrainWidth)){
                        spawnAmount--;
                    }
                    //await UniTask.Delay(25);
                }
            break;
        }
    }
    
    /*private bool CreateObject_Linear(GenSettings obj, int x, int y, int spawnAmount)
    {
        switch(obj.spawnRule)
        {
            case SpawnRule.Height:
                float height = terrain.SampleHeight(new Vector3(x, 0, y));
                float randRoll = Random.value;

                if (height >= obj.spawnRuleValue.x && height < obj.spawnRuleValue.y && randRoll < obj.spawnChance && spawnAmount > 0)
                {
                    Vector3 spawnPos = new Vector3(x, height, y);
                    Instantiate(obj.prefab, spawnPos, Quaternion.AngleAxis(Random.Range(0, 360f), Vector3.up));
                    return true;
                }
            break;
            
            case SpawnRule.Steepness:
                return true;
            //break;
            
            case SpawnRule.Normal:
                return true;
            //break;
            
            case SpawnRule.Position:
                return true;
            //break;
            
        }
        return false;
    }*/
    
    private bool CreateObject_Raycast(GenSettings obj, float terrainHeight, float terrainWidth)
    {
        Vector3 rayOrigin = new Vector3(Random.Range(0, terrainHeight), raycastHeight, Random.Range(0, terrainWidth));
            
        Ray ray = new Ray(rayOrigin, Vector3.down);
        RaycastHit[] hit = new RaycastHit[1];
        bool canSpawn = true;
        if(Physics.RaycastNonAlloc(ray, hit, Mathf.Infinity, raycastMask) > 0) // добавить ещё проверку на объекты вокруг? overlapSphere
        {
            foreach(SpawnRule rule in obj.spawnRules)
            {
                if(HandleSpawnRule(rule, hit[0])){
                    continue;
                }
                else{
                    canSpawn = false;
                }
            }
            
            if(canSpawn)
            {
                Quaternion rot = Quaternion.identity;
                if(obj.alignToGround){
                    rot *= Quaternion.LookRotation(hit[0].normal, Vector3.up) * Quaternion.AngleAxis(90, Vector3.right);
                }
                rot *= Quaternion.AngleAxis(Random.Range(0, 360f), Vector3.up);
                
                Instantiate(obj.prefab, hit[0].point, rot);
                return true;
            }
        }
        return false;
    }
    
    private bool HandleSpawnRule(SpawnRule rule, RaycastHit hit)
    {
        switch(rule.spawnRuleType)
        {
            case SpawnRuleType.Height:
                float height = raycastHeight - (raycastHeight - hit.point.y);
                if(height >= rule.valueRange.x && height < rule.valueRange.y){
                    return true;
                }
            break;
            case SpawnRuleType.Angle:
                float angle = Vector3.Angle(Vector3.up, hit.normal);
                if(angle >= rule.valueRange.x && angle < rule.valueRange.y){
                    return true;
                }
            break;
            
            case SpawnRuleType.Position: // нужно две точки здесь
                return true;
            //break;
        }
        return false;
    }
}
