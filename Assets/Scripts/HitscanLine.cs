using System;
using System.Collections;
using UnityEngine;

public class HitscanLine : MonoBehaviour
{
    public LineRenderer lineRenderer;
    
    public void Init(Vector3 origin, Vector3 destination)
    {
        //lineRenderer.SetPosition(1, new Vector3(0,0, Vector3.Distance(origin, destination)));
        transform.rotation = Quaternion.LookRotation(destination - origin, Vector3.up);
        StartCoroutine(LineLerp(origin, destination, 0.27f));
        //Destroy(gameObject, 0.45f);
    }
    
    private IEnumerator LineLerp(Vector3 origin, Vector3 destination, float duration)
    {
        float timeElapsed = 0;
        float finalLength = Vector3.Distance(origin, destination);
        while (timeElapsed < duration)
        {
            float progress = timeElapsed / duration;
            float length = Mathf.Lerp(0, finalLength, progress);
            lineRenderer.SetPosition(1, new Vector3(0,0, length));
            
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        
        yield return new WaitForSeconds(0.45f);
        Destroy(gameObject);
    }
}
