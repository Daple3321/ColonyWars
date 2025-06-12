using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Crafting Station", menuName = "Scriptable Objects/Buildings/Crafting Station")]
public class CraftingStationData : BuildingData
{
    public List<CraftRecipe> recipes;
}
