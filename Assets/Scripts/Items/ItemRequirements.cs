using System.Collections.Generic;
using System.Text;
using UnityEngine;

[System.Serializable]
public class ItemRequirements
{
    public List<ItemRequirement> requirements;
    
    public string GetRequirements()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("Requirements:\n");
        foreach(ItemRequirement requirement in requirements)
        {
            stringBuilder.Append($"{requirement.quantity} {requirement.item.itemName}\n");
        }
        
        return stringBuilder.ToString();
    }
    
    public string GetRequirementsCompare(int[] quantities)
    {
        StringBuilder stringBuilder = new StringBuilder();
        
        stringBuilder.Append($"<color={GameAssets.colors.GetHex(GameAssets.colors.craftReq)}>Requirements:</color>\n");
        //stringBuilder.Append($"<color=#{ColorUtility.ToHtmlStringRGBA(GameAssets.colors.craftReq)}>Requirements:</color>\n");
        
        for(int i = 0; i < requirements.Count; i++)
        {
            if(quantities[i] >= requirements[i].quantity)
            {
                stringBuilder.Append($"<color=green>{quantities[i]}/{requirements[i].quantity}</color> {requirements[i].item.itemName}\n");
            }
            else{
                stringBuilder.Append($"<color=red>{quantities[i]}/{requirements[i].quantity}</color> {requirements[i].item.itemName}\n");
            }
        }
        
        return stringBuilder.ToString();
    }
}

[System.Serializable]
public class ItemRequirement
{
    public ItemData item;
    public int quantity;
}