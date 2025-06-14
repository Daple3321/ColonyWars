using UnityEngine;

[System.Serializable]
public class AttackSequence
{
    public float sequenceCoolDown = 1.5f;
    public AttackElement[] attacks;
    
    public StatModifier damageMod;
    public StatModifier recoilMod;
    
    private int currentAttack = 0;
    private float cd;
    
    public void Init()
    {
        cd = sequenceCoolDown;
        damageMod = new StatModifier(0, StatModType.PercentMult, 0, this);
        recoilMod = new StatModifier(0, StatModType.PercentMult, 0, this);
    }
    
    public void ResetSequence()
    {
        currentAttack = 0;
        cd = sequenceCoolDown;
        damageMod.Value = attacks[currentAttack].damageMultiplier;
        recoilMod.Value = attacks[currentAttack].recoilMultiplier;
    }
    
    public void UpdateSequence()
    {
        if(cd > 0){
            cd -= Time.deltaTime;
        }
        else{
            ResetSequence();
        }
    }
    
    public AttackElement CurrentAttack(){
        return attacks[currentAttack];
    }
    
    public bool Attack(){
        cd = sequenceCoolDown;
        damageMod.Value = attacks[currentAttack].damageMultiplier;
        recoilMod.Value = attacks[currentAttack].recoilMultiplier;
        
        if(currentAttack < attacks.Length-1)
        {
            currentAttack++;
            return true;
        }
        else{
            currentAttack = 0;
            return false;
        }
    }
}

[System.Serializable]
public struct AttackElement{
    public float duration;
    public float damageMultiplier;
    public float recoilMultiplier;
    public float staminaDrain;
}

public enum MeleeAttackType
{
    RAYCAST = 0, // Single target hit
    SPHERE = 1, // multiple hits
    BOX = 2, // multiple hits
    
}