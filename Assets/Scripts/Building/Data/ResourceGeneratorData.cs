using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New ResourceGen", menuName = "Scriptable Objects/Buildings/Resource Generator")]
public class ResourceGeneratorData : BuildingData
{
    public ItemData[] resourceConditions;
    
    public float gatherRadius = 5f;
    public float resourceQuantityMultiplier = 1f;
    
    public LayerMask resourceMask;
    
    public override Zone[] CreateProjectionZones()
    {
        Zone[] zones = new Zone[1];
        // Zone overlapZone = ZoneFactory.CreateZone(
        //     PlayerAiming.worldMouseFollower.transform.position,
        //     GameAssets.colors.buildingOverlap,
        //     ZoneShape.Cylinder,
        //     overlapRadius);
        
        // overlapZone.transform.SetParent(PlayerAiming.worldMouseFollower.transform);
        // overlapZone.transform.localPosition = Vector3.zero;
        
        Zone gatherZone = ZoneFactory.CreateZone(
            PlayerAiming.worldMouseFollower.transform.position,
            GameAssets.colors.gatherRadius,
            ZoneShape.Cylinder,
            gatherRadius);
        
        gatherZone.transform.SetParent(PlayerAiming.worldMouseFollower.transform);
        gatherZone.transform.localPosition = Vector3.zero;
        
        //zones[0] = overlapZone;
        zones[0] = gatherZone;
        
        return zones;
    }
    
    public Action<Collider[]> OnOverlapResources;
    //public List<int> previousResources = new();
    public override bool CheckBuildConditions(Vector3 projectionPos, BuildProjection buildProjection = null)
    {
        // Collider[] resources = Physics.OverlapSphere(projectionPos, gatherRadius, resourceMask);
        // if(resources.Length > 0)
        // {
        //     // это вообще уже НЕ НОРМАЛЬНО.
        //     Collider[] neededNodes = resources.ToList().FindAll(
        //         x => x.GetComponent<ResourceNode>().resource == resourceCondition
        //     ).ToArray();
            
        //     if(neededNodes.Length > 0){
        //         Debug.Log($"Overlapping {neededNodes.Length} coal");
        //         return true;
        //     }
        // }
        
        if(!base.CheckBuildConditions(projectionPos)){
            return false;
        }
        
        // Collider[] resources = new Collider[8];
        // if(Physics.OverlapSphereNonAlloc(projectionPos, gatherRadius, resources, resourceMask) > 0)
        // {
        //     // это вообще уже НЕ НОРМАЛЬНО.
        //     Collider[] neededNodes = resources.ToList().FindAll(
        //         x => x != null && 
        //         resourceConditions.Contains(x.GetComponent<ResourceNode>().resource)
        //         //x.GetComponent<ResourceNode>().resource == resourceCondition
        //     ).ToArray();
            
        //     if(neededNodes.Length > 0){
        //         OnOverlapResources?.Invoke(neededNodes);
        //         return true;
        //     }
        // }
        
        List<int> foundResources = ResourceManager.i.GetResourcesInRadius_Id(projectionPos, gatherRadius, resourceConditions);
        List<int> outlinesToDelete = new();
        // foreach(int foundRes in foundResources)
        // {
        //     if(!previousResources.Contains(foundRes)){
        //         outlinesToDelete.Add(foundRes);
        //     }
        // }
        foreach(int prevRes in buildProjection.previousResources)
        {
            if(!foundResources.Contains(prevRes)){
                outlinesToDelete.Add(prevRes);
            }
        }
        ResourceManager.i.DeleteOutlines(outlinesToDelete.ToArray());
        
        if(foundResources.Count > 0)
        {
            buildProjection.previousResources.Clear();
            buildProjection.previousResources.AddRange(foundResources);
            ResourceManager.i.OutlineResources(foundResources.ToArray());
            return true;
        }
        
        
        return false;
    }
}
