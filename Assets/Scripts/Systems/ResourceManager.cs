using System;
using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager i { get; private set; }

    void Awake(){
        if (i != null){
            Destroy(this);
        }
        else{
            i = this;
        }
        
        propertyBlock = new MaterialPropertyBlock();
    }
    
    public event Action<ResourceData> OnResourceDeleted;
    
    private Dictionary<int, ResourceData> resources = new();
    private int nextId = 0;

    public int RegisterResource(GameObject obj, ItemData resource, int amount, int clicksToGather = 1) {
        int id = nextId++;
        resources[id] = new ResourceData(
            id, 
            obj, 
            resource, 
            obj.transform.position, 
            amount, 
            clicksToGather
        );
        
        
        return id;
    }
    
    public ResourceData TryGetResource(GameObject go){
        var res = resources.FirstOrDefault(x => x.Value.gameObject == go);
        if(res.Value != null){
            return res.Value;
        }
        else{
            return null;
        }
    }

    public Item GeneratorGather(int id, int amount) {
        if (!resources.TryGetValue(id, out var res)) return null;
        
        Shake(res.gameObject);
        res.amount -= amount;
        if (res.isDepleted) {
            GameObject.Destroy(res.gameObject);
            resources.Remove(id);
            OnResourceDeleted?.Invoke(res);
        }
        
        return res.resource.CreateItemInstance();
    }
    
    public void PlayerHarvest(GameObject resource, int amount) {
        var res = resources.FirstOrDefault(x => x.Value.gameObject == resource);
        if(res.Value == null) return;
        
        if(res.Value.HandleClick())
        {
            Item droppedResource = res.Value.resource.CreateItemInstance();
            WorldItem worldItem;
            worldItem = droppedResource.SpawnItem(resource.transform.position+new Vector3(0, 2, 0),  1);
            worldItem.Drop(resource.transform.up, 360, 1.5f);
            
            res.Value.amount -= amount;
        }
        
        Shake(res.Value.gameObject);
        
        if (res.Value.isDepleted) {
            GameObject.Destroy(res.Value.gameObject);
            resources.Remove(res.Value.id);
            OnResourceDeleted?.Invoke(res.Value);
        }
    }
    public void PlayerToolHarvest(GameObject resource, int yield) {
        var res = resources.FirstOrDefault(x => x.Value.gameObject == resource);
        if(res.Value == null) return;
        
        Item droppedResource = res.Value.resource.CreateItemInstance();
        WorldItem worldItem;
        worldItem = droppedResource.SpawnItem(resource.transform.position+new Vector3(0, 2, 0),  yield);
        worldItem.Drop(resource.transform.up, 360, 1.5f);
        
        res.Value.amount--;
        
        Shake(res.Value.gameObject);
        
        if (res.Value.isDepleted) {
            GameObject.Destroy(res.Value.gameObject);
            resources.Remove(res.Value.id);
            OnResourceDeleted?.Invoke(res.Value);
        }
        
        Debug.Log("Tool gather");
    }
    public bool CanGather(GameObject resource, ToolData tool){
        var res = resources.FirstOrDefault(x => x.Value.gameObject == resource);
        if(res.Value == null) return false;
        
        if(tool.gatherResources.Contains(res.Value.resource)){
            return true;
        }
        else{
            return false;
        }
    }

    public List<ResourceData> GetResourcesInRadius(Vector3 center, float radius, ItemData resourceFilter = null) {
        List<ResourceData> result = new List<ResourceData>();
        
        foreach (var res in resources.Values) 
        {
            if (res.isDepleted) continue;
            if (resourceFilter != null && res.resource != resourceFilter) continue;
            
            if (Vector3.Distance(center, res.position) <= radius) {
                result.Add(res);
            }
        }
        return result;
    }
    public List<ResourceData> GetResourcesInRadius(Vector3 center, float radius, ItemData[] resourceFilter = null) {
        List<ResourceData> result = new List<ResourceData>();
        
        foreach (var res in resources.Values) 
        {
            if (res.isDepleted) continue;
            if (resourceFilter == null) continue;
            
            foreach(ItemData filter in resourceFilter)
            {
                if (filter != res.resource){
                    continue;
                }
                else
                {
                    if (Vector3.Distance(center, res.position) <= radius) {
                        result.Add(res);
                    }
                }
            }
            
            // if (Vector3.Distance(center, res.position) <= radius) {
            //     result.Add(res);
            // }
        }
        return result;
    }
     public List<int> GetResourcesInRadius_Id(Vector3 center, float radius, ItemData[] resourceFilter = null) {
        List<int> result = new List<int>();
        
        foreach (var res in resources.Values) 
        {
            if (res.isDepleted) continue;
            if (resourceFilter == null) continue;
            
            foreach(ItemData filter in resourceFilter)
            {
                if (filter != res.resource)
                {
                    continue;
                }
                else
                {
                    if (Vector3.Distance(center, res.position) <= radius) {
                        result.Add(res.id);
                    }
                }
            }
            
            // if (Vector3.Distance(center, res.position) <= radius) {
            //     result.Add(res.id);
            // }
        }
        return result;
    }
    
    
    Sequence colorSeq;
    MaterialPropertyBlock propertyBlock;
    public void Shake(GameObject res)
    {
        //Renderer rend = res.GetComponent<Renderer>();
        
        Tween.ShakeScale(res.transform, strength: new Vector3(1.1f, 1.1f, 1.1f), duration: 0.25f, frequency: 2);
        // colorSeq.Complete();
        // colorSeq = Sequence.Create()
        // .Chain(Tween.Custom(0f, 1f, duration: 0.15f, onValueChange: newVal => {
        //         propertyBlock.SetColor("_BaseColor", Color.Lerp(Color.white, Color.blue, newVal));
        //         rend.SetPropertyBlock(propertyBlock);
        //     }))
        // .Chain(Tween.Custom(1f, 0f, duration: 0.15f, onValueChange: newVal => {
        //         propertyBlock.SetColor("_BaseColor", Color.Lerp(Color.white, Color.blue, newVal));
        //         rend.SetPropertyBlock(propertyBlock);
        //     }));
    }
}

[System.Serializable]
public class ResourceData {
    public int id;
    public GameObject gameObject;
    public ItemData resource;
    public Vector3 position;
    public int amount;
    public int clicksToGather = 1;
    private int clicksLeft;
    public bool isDepleted => amount <= 0;
    
    public enum ResourceMaterial : byte
    { // FOR EFFECTS
        None,
        Rock,
        Wood,
        Iron,
    }
    public ResourceMaterial resourceMaterial;
    
    public ResourceData(int id, GameObject gameObject, ItemData resource, Vector3 position, int amount, int clicksToGather = 1)
    {
        this.id = id;
        this.gameObject = gameObject;
        this.resource = resource;
        this.position = position;
        this.amount = amount;
        this.clicksToGather = clicksToGather;
        
        clicksLeft = clicksToGather;
    }
    
    public bool HandleClick()
    {
        if(clicksLeft <= 0){
            clicksLeft = clicksToGather;
            return true;
        }
        else{
            clicksLeft--;
            return false;
        }
    }
}