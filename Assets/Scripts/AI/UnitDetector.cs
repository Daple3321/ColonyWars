using System;
using UnityEngine;
using static EnemyDetection;

public class UnitDetector : MonoBehaviour
{
    public DetectionShape detectionShape = DetectionShape.Sphere;
    
    public LayerMask unitMask;
    public Collider detection;
    
    public Action<Unit> OnUnitEnter;
    public Action<Unit> OnUnitExit;

    public void Init()
    {
        
    }


    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.layer == unitMask)
        {
            OnUnitEnter?.Invoke(col.GetComponent<Unit>());
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if(col.gameObject.layer == unitMask)
        {
            OnUnitExit?.Invoke(col.GetComponent<Unit>());
        }
    }
}
