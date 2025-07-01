using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using MackySoft.Choice;
using UnityEngine;

public class ColonyBuilder : MonoBehaviour
{
    private EnemyColony owner;
    public void Init(EnemyColony owner)
    {
        this.owner = owner;
    }
    
    [Space(10), Header("Building pools")]
    [SerializedDictionary("Resource miners", "Chance to spawn")]
    public SerializedDictionary<BuildingData, float> resourcePool;
    [SerializedDictionary("Barracks", "Chance to spawn")]
    public SerializedDictionary<BuildingData, float> barracksPool;
    [SerializedDictionary("Storages", "Chance to spawn")]
    public SerializedDictionary<BuildingData, float> storagePool;
    [SerializedDictionary("Defense Towers", "Chance to spawn")]
    public SerializedDictionary<BuildingData, float> defensePool;
    [SerializedDictionary("Walls", "Chance to spawn")]
    public SerializedDictionary<BuildingData, float> wallPool;
    
    public void PerformBuildAction(ColonyActionType buildAction)
    {
        switch(buildAction){
        case ColonyActionType.Resources:
            BuildResources();
        break;
        
        case ColonyActionType.Storage:
            BuildStorage();
        break;
        
        case ColonyActionType.Barracks:
            BuildBarracks();
        break;
        
        case ColonyActionType.DefenseTower:
            BuildDefense();
        break;
        
        case ColonyActionType.Wall:
            BuildWall();
        break;
        
        default:
            Debug.LogError($"{buildAction} is not a build action!");
        break;
        }
    }
    
    protected BuildingData SelectBuildingFromPool(SerializedDictionary<BuildingData, float> pool)
    {
        IWeightedSelector<KeyValuePair<BuildingData, float>> selector;
        BuildingData selectedBuilding = null;
        
        selector = pool.ToWeightedSelector(x => x.Value);
        selectedBuilding = selector.SelectItemWithUnityRandom().Key;
        return selectedBuilding;
    }
    
    protected virtual void BuildResources()
    {
        Cell c = owner.capturedCells[Random.Range(0, owner.capturedCells.Count)];
        Vector3 pos = ColoniesManager.i.gridManager.RandomPointInCell(c);

        owner.Build(SelectBuildingFromPool(resourcePool), pos);
    }
    protected virtual void BuildStorage()
    {
        Cell c = owner.capturedCells[Random.Range(0, owner.capturedCells.Count)];
        Vector3 pos = ColoniesManager.i.gridManager.RandomPointInCell(c);
        
        owner.Build(SelectBuildingFromPool(storagePool), pos);
    }
    protected virtual void BuildBarracks()
    {
        Cell c = owner.capturedCells[Random.Range(0, owner.capturedCells.Count)];
        Vector3 pos = ColoniesManager.i.gridManager.RandomPointInCell(c);
        
        owner.Build(SelectBuildingFromPool(barracksPool), pos);
    }
    protected virtual void BuildDefense()
    {
        List<Vector2Int> filteredCells = ColoniesManager.i.gridManager.FilterCellsWithDiffNeighbours(owner.capturedCells);
        Vector2Int cellIdx;
        if(filteredCells.Count > 0){ // если нашли ячейки на границе с нейтральной/вражеской территорией
            cellIdx = filteredCells[Random.Range(0, filteredCells.Count)];
        }
        else{
            cellIdx = ColoniesManager.i.gridManager.ClosestCell(owner.cellIndex.x, owner.cellIndex.z, Affiliation.None);
        }
        //Cell c = owner.capturedCells[Random.Range(0, owner.capturedCells.Count)];
        Cell c = ColoniesManager.i.gridManager.GetCell(cellIdx.x, cellIdx.y);
        
        Vector3 pos = ColoniesManager.i.gridManager.RandomPointOnSide(c, owner.playerDirection, 4f);

        owner.Build(SelectBuildingFromPool(defensePool), pos, owner.playerDirection);
    }
    protected virtual void BuildWall()
    {
        List<Vector2Int> filteredCells = ColoniesManager.i.gridManager.FilterCellsWithDiffNeighbours(owner.capturedCells);
        Vector2Int cellIdx;
        if(filteredCells.Count > 0){ // если нашли ячейки на границе с нейтральной/вражеской территорией
            cellIdx = filteredCells[Random.Range(0, filteredCells.Count)];
        }
        else{
            cellIdx = ColoniesManager.i.gridManager.ClosestCell(owner.cellIndex.x, owner.cellIndex.z, Affiliation.None);
        }
        //Cell c = owner.capturedCells[Random.Range(0, owner.capturedCells.Count)];
        Cell c = ColoniesManager.i.gridManager.GetCell(cellIdx.x, cellIdx.y);
        
        Vector3 pos = ColoniesManager.i.gridManager.RandomPointOnSide(c, owner.playerDirection, 1.5f);

        owner.Build(SelectBuildingFromPool(wallPool), pos, owner.playerDirection);
    }
    public virtual void BuildOutpost(Vector2Int cell)
    {
        Vector3 outpostPos = ColoniesManager.i.grid.GetCellCenterWorld(new Vector3Int(cell.x, 0, cell.y));
        outpostPos = GameController.TerrainPoint(outpostPos);
        
        owner.Build(owner.outpost, outpostPos, owner.playerDirection);
    }
}
