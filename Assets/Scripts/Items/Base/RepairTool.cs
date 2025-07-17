using System.Text;
using UnityEngine;

public class RepairTool : MeleeWeapon
{
    public float repairAmount;
    public ItemRequirements repairPrice;
    
    public RepairTool(RepairToolData _itemData) : base(_itemData)
    {
        Init(_itemData);
        repairAmount = _itemData.repairAmount;
        repairPrice = _itemData.repairPrice;
    }


    readonly StringBuilder str = new StringBuilder();
    public override string GetDescription()
    {
        str.Clear();
        str.AppendLine($"Damage: {damage.Value:F0}");
        str.AppendLine($"Repair amount: {repairAmount}");
        str.AppendLine("Repair price: ");
        foreach (var item in repairPrice.requirements){
            str.Append($"<color={Colors.GetHex(GameAssets.colors.craftReq)}>{item.quantity} {item.item.itemName} | </color>");
        }
        str.AppendLine("\n"+description);
        return str.ToString();
    }
}
