using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SquadPanel : MonoBehaviour
{
    public TextMeshProUGUI squadLabel;
    public TextMeshProUGUI squadStats;
    
    public Squad squad;
    
    public List<UnitSlot> unitSlots = new List<UnitSlot>();
    
    public CanvasGroup panelGroup;
    
    public bool hidden = false;
    
    public void Init(Squad squad)
    {
        unitSlots = new List<UnitSlot>();
        this.squad = squad;
        UpdateSquadUI(squad);
    }
    
    public void OnSquadUpdate(Squad newSquad)
    {
        UpdateSquadUI(newSquad);
    }
    
    public void UpdateSquadUI(Squad newSquad)
    {
        ClearSquadUI();
        
        if(squad.units.Count <= 0){
            Hide();
        }
        else{
            Show();
        }
        
        foreach (Unit unit in newSquad.units)
        {   
            GameObject slotObj = Instantiate(GameAssets.unitSlot_Prefab, transform);
            UnitSlot slot = slotObj.GetComponent<UnitSlot>();
            slot.Init(unit, this);
            
            unitSlots.Add(slot);
        }
        
        squadLabel.text = $"Player's squad {newSquad.units.Count}/{newSquad.maxUnits}";
        squadStats.text = $"| Damage: {newSquad.stats.meanDamage:F1} | ";
    }
    
    public void ClearSquadUI()
    {
        foreach(UnitSlot unitSlot in unitSlots)
        {
            Destroy(unitSlot.gameObject);
        }
        unitSlots.Clear();
        
        squadLabel.text = $"Player's squad 0/{squad.maxUnits}";
        squadStats.text = $"| Attack: 0 | ";
    }
    
    public void Show()
    {
        if(!isActiveAndEnabled){
            gameObject.SetActive(true);
            hidden = false;
        }
    }
    public void Hide()
    {
        gameObject.SetActive(false);
        hidden = true;
    }
    
    public void Switch()
    {
        if(isActiveAndEnabled){
            gameObject.SetActive(false);
            hidden = true;
        }
        else{
            gameObject.SetActive(true);
            hidden = false;
        }
    }
}
