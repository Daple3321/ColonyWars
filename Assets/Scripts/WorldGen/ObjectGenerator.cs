using System;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;

public class ObjectGenerator : MonoBehaviour
{
    public LayerMask raycastMask;
    public LayerMask overlapMask;
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
                if(CheckSpawnRule(rule, hit[0])){
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
                
                Transform t = Instantiate(obj.prefab, hit[0].point, rot).transform;
                
                if(obj.randomizeScale){
                    float randScale = Random.Range(obj.scaleRange.x, obj.scaleRange.y);
                    t.localScale = new Vector3(randScale, randScale, randScale);
                }
                return true;
            }
        }
        return false;
    }
    
    private bool CheckSpawnRule(SpawnRule rule, RaycastHit hit)
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
    
    
    /*public Building CreateBuilding_Interval(BuildingData building, float buildingInterval, params string[] overlapBlacklist)
    {
        Vector3 rayOrigin = new Vector3(Random.Range(0, terrain.terrainData.size.x), raycastHeight, Random.Range(0, terrain.terrainData.size.x));
        
        Ray ray = new Ray(rayOrigin, Vector3.down);
        RaycastHit[] hit = new RaycastHit[1];
        if(Physics.RaycastNonAlloc(ray, hit, Mathf.Infinity, raycastMask) > 0)
        {
            if(!building.CheckBuildConditions(hit[0].point)){
                return null;
            }
            
            Collider[] overlap = Physics.OverlapSphere(hit[0].point, buildingInterval, overlapMask);
            if(overlap.Length > 0)
            {
                foreach(Collider c in overlap)
                {
                    if(!c.gameObject.tag.ContainsAny(overlapBlacklist))
                    {
                        Quaternion rot = Quaternion.identity;
                        rot *= Quaternion.AngleAxis(Random.Range(0, 360f), Vector3.up);
                        GameObject go = Instantiate(building.prefab, hit[0].point, rot);
                        
                        return go.GetComponent<Building>();
                    }
                    else{
                        return null;
                    }
                }
            }
            else
            {
                Quaternion rot = Quaternion.identity;
                rot *= Quaternion.AngleAxis(Random.Range(0, 360f), Vector3.up);
                GameObject go = Instantiate(building.prefab, hit[0].point, rot);
                
                return go.GetComponent<Building>();
            }
        }
        
        return null;
    }*/
    
    public Building CreateBuilding_Rules(BuildingData building, Vector3 pos) // вообще чёт не понял зачем это так сделано
    {
        bool spawned = false;
        int maxIterations = 10;
        while(!spawned && maxIterations > 0) // repeat until suitable place is found
        {
            Collider[] hitColliders = Physics.OverlapSphere(pos, building.overlapRadius, LayerMask.GetMask("EnemyBuilding","PlayerBuilding"));
            if(hitColliders.Length <= 0)
            {
                Quaternion rot = Quaternion.identity;
                rot *= Quaternion.AngleAxis(Random.Range(0, 360f), Vector3.up);
                GameObject go = Instantiate(building.prefab, pos, rot);
                
                spawned = true;
                
                return go.GetComponent<Building>();
            }
            
            maxIterations--;
        }
        
        /*Collider[] hitColliders = Physics.OverlapSphere(pos, building.overlapRadius, LayerMask.GetMask("EnemyBuilding","PlayerBuilding"));
        if(hitColliders.Length <= 0)
        {
            Quaternion rot = Quaternion.identity;
            rot *= Quaternion.AngleAxis(Random.Range(0, 360f), Vector3.up);
            GameObject go = Instantiate(building.prefab, pos, rot);
            
            spawned = true;
            
            return go.GetComponent<Building>();
        }*/
        
        return null;
    }
}
