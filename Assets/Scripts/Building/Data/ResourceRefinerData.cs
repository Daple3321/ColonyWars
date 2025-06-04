using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceRefiner", menuName = "Scriptable Objects/Buildings/Resource Refiner")]
public class ResourceRefinerData : BuildingData
{
    public List<ItemRecipe> recipes;
    
    public float refineSpeed;
}
