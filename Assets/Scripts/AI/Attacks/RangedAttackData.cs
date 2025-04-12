using UnityEngine;

[System.Serializable]
public class RangedAttackData : AttackData
{
    public int bulletsPerShot = 1;
    public float projectileSpeed = 40;
    public float projectileLifetime = 2.5f;
    public int penetrationAmount = 0;
    public Vector2Int projectileScatter = new Vector2Int(0, 10);
    public GameObject projectilePrefab;
}
