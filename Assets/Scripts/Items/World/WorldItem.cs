using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public ItemData itemData;

    public Item originItem;

    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    
    public void Initialize(ItemData data, Item origin)
    {
        itemData = data;
        name = data.itemName;
        originItem = origin;

        if (data.customPrefab == null)
        {
            meshFilter.mesh = data.itemMesh;
            meshRenderer.sharedMaterials = data.itemMaterials;
        }
        else
        {
            
        }

        if (data is WeaponData weaponData)
        {
            Debug.Log($"Dropped weapon: {weaponData.itemName}, Damage: {weaponData.damage}");
        }
    }

    public ItemData GetItemData() => itemData;
}
