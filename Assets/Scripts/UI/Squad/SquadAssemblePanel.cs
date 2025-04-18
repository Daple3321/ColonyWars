using System.Collections.Generic;
using UnityEngine;

public class SquadAssemblePanel : MonoBehaviour
{
    public SquadManager squadManager;
    
    public List<NearbyUnitSlot> unitSlots = new List<NearbyUnitSlot>();
    public List<Unit> containedUnits = new List<Unit>();
    
    public RectTransform gridParent;
    
    public CanvasGroup panelGroup;
    
    public void Init(SquadManager squadManager){
        unitSlots = new List<NearbyUnitSlot>();
        containedUnits = new List<Unit>();
        this.squadManager = squadManager;
        UpdateUI(new List<Unit>{});
    }
    
    public void UpdateUI(List<Unit> nearbyUnits)
    {
        //ClearUI();
        
        foreach (Unit unit in nearbyUnits)
        {   
            if(!unit.InSquad())
            {
                if(!containedUnits.Contains(unit)){
                    containedUnits.Add(unit);
                    
                    GameObject slotObj = Instantiate(GameAssets.nearbyUnitSlot_Prefab, transform);
                    NearbyUnitSlot slot = slotObj.GetComponent<NearbyUnitSlot>();
                    slot.Init(unit, this);
                    Debug.Log("Spawning new slot");
                    
                    unitSlots.Add(slot);
                }
                
            }
            
            // NearbyUnitSlot foundSlot = unitSlots.Find(x => x.unit == unit);
            // if(foundSlot == null){ // удаляем слот если такого юнита больше нет в nearbyUnits
            //     containedUnits.Add(unit);
                    
            //     GameObject slotObj = Instantiate(GameAssets.nearbyUnitSlot_Prefab, transform);
            //     NearbyUnitSlot slot = slotObj.GetComponent<NearbyUnitSlot>();
            //     slot.Init(unit, this);
                    
            //     unitSlots.Add(slot);
            // }
            // else
            // {
            //     containedUnits.Remove(foundSlot.unit);
            //     DestroySlot(foundSlot);
            // }
        }
        
        List<NearbyUnitSlot> slotsToDestoy = new List<NearbyUnitSlot>();
        foreach(NearbyUnitSlot slot in unitSlots)
        {
            Unit foundUnit = nearbyUnits.Find(x => x == slot.unit);
            if(foundUnit == null)
            {
                slotsToDestoy.Add(slot);
            }
        }
        
        //Debug.Log($"Destroying {slotsToDestoy.Count} slots");
        foreach(NearbyUnitSlot slotToDestroy in slotsToDestoy)
        {
            //Debug.Log($"Destroying: {slotToDestroy}");
            DestroySlot(slotToDestroy);
        }
    }
    
    public void DestroySlot(NearbyUnitSlot slotToDestroy)
    {
        //Debug.Log("Destroying slot");
        containedUnits.Remove(slotToDestroy.unit);
        unitSlots.Remove(slotToDestroy);
        Destroy(slotToDestroy.gameObject);
    }
    
    public void ClearUI()
    {
        foreach (NearbyUnitSlot slot in unitSlots)
        {
            Destroy(slot.gameObject);
        }
        containedUnits.Clear();
        unitSlots.Clear();
    }
}
