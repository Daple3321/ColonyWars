using TMPro;
using UnityEngine;

public class BuildingHoverPanel : MonoBehaviour
{
    public WorldBar healthBar;
    
    public TextMeshProUGUI infoText;
    
    public void Init(Building building)
    {
        healthBar.Init(building.affiliation);
        healthBar.barDesc.text = building.buildingData.buildingName;
        building.OnHealthChanged += healthBar.UpdateBar;
        healthBar.UpdateBar(building.health, building.maxHealth);
    }
    
    public void UpdateInfo(Building building){
        healthBar.UpdateBar(building.health, building.maxHealth);
        //infoText.text = building.buildingData;
    }
    
    public void ShowForBuilding(Building building, string info = ""){
        transform.position = building.transform.position + new Vector3(0,3.2f,0);
        gameObject.SetActive(true);
        infoText.text = info;
        UpdateInfo(building);
    }
    
    public void Hide(Building building){
        if(building != null){
            building.OnHealthChanged -= healthBar.UpdateBar;
        }
        gameObject.SetActive(false);
    }
}
