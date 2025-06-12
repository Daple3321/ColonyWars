using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPanel : MonoBehaviour
{
    public TextMeshProUGUI buildingName;
    public Button destroyButton;
    public Button upgradesButton;
    public Bar healthBar;
    public RectTransform dataContainter;
    
    public Building building;
    
    private BuildingPanelManager manager;
    public virtual void Init(Building building, BuildingPanelManager manager)
    {
        this.building = building;
        this.manager = manager;
        
        healthBar.Init(Affiliation.Player);
        building.OnHealthChanged += healthBar.UpdateBar;
        healthBar.UpdateBar(building.health, building.maxHealth);
        
        buildingName.text = building.buildingData.buildingName;
        
        destroyButton.onClick.AddListener(DestroyButton);
    }
    protected virtual void OnDestroy(){
        building.OnHealthChanged -= healthBar.UpdateBar;
    }

    void Update()
    {
        HideIfOutsideRange();
    }
    
    private void HideIfOutsideRange()
    {
        if(Vector3.Distance(GameController.p.transform.position, building.transform.position) > building.interactionRange){
            manager.ClearCurrentPanel();
            //canHide = false;
        }
    }
    
    protected void DestroyButton()
    {
        building.Death();
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
