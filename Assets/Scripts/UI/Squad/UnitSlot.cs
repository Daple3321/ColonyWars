using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
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
        healthBar.UpdateBar(unit.health, unit.maxHealth);
        
        parentPanel = squadPanel;
        
        nameText.text = unit.unitName;
        
        Tween.PunchScale(rectTransform, strength: new Vector3(1.1f, 1.1f, 1), duration: 0.35f, frequency: 4);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // делать здесь set homePos
        unit.squad.RemoveUnit(unit); // как-то это не правильно
        //unit.UnregisterFromSquad();
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
