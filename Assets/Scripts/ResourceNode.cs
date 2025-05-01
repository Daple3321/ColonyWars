using UnityEngine;
using PrimeTween;

public class ResourceNode : MonoBehaviour, IClickable
{
    public ItemData resource;
    
    public bool isInfinite = true;
    public int quantity = 25;
    
    public float gatherRadius;
    
    public int clicksToGather = 1;
    public int clicksLeft = 1;
    
    private Collider col;
    private Material mat;

    void Awake()
    {
        mat = GetComponent<MeshRenderer>().material;
        col = GetComponent<Collider>();
        
        clicksLeft = clicksToGather;
    }
    
    
    public virtual void HandleClick()
    {
        if(!isInfinite)
        {
            if(quantity > 0 && clicksLeft <= 0){
                Gather();
                quantity--;
            }
            else if(quantity <= 0){
                Destroy(gameObject);
            }
            
        }
        else if(isInfinite && clicksLeft <= 0){
            Gather();
        }
        Shake();
    }
    public virtual void Gather()
    {
        GameController.p.playerInventory.TryAddItem(new Item(resource), 1);
        clicksLeft = clicksToGather;
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
            
            clicksLeft--;
            HandleClick();
            //Gather();
        }
    }
}
