using UnityEngine;
using PrimeTween;
using UnityEngine.UI;
using TMPro;

public class CommandMenu : MonoBehaviour
{
    private RectTransform rectTransform;
    Tween scaleTween;
    
    public bool isOpen = false;
    
    public RectTransform selectedUnitGroup;
    public RectTransform selectedCommanderGroup;
    
    public TextMeshProUGUI selectedUnitLabel;
    public TextMeshProUGUI selectedCommanderLabel;
    public TextMeshProUGUI unitNotSelectedLabel;
    
    [Space(6), Header("Buttons")]
    public Button unitMove;
    public Button unitFollow;
    public Button unitRemove;
    
    public Button commanderMove;
    public Button commanderFollow;
    public Button commanderAssemble;
    public Button commanderRemove;
    
    public Button deselectUnit;
    
    
    void Awake(){rectTransform = GetComponent<RectTransform>();}
    
    private SquadManager squadManager;
    public void Init(SquadManager squadManager)
    {
        this.squadManager = squadManager;
        squadManager.OnUnitSelect += u => SetupMenu();
        SetupButtons();
        
        Hide();
    }
    
    private void SetupButtons()
    {
        unitMove.onClick.AddListener(squadManager.UnitMove);
        unitFollow.onClick.AddListener(squadManager.UnitFollow);
        unitRemove.onClick.AddListener(squadManager.UnitRemove);
        
        commanderMove.onClick.AddListener(squadManager.CommanderSquadMove);
        commanderFollow.onClick.AddListener(squadManager.CommanderSquadFollow);
        commanderAssemble.onClick.AddListener(squadManager.CommanderAssemble);
        commanderRemove.onClick.AddListener(squadManager.CommanderClearSquad);
        
        deselectUnit.onClick.AddListener(squadManager.DeselectUnit);
    }
    
    private void SetupMenu()
    {
        if(squadManager.selectedUnit != null)
        {
            unitNotSelectedLabel.gameObject.SetActive(false);
            selectedUnitGroup.gameObject.SetActive(true);
            deselectUnit.gameObject.SetActive(true);
            selectedUnitLabel.text = $"{squadManager.selectedUnit.unitName} <color=yellow>Lv.{squadManager.selectedUnit.levelSystem.GetLevel()}</color>";
            
            if(squadManager.selectedUnit.InSquad()){
                unitFollow.gameObject.SetActive(true);
                unitRemove.gameObject.SetActive(true);
            }
            else{
                unitFollow.gameObject.SetActive(false);
                unitRemove.gameObject.SetActive(false);
            }
        }
        else{
            unitNotSelectedLabel.gameObject.SetActive(true);
            selectedUnitGroup.gameObject.SetActive(false);
            deselectUnit.gameObject.SetActive(false);
        }
        
        if(squadManager.selectedUnit is Commander c){
            selectedCommanderGroup.gameObject.SetActive(true);
            
            selectedCommanderLabel.text = $"{c.unitName}s Lv.{c.levelSystem.GetLevel()} squad";
        }
        else{
            selectedCommanderGroup.gameObject.SetActive(false);
        }
    }
    public void Show()
    {
        gameObject.SetActive(true);
        
        SetupMenu();
        
        isOpen = true;
        EventBus.i.OnInteractivePanelOpened?.Invoke();
        
        scaleTween.Stop();
        scaleTween = Tween.Scale(rectTransform, 1f, 0.25f, Ease.OutCubic);
    }
    public void Hide()
    {
        isOpen = false;
        EventBus.i.OnInteractivePanelClosed?.Invoke();
        
        scaleTween = Tween.Scale(rectTransform, 0, 0.2f, Ease.InCubic).OnComplete(()=>
            gameObject.SetActive(false)
        );
    }
}
