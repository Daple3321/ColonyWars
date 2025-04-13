using PrimeTween;
using TMPro;
using UnityEngine;

public class UnitSlot : MonoBehaviour
{
    public Unit unit;
    public Bar healthBar;
    public TextMeshProUGUI nameText;
    
    private SquadPanel parentPanel;
    private RectTransform rectTransform;
    
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
