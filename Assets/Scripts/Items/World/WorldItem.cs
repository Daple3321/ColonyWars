using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public ItemData itemData;

    public Item originItem;

    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    
    public virtual void Initialize(ItemData data, Item origin)
    {
        itemData = data;
        name = data.itemName;
        originItem = origin;

        if (data.customPrefab == null)
        {
            meshFilter.mesh = data.itemMesh;
            meshRenderer.sharedMaterials = data.itemMaterials;
        }
    }

    public ItemData GetItemData() => itemData;
}
