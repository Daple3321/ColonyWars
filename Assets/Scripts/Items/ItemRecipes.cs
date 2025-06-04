using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemRecipes
{
    public List<ItemRecipe> recipes;
}

[System.Serializable]
public struct ItemRecipe
{
    public ItemRequirements itemRequirements;
    
    public float craftTime;
    
    public ItemData finalItem;
    public int finalAmount;
    
    public static ItemRecipe GetEmpty()
    {
        return new ItemRecipe();
    }
}