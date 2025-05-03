using System;
using UnityEngine;

public class TriggerZone : Zone
{
    public LayerMask collisionMask;
    public Collider col;
    
    public event Action<GameObject> OnZoneEnter;
    public event Action<GameObject> OnZoneExit;

    protected virtual void OnTriggerEnter(Collider other)
    {
        OnZoneEnter?.Invoke(other.gameObject);
    }
    
    protected virtual void OnTriggerExit(Collider other)
    {
        OnZoneExit?.Invoke(other.gameObject);
    }
}
