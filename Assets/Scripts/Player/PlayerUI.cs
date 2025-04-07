using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public Bar healthBar;
    public Bar staminaBar;

    public void Init()
    {
        healthBar = GameObject.Find("HealthBar").GetComponent<Bar>();
        healthBar.Init(ref EventBus.i.PlayerHealthChanged);

        staminaBar = GameObject.Find("StaminaBar").GetComponent<Bar>();
        staminaBar.Init(ref EventBus.i.PlayerStaminaChanged);
    }
}
