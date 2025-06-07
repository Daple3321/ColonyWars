using UnityEngine;

[CreateAssetMenu(fileName = "New Ranged weapon", menuName = "Scriptable Objects/Items/Ranged weapon")]
public class RangedWeaponData : WeaponData
{
    [Space(7), Header("Ranged weapon data")]
    public int projectilesPerShot = 1;
    public float shootDistance;
    public float projectileSpeed;
    public int penetrationAmount;
    public float projectileLifetime;
    public float knockBackForce;
    public AnimationCurve concentrationScatter;
    public ShootStyle shootStyle;
    
    public override Item CreateItemInstance(){
        return new RangedWeapon(this);
    }
}

