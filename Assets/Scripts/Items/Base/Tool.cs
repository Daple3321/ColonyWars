using System.Text;
using UnityEngine;

public class Tool : MeleeWeapon
{
    public Vector2Int yieldRange;
    public ItemData[] gatherResources;
    
    public Tool(ToolData _itemData) : base(_itemData)
    {
        Init(_itemData);
        yieldRange = _itemData.yieldRange;
        gatherResources = _itemData.gatherResources;
    }


    readonly StringBuilder str = new StringBuilder();
    public override string GetDescription()
    {
        str.Clear();
        str.AppendLine($"Damage: {damage.Value:F0}");
        str.AppendLine($"Yield: {yieldRange.x}-{yieldRange.y}");
        str.AppendLine("Can gather: ");
        foreach (var item in gatherResources){
            str.Append($"<color={Colors.GetHex(GameAssets.colors.rare)}>{item.itemName} | </color>");
        }
        str.AppendLine(description);
        return str.ToString();
    }
}
