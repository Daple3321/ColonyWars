using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Bar healthBar;
    public Bar staminaBar;
    
    public TextMeshProUGUI commandEnergy;
    
    public Bar playerPointsBar;
    public Bar enemyPointsBar;
    
    public Bar chargeBar;
    
    public RectTransform ammoCounter;
    public TextMeshProUGUI ammoText;
    public Image ammoIcon;

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
        
        chargeBar = GameObject.Find("ChargeBar").GetComponent<Bar>();
        chargeBar.Init();
        SwitchChargeBar(false);
        
        ammoCounter = GameObject.Find("AmmoCounter").GetComponent<RectTransform>();
        ammoText = ammoCounter.GetComponentInChildren<TextMeshProUGUI>();
        EventBus.i.PlayerAttackedWithAmmo += UpdateAmmoCount;
        ammoIcon = GameObject.Find("ammoIcon").GetComponent<Image>();
    }
    
    public void UpdateCommandEnergy(float energy, float maxEnergy)
    {
        commandEnergy.text = $"{energy:F0}/{maxEnergy}";
    }
    
    public void SwitchChargeBar(bool state)
    {
        if(chargeBar.gameObject.activeInHierarchy && !state){
            chargeBar.gameObject.SetActive(state);
        }
        else if(!chargeBar.gameObject.activeInHierarchy && state){
            chargeBar.gameObject.SetActive(state);
        }
    }
    
    public void UpdateChargeBar(float val)
    {
        chargeBar.UpdateBar(val, 1);
    }
    
    public void SwitchAmmoCounter(bool state)
    {
        if(ammoCounter.gameObject.activeInHierarchy && !state){
            ammoCounter.gameObject.SetActive(state);
        }
        else if(!ammoCounter.gameObject.activeInHierarchy && state){
            ammoCounter.gameObject.SetActive(state);
        }
    }
    public void SetAmmoIcon(Sprite icon)
    {
        ammoIcon.sprite = icon;
    }
    public void UpdateAmmoCount(float ammoLeft)
    {
        ammoText.text = ammoLeft.ToString("F0");
    }
}
