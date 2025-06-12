using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Scriptable Objects/Items/InventoryData")]
public class InventoryData : ScriptableObject
{
    //public List<InventoryItem> inventoryItems; // InventoryItem конечно для этого не удобный
    
    [SerializedDictionary("Item", "Quantity")]
    public SerializedDictionary<ItemData, int> items;
}
