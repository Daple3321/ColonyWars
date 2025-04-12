using UnityEngine;

public class WorldBar : Bar
{
    public Transform followTarget;
    public Vector3 offset;
    
    void Update()
    {
        if(followTarget != null)
            rectTransform.position = followTarget.position + offset;
    }
}
