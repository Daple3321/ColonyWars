using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public Bar healthBar;
    public Bar staminaBar;

    public void Init()
    {
        healthBar = GameObject.Find("HealthBar").GetComponent<Bar>();
        healthBar.Init();
        EventBus.i.PlayerHealthChanged += healthBar.UpdateBar;

        staminaBar = GameObject.Find("StaminaBar").GetComponent<Bar>();
        staminaBar.Init();
        EventBus.i.PlayerStaminaChanged += staminaBar.UpdateBar;
    }
}
