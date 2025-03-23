using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    void Awake()
    {
        enabled = false;
    }

    public void Init()
    {
        enabled = true;
    }
}
