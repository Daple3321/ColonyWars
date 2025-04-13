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
    
    public void Init(Squad squad)
    {
        unitSlots = new List<UnitSlot>();
        this.squad = squad;
        SetupSquadUI(squad);
    }
    
    public void OnSquadUpdate(Squad newSquad)
    {
        SetupSquadUI(newSquad);
    }
    
    public void SetupSquadUI(Squad newSquad)
    {
        ClearSquadUI();
        
        float meanDmg = 0;
        foreach (Unit unit in newSquad.units)
        {   
            GameObject slotObj = Instantiate(GameAssets.unitSlot_Prefab, transform);
            UnitSlot slot = slotObj.GetComponent<UnitSlot>();
            slot.Init(unit, this);
            
            meanDmg += unit.damage;
            
            unitSlots.Add(slot);
        }
        
        meanDmg /= newSquad.units.Count;
        squadLabel.text = $"Player's squad {newSquad.units.Count}/{newSquad.maxUnits}";
        squadStats.text = $"| Attack: {meanDmg} | ";
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
}
