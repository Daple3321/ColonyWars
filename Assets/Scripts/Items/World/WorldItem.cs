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

    public virtual void Attach(Transform attachTo)
    {
        transform.position = attachTo.position;
        transform.rotation = attachTo.rotation;

        transform.SetParent(attachTo);
    }

    public ItemData GetItemData() => itemData;
}
