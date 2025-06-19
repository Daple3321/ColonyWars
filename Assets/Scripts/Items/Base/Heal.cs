using UnityEngine;

public class Heal : Usable
{
    public HealData healData;
    public Heal(ItemData data) : base(data)
    {
        Init(data);
        
        if(data is HealData hd){
            healData = hd;
        }
    }

    protected override void OnUse()
    {
        base.OnUse();
        
        GameController.p.AddHealth(healData.healAmount);
    }
}
