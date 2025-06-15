using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Barracks", menuName = "Scriptable Objects/Buildings/Barracks")]
public class BarracksData : BuildingData
{
    public List<UnitRecipe> recipes;
}
