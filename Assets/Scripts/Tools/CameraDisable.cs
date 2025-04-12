using UnityEngine;

public class CameraDisable : MonoBehaviour
{
    void Awake()
    {
        Destroy(gameObject);
    }
}
