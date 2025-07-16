using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class NearbyUnitSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Unit unit;
    public Bar healthBar;
    public TextMeshProUGUI nameText;
    
    private SquadAssemblePanel parentPanel;
    private RectTransform rectTransform;
    
    public RectTransform hoverPanel;
    
    private Squad originSquad;
    public void Init(Squad originSquad, Unit unit, SquadAssemblePanel squadPanel)
    {
        this.unit = unit;
        this.originSquad = originSquad;
        //unit.onUnitDeath += OnUnitDeath;
        unit.onUnregisterFromSquad += OnUnitUnregister;
        rectTransform = GetComponent<RectTransform>();
        
        healthBar.Init(Affiliation.Player);
        unit.onUnitHealthChanged += healthBar.UpdateBar;
        healthBar.UpdateBar(unit.GetHealth());
        
        parentPanel = squadPanel;
        
        nameText.text = unit.data.unitName;
        
        //Tween.PunchScale(rectTransform, strength: new Vector3(1.1f, 1.1f, 1), duration: 0.35f, frequency: 4);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        bool wasAdded = parentPanel.squadManager.AddUnit(originSquad, unit);
        if(wasAdded){
            parentPanel.DestroySlot(this);
        }
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
        //parentPanel.unitSlots.Remove(this);
        parentPanel.DestroySlot(this);
        //Destroy(gameObject);
    }
}
