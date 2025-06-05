using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Objects/Items/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    [TextArea]
    public string description;
    public Sprite icon;
    public Rarity rarity;
    public bool IsStackable;

    public int id => GetInstanceID();

    public int maxStackSize = 1;
    
    public bool instantPickup = false;


    public Mesh itemMesh;
    public Material[] itemMaterials;
    public bool hasCustomScale = false;
    public Vector3 customScale = new Vector3(1,1,1);
    public GameObject customPrefab;
    
    public virtual Item CreateItemInstance(){
        return new Item(this);
    }
    
    public virtual WorldItem SpawnItem(Vector3 spawnPos, int quantity)
    {
        WorldItem worldItem;
        GameObject obj;
        if (customPrefab != null) // если есть кастомный префаб
        {
            obj = GameObject.Instantiate(customPrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            obj = GameObject.Instantiate(GameAssets.itemPrefab, spawnPos, Quaternion.identity);
        }
        
        if(hasCustomScale){
            obj.transform.localScale = customScale;
        }
        worldItem = obj.GetComponent<WorldItem>();
        worldItem.Initialize(this, CreateItemInstance(), quantity);
        //worldItem.Drop();

        return worldItem;
    }
}
