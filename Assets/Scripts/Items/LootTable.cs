using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using MackySoft.Choice;
using UnityEngine;

[System.Serializable]
public class LootTable
{
    [SerializedDictionary("Item", "Chance to roll")]
    public SerializedDictionary<LootElement, float> table;
    
    public Vector2Int rollAmountRange;
    
    public void DropLoot(Vector3 dropPos)
    {
        var selector = table.ToWeightedSelector(x => x.Value);
        
        int rolls = Random.Range(rollAmountRange.x, rollAmountRange.y);
        for(int i = 0; i < rolls; i++)
        {
            LootElement selectedItem = selector.SelectItemWithUnityRandom().Key;
            
            WorldItem droppedItem;
            int amount = Random.Range(selectedItem.amountRange.x, selectedItem.amountRange.y);
            droppedItem = selectedItem.item.SpawnItem(dropPos, amount);
            
            droppedItem.Drop(Vector3.up, 45f, 1.5f);
        }
    }
    
    public List<LootElement> Roll()
    {
        var selector = table.ToWeightedSelector(x => x.Value);
        List<LootElement> finalLoot = new List<LootElement>();
        
        int rolls = Random.Range(rollAmountRange.x, rollAmountRange.y);
        for(int i = 0; i < rolls; i++)
        {
            LootElement selectedItem = selector.SelectItemWithUnityRandom().Key;
            finalLoot.Add(selectedItem);
        }
        
        return finalLoot;
    }
    
    public bool IsEmpty(){
        return table.Count == 0;
    }
}

[System.Serializable]
public struct LootElement
{
    public ItemData item;
    public Vector2Int amountRange;
}