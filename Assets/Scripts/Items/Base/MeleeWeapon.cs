using System.Text;
using UnityEngine;

[System.Serializable]
public class MeleeWeapon : Weapon
{
    public float attackDuration;
    public AttackSequence attackSequence;

    public MeleeWeapon(MeleeWeaponData _itemData) : base(_itemData)
    {
        Init(_itemData);
        //LoadStats();
    }

    public override void LoadStats()
    {
        base.LoadStats();

        if (itemData is MeleeWeaponData weaponData)
        {
            attackSequence = weaponData.attackSequence;
            attackSequence.Init();
            damage.AddModifier(attackSequence.damageMod);
            recoilForce.AddModifier(attackSequence.recoilMod);
        }
        
    }
    
    public override void UpdateWeapon()
    {
        if (_attackCd > 0)
        {
            _attackCd -= Time.deltaTime;
        }
        
        attackSequence.UpdateSequence();
    }
    
    public override bool CanAttack()
    {
        if (attackType == AttackType.SINGLE && !isAttacking && mouseReleased && _attackCd <= 0 && CheckAmmo() 
            && GameController.p.movement.HasStamina(attackSequence.CurrentAttack().staminaDrain))
        {
            return true;
        }
        else if (attackType == AttackType.AUTOMATIC && !isAttacking && _attackCd <= 0 && CheckAmmo() 
            && GameController.p.movement.HasStamina(attackSequence.CurrentAttack().staminaDrain))
        {
            return true;
        }
        else if(attackType == AttackType.CHARGE && !isAttacking && _attackCd <= 0 && IsCharged() && CheckAmmo()
            && GameController.p.movement.HasStamina(attackSequence.CurrentAttack().staminaDrain))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void Attack()
    {
        isAttacking = true;
        _attackCd = attackSequence.CurrentAttack().duration;
        damage.OnModifierChanged(); // чтоб сделать isDirty = true;
        recoilForce.OnModifierChanged();
        
        // эээмммм....
        GameController.p.movement.AddStamina(-attackSequence.CurrentAttack().staminaDrain);
        GameController.p.movement.canRegenStamina = false;
        GameController.p.movement.StartStaminaRegenDelay(2f);
        GameController.p.playerAnimation.OnMeleeAttack(attackSequence.CurrentAttack().duration);
        // -----------
        
        attackSequence.Attack();
        attackCharge = 0;
        if(needsAmmo)
        {
            SubtractAmmo();
            //Debug.Log($"Ammo count: {GetAmmoInfo()}");
        }
        
        Debug.Log("ATTACKING");
    }
    
    public override string GetDescription()
    {
        StringBuilder str = new StringBuilder();
        str.AppendLine($"Damage: {damage.Value:F0}");
        str.AppendLine(description);
        return str.ToString();
    }
}
