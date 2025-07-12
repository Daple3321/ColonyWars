using UnityEngine;
using IngameDebugConsole;
using System.Collections.Generic;
using System.Linq;

public class ConsoleCommands : MonoBehaviour
{
    public List<ItemData> items;
    void Awake()
    {
        items = Resources.LoadAll<ItemData>("Items/").ToList();
        
        DebugLogConsole.AddCommandInstance( "give", "Gives item by name", nameof(GiveItem), this );
        DebugLogConsole.AddCommandInstance( "sens", "Changes mouse sensitivity", nameof(ChangeSens), this );
        DebugLogConsole.AddCommandInstance( "resetSens", "Resets mouse sensitivity to default", nameof(ResetSens), this );
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
    
    public void GiveItem(string itemName, int quantity)
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
}
