using UnityEngine;

[System.Serializable]
public class HitscanAttackData : AttackData
{
    public int projectilesPerShot = 1;
    public int penetrationAmount = 0;
    public float knockBackForce = 0;
    public Vector2Int projectileScatter = new Vector2Int(0, 10);
}
