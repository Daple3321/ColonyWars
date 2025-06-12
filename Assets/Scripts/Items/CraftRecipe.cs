using UnityEngine;

[CreateAssetMenu(fileName = "New Craft Recipe", menuName = "Crafting/Recipe")]
public class CraftRecipe : ScriptableObject
{
    public ItemRequirements requirements;
    
    public float craftTime;
    
    public ItemData finalItem;
    public int finalAmount;
}