using Cysharp.Threading.Tasks;
using PrimeTween;
using TMPro;
using UnityEngine;
using static EntityStatType;

public class BuildingHoverPanel : MonoBehaviour
{
    public WorldBar healthBar;
    
    public TextMeshProUGUI infoText;
    
    public int showDelay = 700;
    private bool delayOver = true;
    
    [SerializeField] private RectTransform rectTransform;
    public void SetupForBuilding(Building building)
    {
        healthBar.Init(building.affiliation);
        healthBar.barDesc.text = $"{building.buildingData.buildingName} <color=yellow>Lv.{building.levelSystem.GetLevel()}</color>";
        building.OnHealthChanged += healthBar.UpdateBar;
        healthBar.UpdateBar(building.health, building.buildingStats[maxHealth].Value);
    }
    
    public void UpdateBar(Building building){
        healthBar.UpdateBar(building.health, building.buildingStats[maxHealth].Value);
    }
    public void UpdateInfo(string info = ""){
        infoText.text = info;
    }
    
    public void Show(Building building, string info = "")
    {
        ShowForBuilding(building, info);
        // if(delayOver){
        //     //ShowForBuilding(building, info).Forget();
        // }
    }
    
    // private async UniTask ShowForBuilding(Building building, string info = "")
    // {
    //     delayOver = false;
        
    //     await UniTask.Delay(showDelay);
        
    //     gameObject.SetActive(true);
        
    //     Tween.ScaleY(rectTransform, 1f, 0.35f, Ease.OutCubic);
    //     transform.position = building.transform.position + new Vector3(0,4.2f,0);
    //     infoText.text = info;
    //     UpdateBar(building);
        
    //     delayOver = true;
    // }
    
    private void ShowForBuilding(Building building, string info = ""){
        transform.position = building.transform.position + new Vector3(0,4.2f,0);
        gameObject.SetActive(true);
        infoText.text = info;
        UpdateBar(building);
    }
    
    public void Hide(Building building){
        if(building != null){
            building.OnHealthChanged -= healthBar.UpdateBar;
        }
        //rectTransform.localScale = new Vector3(1, 0, 1);
        gameObject.SetActive(false);
    }
}
