using System.Collections;
using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public ItemData itemData;

    [SerializeReference] public Item originItem;
    
    public int quantity;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    
    public Transform attachmentPoint;
    
    public bool dropped = false;
    
    public virtual void Initialize(ItemData data, Item origin, int quantity = 1)
    {
        itemData = data;
        name = data.itemName;
        originItem = origin;
        gameObject.layer = 8; // items
        this.quantity = quantity;   

        if (data.customPrefab == null)
        {
            meshFilter.mesh = data.itemMesh;
            meshRenderer.sharedMaterials = data.itemMaterials;
        }
    }
    
    public virtual void Drop()
    {
        LootDrop drop = gameObject.AddComponent<LootDrop>();
        drop.onDropped += OnDropped;
        drop.StartDrop();
    }
    public virtual void Drop(Vector3 direction, float angleDisp = 25f, float velocityMultiplier = 1f)
    {
        LootDrop drop = gameObject.AddComponent<LootDrop>();
        drop.onDropped += OnDropped;
        drop.StartDrop(direction, angleDisp, velocityMultiplier);
    }
    public virtual void OnDropped()
    {
        dropped = true;
        // drop effect?
        // item halo
    }
    
    public IEnumerator DelayedAttach(Transform attachTo)
    {
        yield return new WaitForSeconds(0.5f);
        Attach(attachTo);
    }
    public virtual void Attach(Transform attachTo)
    {
        if (attachmentPoint != null)
        {
            Vector3 posCorrection = attachTo.position - attachmentPoint.position;
            transform.position += posCorrection;
        }
        else
        {
            transform.position = attachTo.position;
        }
        transform.rotation = attachTo.rotation;

        transform.SetParent(attachTo, true);
    }

    public ItemData GetItemData() => itemData;
}
