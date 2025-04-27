using UnityEngine;

public class HitscanLine : MonoBehaviour
{
    public LineRenderer lineRenderer;
    
    public void Init(Vector3 origin, Vector3 destination)
    {
        lineRenderer.SetPosition(1, new Vector3(0,0, Vector3.Distance(origin, destination)));
        transform.rotation = Quaternion.LookRotation(destination - origin, Vector3.up);
        Destroy(gameObject, 1.5f);
    }
}
