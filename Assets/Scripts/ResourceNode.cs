using UnityEngine;
using PrimeTween;

public class ResourceNode : MonoBehaviour, IClickable
{
    public ItemData resource;
    
    public bool isInfinite = true;
    public int quantity = 25;
    
    public float gatherRadius;
    
    private Collider col;
    private Material mat;

    void Awake()
    {
        mat = GetComponent<MeshRenderer>().material;
        col = GetComponent<Collider>();
    }

    public virtual void Gather()
    {
        if(!isInfinite)
        {
            if(quantity > 0){
                Shake();
                
                quantity--;
            }
            else{
                Destroy(gameObject);
            }
            
        }
        else{
            Shake();
        }
    }
    
    Sequence colorSeq;
    public void Shake()
    {
        Tween.ShakeScale(transform, strength: new Vector3(1.1f, 1.1f, 1.1f), duration: 0.25f, frequency: 2);
        colorSeq.Complete();
        colorSeq = Sequence.Create()
            .Chain(Tween.MaterialColor(mat, Color.blue, 0.15f))
            .Chain(Tween.MaterialColor(mat, Color.white, 0.15f));
    }

    public void OnClick(GameObject caller)
    {
        if(Vector3.Distance(transform.position, caller.transform.position) <= gatherRadius){
            //Debug.Log($"Clicked on {resource.name} node");
            
            GameController.p.playerInventory.inventory.AddItem(new Item(resource), 2); // <-- player mine yield stat here!
            Gather();
            
        }
    }
}
