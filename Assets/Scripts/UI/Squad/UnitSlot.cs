using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Unit unit;
    public Bar healthBar;
    public TextMeshProUGUI nameText;
    
    private SquadPanel parentPanel;
    private RectTransform rectTransform;
    
    public RectTransform hoverPanel;
    
    public void Init(Unit unit, SquadPanel squadPanel)
    {
        this.unit = unit;
        //unit.onUnitDeath += OnUnitDeath;
        unit.onUnregisterFromSquad += OnUnitUnregister;
        rectTransform = GetComponent<RectTransform>();
        
        healthBar.Init(Affiliation.Player);
        unit.onUnitHealthChanged += healthBar.UpdateBar;
        healthBar.UpdateBar(unit.GetHealth());
        
        parentPanel = squadPanel;
        
        nameText.text = unit.unitName;
        
        Tween.PunchScale(rectTransform, strength: new Vector3(1.1f, 1.1f, 1), duration: 0.35f, frequency: 4);
    }

    public void RemoveUnit(){
        unit.squad.RemoveUnit(unit);
    }
    public void SelectUnit()
    {
        GameController.p.squadManager.SelectUnit(unit);
        // if(unit is Commander commander){
        // }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverPanel.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverPanel.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        unit.onUnregisterFromSquad -= OnUnitUnregister;
        unit.onUnitHealthChanged -= healthBar.UpdateBar;
    }

    private void OnUnitUnregister(){
        parentPanel.unitSlots.Remove(this);
        //Debug.Log($"{unit.unitName} unregistered. Deleting slot");
        Destroy(gameObject);
    }
}
