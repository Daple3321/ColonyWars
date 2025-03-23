using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    public Transform player;

    void Start()
    {
        transform.SetParent(null);
    }

    void Update()
    {
        transform.position = player.position;
    }
}
