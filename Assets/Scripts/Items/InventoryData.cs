using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Scriptable Objects/Items/InventoryData")]
public class InventoryData : ScriptableObject
{
    public List<InventoryItem> inventoryItems;
}
