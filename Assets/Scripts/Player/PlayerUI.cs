using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public Bar healthBar;
    public Bar staminaBar;
    
    public TextMeshProUGUI commandEnergy;

    public void Init()
    {
        healthBar = GameObject.Find("HealthBar").GetComponent<Bar>();
        healthBar.Init(Affiliation.None, GameAssets.colors.playerHealthbar);
        EventBus.i.PlayerHealthChanged += healthBar.UpdateBar;

        staminaBar = GameObject.Find("StaminaBar").GetComponent<Bar>();
        staminaBar.Init(Affiliation.None, GameAssets.colors.playerStaminaBar);
        EventBus.i.PlayerStaminaChanged += staminaBar.UpdateBar;
        
        commandEnergy = GameObject.Find("commandEnergy_text").GetComponent<TextMeshProUGUI>();
        EventBus.i.PlayerCommandEnergyChanged += UpdateCommandEnergy;
    }
    
    public void UpdateCommandEnergy(float energy, float maxEnergy)
    {
        commandEnergy.text = $"{energy:F0}/{maxEnergy}";
    }
}
