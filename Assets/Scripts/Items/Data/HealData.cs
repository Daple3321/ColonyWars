using UnityEngine;

[CreateAssetMenu(fileName = "Heal", menuName = "Scriptable Objects/Items/Heal")]
public class HealData : UsableData
{
    public float healAmount;
    
    public override Item CreateItemInstance(){
        return new Heal(this);
    }
}
