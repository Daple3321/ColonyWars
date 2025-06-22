using UnityEngine;

[CreateAssetMenu(fileName = "Tool", menuName = "Scriptable Objects/Items/Tool")]
public class ToolData : MeleeWeaponData
{
    public ItemData[] gatherResources;
    public Vector2Int yieldRange;
    
    public override Item CreateItemInstance(){
        return new Tool(this);
    }
}
