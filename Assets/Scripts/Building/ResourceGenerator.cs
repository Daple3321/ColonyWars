using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceGenerator : Generator
{
    protected ResourceGeneratorData generatorData;
    
    public float gatherRate = 5f;
    private float _gatherRate = 5f;
    public int yieldAmount = 1;
    
    public List<ResourceNode> resourcesNearby;
    
    public override void Init(BuildingData data)
    {
        meshes = transform.GetComponentsInChildren<MeshRenderer>();
        resourcesNearby = new List<ResourceNode>();
        _gatherRate = gatherRate;
        
        if(data is ResourceGeneratorData resourceGeneratorData){
            this.generatorData = resourceGeneratorData;
        }
    }
    
    public override IEnumerator Build()
    {
        float timeLeft = 0;
        while (timeLeft < buildTime)
        {
            buildProgress = timeLeft/buildTime;
            
            ChangeColor(Color.Lerp(Color.black, Color.white, buildProgress));
            timeLeft += Time.deltaTime;
            yield return null;
        }
        
        built = true;
        buildProgress = 1f;
        ChangeColor(Color.white);
        
        resourcesNearby = CheckForResources().ToList();
        foreach(ResourceNode node in resourcesNearby){
            node.OnResourceDeleted += OnResourceDeleted;
        }
    }

    void Update()
    {
        if(resourcesNearby != null){
            HandleGathering();
        }
    }
    
    public void HandleGathering(){
        if(_gatherRate > 0){
            _gatherRate -= Time.deltaTime;
        }
        else{
            Gather();
            _gatherRate = gatherRate;
        }
    }

    public virtual void Gather()
    {
        foreach(ResourceNode node in resourcesNearby) // ошибка при удалении нода
        {
            node.GeneratorGather();
            
            // add to inventory
        }
    }
    
    public ResourceNode[] CheckForResources()
    {
        Collider[] resources = Physics.OverlapSphere(transform.position, generatorData.gatherRadius, generatorData.resourceMask);
        if(resources.Length > 0)
        {
            // это вообще уже НЕ НОРМАЛЬНО.
            Collider[] neededNodes = resources.ToList().FindAll(
                x => generatorData.resourceConditions.Contains(x.GetComponent<ResourceNode>().resource)
            ).ToArray();
            
            if(neededNodes.Length > 0)
            {
                ResourceNode[] nodes = new ResourceNode[neededNodes.Length];
                for(int i = 0; i < neededNodes.Length; i++){
                    nodes[i] = neededNodes[i].GetComponent<ResourceNode>();
                }
                return nodes;
            }
        }
        return null;
    }
    
    public void OnResourceDeleted(ResourceNode node)
    {
        resourcesNearby.Remove(node);
    }
}
