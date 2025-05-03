using System;
using TMPro;
using UnityEngine;

public class BuildingPanel : MonoBehaviour
{
    public TextMeshProUGUI buildingName;
    public Bar healthBar;
    public RectTransform dataContainter;
    
    public Building building;
    
    
    public virtual void Init(Building building)
    {
        this.building = building;
        
        healthBar.Init(Affiliation.Player);
        building.OnHealthChanged += healthBar.UpdateBar;
        healthBar.UpdateBar(building.health, building.maxHealth);
        
        buildingName.text = building.buildingData.buildingName;
    }
    void OnDestroy(){
        building.OnHealthChanged -= healthBar.UpdateBar;
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide(Action OnComplete = null)
    {
        // tween
        gameObject.SetActive(false);
        OnComplete?.Invoke();
    }
}
