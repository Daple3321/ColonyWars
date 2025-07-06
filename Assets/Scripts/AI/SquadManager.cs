using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SquadManager : MonoBehaviour
{
    public Squad selectedSquad;
    public Unit selectedUnit;
    
    public float squadCallDistance;
    public LayerMask unitsMask;
    
    public int maxUnits;
    
    public CommandMenu commandMenu;
    public SquadPanel playerSquadUI;
    public SquadPanel selectedSquadUI;
    public SquadAssemblePanel squadAssembleUI;
    public float searchDelay;
    private float _searchDelay;
    public bool searchingForUnits = false;
    public List<Unit> nearbyUnits = new List<Unit>();

    void Awake(){
        enabled = false;
    }
    
    private Zone searchZone;
    public void Init()
    {
        SetupPlayerSquadUI();
        SetupSelectedSquadUI();
        
        DeselectUnit();
        selectedSquadUI.Hide();
        SwitchAssemblePanel();
        
        _searchDelay = searchDelay;
        searchZone = ZoneFactory.CreateZone(
            transform.position,
            GameAssets.colors.gatherRadius,
            ZoneShape.Cylinder,
            squadCallDistance);
        searchZone.transform.SetParent(transform, false);
        searchZone.transform.localPosition = Vector3.zero;
        searchZone.gameObject.SetActive(false);
        
        enabled = true;
    }
    private void SetupPlayerSquadUI()
    {
        commandMenu = GameController.i.commandMenu;
        commandMenu.Init(this);
        playerSquadUI = GameController.i.playerSquadPanel;
        squadAssembleUI = GameController.i.squadAssemblePanel;
        
        playerSquadUI.Init("Player", GameController.p.squad);
        squadAssembleUI.Init(GameController.p.squad, this);
        GameController.p.squad.onSquadUpdate += playerSquadUI.OnSquadUpdate;
    }
    private void SetupSelectedSquadUI()
    {
        selectedSquadUI = GameController.i.selectedSquadPanel;
        selectedSquadUI.Init("Commander", selectedSquad);
        
        selectedSquad.onSquadUpdate += selectedSquadUI.OnSquadUpdate;
    }
    
    public void OnUnitSelected(Unit unit)
    {
        if(unit == selectedUnit){
            DeselectUnit();
            return;
        }
        
        DeselectUnit();
        
        if(unit is Commander c){
            OnCommanderSelected(c);
        }
        
        selectedUnit = unit;
        Outline outline = selectedUnit.characterObject.AddComponent<Outline>();
        if(outline != null){
            outline.OutlineColor = GameAssets.colors.craftReq;
            outline.OutlineWidth = 4.5f;
        }
    }
    public void OnCommanderSelected(Commander commander)
    {
        //selectedCommander = commander;
        //selectedCommander.SwitchUnitSearching();
        selectedSquad = commander.ownedSquad;
        
        commander.SwitchUnitSearching();
        commander.ownedSquad.OutlineSquad();
        
        selectedSquadUI.Init("Commander", selectedSquad);
        selectedSquad.onSquadUpdate += selectedSquadUI.OnSquadUpdate;
        selectedSquadUI.Show();
    }
    public void CommanderSquadMove(){
        if(selectedUnit == null) {Debug.LogError("No selected unit!"); return;}
        
        if(selectedUnit is Commander c){
            Vector3 mousePos = Input.mousePosition;
            Ray mouseRay = Camera.main.ScreenPointToRay(mousePos);
            if (Physics.Raycast(mouseRay, out RaycastHit hit, 50, LayerMask.GetMask("Ground")))
            {
                c.ownedSquad.MoveOrder(hit.point);
            }
        }
    }
    public void CommanderSquadFollow(){
        if(selectedUnit == null) {Debug.LogError("No selected unit!"); return;}
        
        if(selectedUnit is Commander c){
            c.FollowOrder();
        }
    }
    public void CommanderAssemble(){
        if(selectedUnit == null) {Debug.LogError("No selected unit!"); return;}
        
        if(selectedUnit is Commander c){
            c.TryAssembleSquad();
        }
    }
    public void CommanderClearSquad(){
        if(selectedUnit == null) {Debug.LogError("No selected unit!"); return;}
        
         if(selectedUnit is Commander c){
            c.ClearSquad();
        }
    }
    public void DeselectUnit()
    {
        if(selectedUnit == null) return;
        
        if(selectedUnit.characterObject.TryGetComponent(out Outline o)){
            
            Destroy(o);
        }
        
        if(selectedUnit is Commander c){
            selectedSquad.onSquadUpdate -= selectedSquadUI.OnSquadUpdate;
            c.SwitchUnitSearching();
            c.ownedSquad.RemoveOutlines();
            
            selectedSquad = GameController.p.squad;
            selectedSquadUI.Hide();
        }
        
        selectedUnit = null;
    }
    
    public void UnitMove()
    {
        Vector3 mousePos = Input.mousePosition;
        Ray mouseRay = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(mouseRay, out RaycastHit hit, 50, LayerMask.GetMask("Ground")))
        {
            selectedUnit.SetHome(hit.point);
            selectedUnit.StopFollowing();
            selectedUnit.followTarget = null;
        }
    }
    public void UnitFollow()
    {
        if(!selectedUnit.InSquad()){
            Debug.LogWarning($"{selectedUnit.unitName} not in any squad", selectedUnit.gameObject);
            return;
        }
        
        selectedUnit.squad.FollowOrder(selectedUnit);
    }
    public void UnitRemove()
    {
        if(!selectedUnit.InSquad()){
            Debug.LogWarning($"{selectedUnit.unitName} not in any squad", selectedUnit.gameObject);
            return;
        }
        
        selectedUnit.squad.RemoveUnit(selectedUnit);
    }
    
    void Update()
    {
        if(_searchDelay > 0 && searchingForUnits){
            _searchDelay -= Time.deltaTime;
        }
        else if(_searchDelay <= 0 && searchingForUnits){
            SearchForUnits();
            squadAssembleUI.UpdateUI(nearbyUnits);
            _searchDelay = searchDelay;
        }
    }
    
    public void SwitchAssemblePanel()
    {
        if(squadAssembleUI.isActiveAndEnabled){
            searchingForUnits = false;
            squadAssembleUI.gameObject.SetActive(false);
            if(searchZone!=null){
                searchZone.gameObject.SetActive(false);
            }
            EventBus.i.OnInteractivePanelClosed?.Invoke();
        }
        else{
            searchingForUnits = true;
            //squadAssembleUI.ClearUI();
            searchZone.gameObject.SetActive(true);
            squadAssembleUI.gameObject.SetActive(true);
            EventBus.i.OnInteractivePanelOpened?.Invoke();
            
            
        }
    }
    public bool SearchForUnits()
    {
        var hitColliders = Physics.OverlapSphere(transform.position, squadCallDistance, unitsMask);
        nearbyUnits.Clear();
        if (hitColliders.Length > 0)
        {
            foreach (Collider col in hitColliders)
            {
                Unit hitUnit;
                if (col.TryGetComponent<Unit>(out hitUnit))
                {
                    nearbyUnits.Add(hitUnit);
                }
            }
            return true;
        }
        else{
            return false;
        }
    }
    
    public void FollowOrder(){
        GameController.p.squad.FollowOrder(transform);
    }
    public void HomePosOrder(Vector3 orderPos){
        GameController.p.squad.MoveOrder(orderPos);
    }
    public void UnitOrder(Vector3 orderPos, Unit targetUnit){
        GameController.p.squad.MoveOrder(orderPos, targetUnit);
    }
    public void ClearSquad(){
        GameController.p.squad.RemoveAllUnits();
    }
    
    public bool AddUnit(Squad targetSquad, Unit unit)
    {
        return targetSquad.TryAddUnit(unit);
    }
    
    public bool TryAssembleSquad()
    {
        var hitColliders = Physics.OverlapSphere(transform.position, squadCallDistance, unitsMask);
        if (hitColliders.Length > 0)
        {
            foreach (Collider col in hitColliders)
            {
                if (col.TryGetComponent(out Unit hitUnit))
                {
                    //Debug.Log($"Hit unit: {hitUnit.name}");
                    GameController.p.squad.TryAddUnit(hitUnit);
                }
            }
            //squad.FollowOrder(transform);

            return true;
        }
        else
        {
            Debug.Log("No units in radius to create a squad.");

            return false;
        }
    }
    
    // public bool HasSquad(){
    //     return squad.units.Count > 0 ? true : false;
    // }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.blue;
        
        if(!GameController.p.squad.IsEmpty()){
            //Handles.DrawWireDisc(squad.GetCenterPosition(), Vector3.one, 360, 0.35f);
            Handles.DrawWireCube(GameController.p.squad.GetCenterPosition(), Vector3.one);
        }
    }
#endif
}

public enum CommandType : byte
{
    NONE,
    FOLLOW,
    HOMEPOS,
    CREATE_SQUAD,
    CLEAR_SQUAD,
}