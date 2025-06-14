using UnityEngine;

[CreateAssetMenu(fileName = "New Melee Weapon", menuName = "Scriptable Objects/Items/Melee Weapon")]
public class MeleeWeaponData : WeaponData
{
    [Space(7), Header("Melee weapon data")]
    public float attackDistance = 2f;
    public float knockBackForce;
    public Vector3 attackBoxExtents;
    
    public AttackSequence attackSequence;
    
    public MeleeAttackType meleeAttackType;
    
    public override Item CreateItemInstance(){
        return new MeleeWeapon(this);
    }
}