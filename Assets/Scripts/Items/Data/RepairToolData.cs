using UnityEngine;

[CreateAssetMenu(fileName = "Repair Tool", menuName = "Scriptable Objects/Items/Repair tool")]
public class RepairToolData : MeleeWeaponData
{
    public float repairAmount = 15f;
    
    public ItemRequirements repairPrice;
    
    public override Item CreateItemInstance(){
        return new RepairTool(this);
    }
}
