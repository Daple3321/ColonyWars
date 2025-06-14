using UnityEngine;

[System.Serializable]
public class MeleeAttackData : AttackData
{
    public float knockBackForce;
    public Vector3 attackBoxExtents;
    
    public AttackSequence attackSequence;
    
    public MeleeAttackType meleeAttackType;
}
