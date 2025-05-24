using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public Bar healthBar;
    public Bar staminaBar;
    
    public TextMeshProUGUI commandEnergy;
    
    public Bar playerPointsBar;
    public Bar enemyPointsBar;

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
        
        playerPointsBar = GameObject.Find("PlayerPoints_Bar").GetComponent<Bar>();
        EventBus.i.OnPlayerPointsChanged += playerPointsBar.UpdateBar;
        playerPointsBar.Init();
        
        enemyPointsBar = GameObject.Find("EnemyPoints_Bar").GetComponent<Bar>();
        EventBus.i.OnEnemyPointsChanged += enemyPointsBar.UpdateBar;
        enemyPointsBar.Init();
    }
    
    public void UpdateCommandEnergy(float energy, float maxEnergy)
    {
        commandEnergy.text = $"{energy:F0}/{maxEnergy}";
    }
}
