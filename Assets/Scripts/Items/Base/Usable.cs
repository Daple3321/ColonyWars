using UnityEngine;

public class Usable : Item
{
    public UsableData usableData;
    public Usable(ItemData data) : base(data)
    {
        Init(data);
        
        if(data is UsableData ud){
            usableData = ud;
            uses = ud.uses;
        }
    }
    
    public virtual void Use(float useAmount)
    {
        if(!usableData.isInfinite && CheckUses()){
            uses -= useAmount;
        }
        charge = 0f;
        EventBus.i.PlayerAttackedWithAmmo?.Invoke(uses);
        
        OnUse();
    }
    
    protected virtual void OnUse(){}
    
    public float uses;
    public float charge = 0f;
    
    public bool mouseReleased = true;
    
    public virtual bool CanUse()
    {
        if(!CheckUses()) return false;
        
        if(usableData.useType == UseType.INSTANT)
        {
            return true;
        }
        else if (usableData.useType == UseType.HOLD_INSTANT && IsCharged())
        {
            return true;
        }
        else if (usableData.useType == UseType.HOLD_ACTIVE)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public virtual void Charge(){
        charge += usableData.chargeRate * Time.deltaTime;
    }
    
    public virtual bool IsCharged(){
        return charge >= 1f;
    }
    
    public bool CheckUses(){
        if(usableData.isInfinite){
            return true;
        }
        else{
            return uses > 0f;
        }
    }
}
