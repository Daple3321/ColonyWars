using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemRequirements
{
    public List<ItemRequirement> requirements;
}

[System.Serializable]
public struct ItemRequirement
{
    public ItemData item;
    public int quantity;
}