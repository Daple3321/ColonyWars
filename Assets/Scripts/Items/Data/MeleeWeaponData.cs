using UnityEngine;

[CreateAssetMenu(fileName = "New Melee Weapon", menuName = "Scriptable Objects/Items/Melee Weapon")]
public class MeleeWeaponData : WeaponData
{
    public float attackDuration;
    public float attackCharge;
    public float attackInterval;
    
    public override Item CreateItemInstance(){
        return new MeleeWeapon(this);
    }
}
