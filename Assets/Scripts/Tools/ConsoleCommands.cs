using UnityEngine;
using IngameDebugConsole;
using System.Collections.Generic;
using System.Linq;

public class ConsoleCommands : MonoBehaviour
{
    public List<ItemData> items;
    public List<UnitData> units;
    void Awake()
    {
        items = Resources.LoadAll<ItemData>("Items/").ToList();
        units = Resources.LoadAll<UnitData>("Units/").ToList();
        
        DebugLogConsole.AddCommandInstance( "give", "Gives item by name", nameof(GiveItem), this );
        DebugLogConsole.AddCommandInstance( "sens", "Changes mouse sensitivity", nameof(ChangeSens), this );
        DebugLogConsole.AddCommandInstance( "resetSens", "Resets mouse sensitivity to default", nameof(ResetSens), this );
        DebugLogConsole.AddCommandInstance( "unit", "Spawns unit of specified name and level", nameof(SpawnUnit), this );
    }

    [ConsoleMethod( "cube", "Creates a cube at specified position" )]
	public static void CreateCubeAt( Vector3 position )
	{
		GameObject.CreatePrimitive( PrimitiveType.Cube ).transform.position = position;
	}
    
    [ConsoleMethod( "starter", "Gives a starter kit" )]
    public static void Starter()
    {
        InventoryData kitStarter = Resources.Load<InventoryData>("Player/StartInv");
        GameController.p.playerInventory.inventory.LoadFromData(kitStarter.items);
    }
    
    public void GiveItem(string itemName, int quantity = 1)
    {
        ItemData foundItem = items.Find(x=> x.itemName == itemName);
        if (foundItem != null){
            Item item = foundItem.CreateItemInstance();
            
            GameController.p.playerInventory.TryAddItem(item, quantity);
        }
    }
    
    public void ChangeSens(float newSensitivty){
        GameController.p.cameraController.ChangeSensitivity(newSensitivty);
    }
    public void ResetSens(){
        GameController.p.cameraController.ResetSensitivity();
    }
    
    public void SpawnUnit(string unitName, int level = 1)
    {
        UnitData foundUnit = units.Find(x => x.unitName == unitName);
        if(foundUnit == null){Debug.LogError($"Unit with name {unitName} not found."); return;}
        
        if(PlayerAiming.Raycast(LayerMask.GetMask("Ground"), out RaycastHit hit))
        {
            GameObject go = Instantiate(foundUnit.prefab, hit.point, Quaternion.identity);
            Unit u = go.GetComponent<Unit>();
            u.Init();
            u.InitPatrolRoute();
            
            if(u is Enemy e){
                e.InitEnemy();
            }
            
            u.ChangeLevel(level);
        }
    }
}
