using UnityEngine;

public abstract class Weapon : Equipable, IWeapon
{
    
    
    public virtual void Attack()
    {
        Debug.Log($"Attacked with: {itemName}");
    }

    public abstract void LoadStats<T>(T stats);
}

public interface IWeapon
{
    void Attack();
}