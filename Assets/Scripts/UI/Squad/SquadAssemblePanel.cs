using System.Collections.Generic;
using UnityEngine;

public class SquadAssemblePanel : MonoBehaviour
{
    public SquadManager squadManager;
    
    public List<NearbyUnitSlot> unitSlots = new List<NearbyUnitSlot>();
    public RectTransform gridParent;
    
    public CanvasGroup panelGroup;
    
    public void Init(SquadManager squadManager){
        unitSlots = new List<NearbyUnitSlot>();
        this.squadManager = squadManager;
        UpdateUI(new Unit[]{});
    }
    
    public void UpdateUI(Unit[] units)
    {
        ClearUI();
        
        foreach (Unit unit in units)
        {   
            if(!unit.InSquad()){
                GameObject slotObj = Instantiate(GameAssets.nearbyUnitSlot_Prefab, transform);
                NearbyUnitSlot slot = slotObj.GetComponent<NearbyUnitSlot>();
                slot.Init(unit, this);

                unitSlots.Add(slot);
            }
        }
    }
    
    public void DestroySlot(NearbyUnitSlot slotToDestroy)
    {
        unitSlots.Remove(slotToDestroy);
        Destroy(slotToDestroy.gameObject);
    }
    
    public void ClearUI()
    {
        foreach (NearbyUnitSlot slot in unitSlots)
        {
            Destroy(slot.gameObject);
        }
        unitSlots.Clear();
    }
}
