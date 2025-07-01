using System;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using UnityEditor.Experimental.GraphView;

public class ObjectGenerator : MonoBehaviour
{
    public LayerMask raycastMask;
    public LayerMask overlapMask;
    public float raycastHeight = 50f;
    public Terrain terrain;
    [SerializeField] private ObjectGenSettings genSettings;
    
    public List<PersistentObject> persistentObjects;
    
    void Awake(){enabled = false;}

    void Update()
    {
        HandlePersistentSpawning();
    }

    public void Init(ObjectGenSettings objectGenSettings)
    {
        genSettings = objectGenSettings;
        
        persistentObjects = new List<PersistentObject>();
        foreach(var pObj in genSettings.persistentObjects)
        {
            if(pObj is PersistentGenSettings persistentSettings)
            {
                persistentObjects.Add(new PersistentObject(persistentSettings));
            }
        }
        
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
    
    public void HandlePersistentSpawning()
    {
        foreach(PersistentObject obj in persistentObjects)
        {
            obj.UpdateTimer();
            if(obj.ReadyToSpawn())
            {
                GameObject go = GenerateSinglePersistentObject(obj.settings);
                if(go != null){
                    obj.OnObjectSpawn();
                    
                    PersistentObjectTracker tracker = go.AddComponent<PersistentObjectTracker>();
                    tracker.source = obj;

                    tracker.OnDestroyed += (t) =>
                    {
                        t.source.OnObjectDeleted();
                    };
                    
                    // ResourceManager.i.OnResourceDeleted += res =>
                    // {
                    //     if(res.gameObject == go){
                    //         obj.OnObjectDeleted();
                    //     }
                    // };
                }
            }
        }
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
                    GameObject go = null;
                    if(obj is ResourceGenSettings resource)
                    {
                        go = CreateResource_Raycast(resource, terrainHeight, terrainWidth);
                        if(go != null){
                            spawnAmount--;
                        }
                    }
                    else
                    {
                        go = CreateObject_Raycast(obj, terrainHeight, terrainWidth);
                        if(go != null){
                            spawnAmount--;
                        }
                    }
                    
                    //await UniTask.Delay(25);
                }
            break;
        }
    }
    
    public GameObject GenerateSinglePersistentObject(GenSettings obj)
    {
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
                GameObject go = null;
                if(obj is PersistentResourceSettings resource)
                {
                    go = CreateResource_Raycast(resource, terrainHeight, terrainWidth);
                    return go;
                }
                else
                {
                    go = CreateObject_Raycast(obj, terrainHeight, terrainWidth);
                    return go;
                }
        }
        
        return null;
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
    
    private GameObject CreateObject_Raycast(GenSettings obj, float terrainHeight, float terrainWidth)
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
                return t.gameObject;
            }
        }
        return null;
    }
    
    private GameObject CreateResource_Raycast(ResourceGenSettings obj, float terrainHeight, float terrainWidth)
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
                
                ResourceManager.i.RegisterResource(t.gameObject, obj.resource, Random.Range(obj.amountRange.x, obj.amountRange.y), obj.clicksToGather);
                
                return t.gameObject;
            }
        }
        return null;
    }
    private GameObject CreateResource_Raycast(PersistentResourceSettings obj, float terrainHeight, float terrainWidth)
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
                
                ResourceManager.i.RegisterResource(t.gameObject, obj.resource, Random.Range(obj.amountRange.x, obj.amountRange.y), obj.clicksToGather);
                
                return t.gameObject;
            }
        }
        return null;
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
    public Building CreateBuilding(BuildingData building, Vector3 pos, GridManager.Direction lookDir)
    {
        Quaternion rot = Helper.GetRotationFromDirection(lookDir);
        //rot *= Quaternion.AngleAxis(Random.Range(0, 360f), Vector3.up);
        GameObject go = Instantiate(building.prefab, pos, rot);
        
        return go.GetComponent<Building>(); 
    }
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

[System.Serializable]
public class PersistentObject
{
    public int currentAmount;
    public int maxAmount;
    
    public float spawnDelay;
    private float _spawnDelay;
    
    public PersistentGenSettings settings;
    
    public PersistentObject(PersistentGenSettings obj)
    {
        this.settings = obj;
        maxAmount = obj.maxAmount;
        currentAmount = 0;
        spawnDelay = obj.spawnDelay;
        _spawnDelay = spawnDelay;
    }
    
    public void UpdateTimer()
    {
        if(_spawnDelay > 0){
            _spawnDelay -= Time.deltaTime;
        }
    }
    
    //public void Spawn(){_spawnDelay = spawnDelay;}
    
    public bool ReadyToSpawn() { return _spawnDelay <= 0f && currentAmount < maxAmount; }
    
    public void OnObjectSpawn()
    {
        currentAmount++;
        _spawnDelay = spawnDelay;
    }
    public void OnObjectDeleted()
    {
        currentAmount--;
    }
}