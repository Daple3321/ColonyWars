using UnityEngine;

[CreateAssetMenu(fileName = "New Ranged weapon", menuName = "Scriptable Objects/Items/Ranged weapon")]
public class RangedWeaponData : WeaponData
{
    public float shootDistance;
    public float projectileSpeed;
    public int penetrationAmount;
    public float projectileLifetime;
    public AnimationCurve concentrationScatter;
    public ShootStyle shootStyle;
}

