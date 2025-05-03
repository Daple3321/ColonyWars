using System.Text;
using UnityEngine;

[System.Serializable]
public class MeleeWeapon : Weapon
{
    public float attackDuration;
    public float attackCharge;
    public float attackInterval;

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
            attackDuration = weaponData.attackDuration;
            attackCharge = weaponData.attackCharge;
            attackInterval = weaponData.attackInterval;
        }
    }
    
    public override string GetDescription()
    {
        StringBuilder str = new StringBuilder();
        str.AppendLine(description);
        str.AppendLine("Damage: " + damage.ToString("F0"));
        return str.ToString();
    }
}
