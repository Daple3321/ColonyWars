using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Scriptable Objects/Units/Unit")]
public class UnitData : ScriptableObject
{
    public string unitName;
    [TextArea]
    public string description;
    
    public Sprite icon;
    
    public UnitRecipe recipe;
    
    public LootTable lootTable;
    
    public GameObject prefab;
}
