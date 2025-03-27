using UnityEngine;

[CreateAssetMenu(fileName = "New Ranged weapon", menuName = "Scriptable Objects/Items/Ranged weapon")]
public class RangedWeaponData : WeaponData
{
    public float shootDistance;
    public float shootInterval;
    public ShootStyle shootStyle;
}

