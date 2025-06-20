using System;
using UnityEngine;

public class PersistentObjectTracker : MonoBehaviour
{
    public Action<PersistentObjectTracker> OnDestroyed;
    public PersistentObject source;

    private void OnDestroy()
    {
        OnDestroyed?.Invoke(this);
    }
    
    [ContextMenu("Destroy Object")]
    public void TestDestroy()
    {
        Destroy(gameObject);
    }
}
