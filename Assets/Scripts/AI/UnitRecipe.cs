using UnityEngine;

[CreateAssetMenu(fileName = "Unit Recipe", menuName = "Crafting/Unit Recipe")]
public class UnitRecipe : ScriptableObject
{
    public ItemRequirements requirements;
    
    public float craftTime;
    
    public UnitData unit;
}
