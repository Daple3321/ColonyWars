using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SquadManager : MonoBehaviour
{
    public Squad selectedSquad;
    public Commander selectedCommander;
    
    public float squadCallDistance;
    public LayerMask unitsMask;
    
    public int maxUnits;
    
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
        
        SquadDeselect();
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
    public void OnSquadSelected(Commander commander, Squad squad)
    {
        selectedCommander = commander;
        //selectedCommander.SwitchUnitSearching();
        selectedSquad = squad;
        
        selectedSquadUI.Init("Commander", selectedSquad);
        selectedSquad.onSquadUpdate += selectedSquadUI.OnSquadUpdate;
        selectedSquadUI.Show();
    }
    public void CommanderAssemble(){
        if(selectedCommander == null) {Debug.LogError("No selected commander!"); return;}
        
        selectedCommander.TryAssembleSquad();
    }
    public void CommanderClearSquad(){
        if(selectedCommander == null) {Debug.LogError("No selected commander!"); return;}
        
        selectedCommander.ClearSquad();
    }
    public void SquadDeselect(){
        selectedSquad.onSquadUpdate -= selectedSquadUI.OnSquadUpdate;
        
        selectedSquad = GameController.p.squad;
        selectedSquadUI.Hide();
        //selectedCommander.SwitchUnitSearching();
        selectedCommander = null;
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
        selectedSquad.FollowOrder(transform);
    }
    public void HomePosOrder(Vector3 orderPos){
        selectedSquad.MoveOrder(orderPos);
    }
    public void UnitOrder(Vector3 orderPos, Unit targetUnit){
        selectedSquad.MoveOrder(orderPos, targetUnit);
    }
    public void ClearSquad(){
        selectedSquad.RemoveAllUnits();
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
                    selectedSquad.TryAddUnit(hitUnit);
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