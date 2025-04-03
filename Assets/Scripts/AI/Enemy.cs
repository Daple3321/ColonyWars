using UnityEngine;

public class Enemy : Unit
{
    void Start()
    {
        base.Init();
    }

    public override void Death()
    {
        Destroy(gameObject);
    }
}
