using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    public Transform player;

    void Start()
    {
        transform.SetParent(null);
        EventBus.i.PlayerDeath += OnPlayerDeath;
    }

    void Update()
    {
        transform.position = player.position;
    }

    private void OnPlayerDeath()
    {
        enabled = false;
    }
}
