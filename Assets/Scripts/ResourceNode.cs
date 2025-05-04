using UnityEngine;
using PrimeTween;
using System;

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
    
    public Action<ResourceNode> OnResourceDeleted;
    public virtual void HandleClick()
    {
        if(!isInfinite)
        {
            if(quantity > 0 && clicksLeft <= 0){
                PlayerGather();
                quantity--;
            }
            else if(quantity <= 0){
                OnResourceDeleted?.Invoke(this);
                Destroy(gameObject);
            }
            
        }
        else if(isInfinite && clicksLeft <= 0){
            PlayerGather();
        }
        Shake();
    }
    
    public virtual Item GeneratorGather()
    {
        if(!isInfinite)
        {
            if(quantity > 0){
                quantity--;
                Shake();
                return new Item(resource);
            }
            else if(quantity <= 0){
                OnResourceDeleted?.Invoke(this);
                Destroy(gameObject);
            }
            
        }
        else{
            Shake();
            return new Item(resource);
        }
        
        return null;
    }
    
    public virtual void PlayerGather()
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

    public void OnClick(Player caller)
    {
        if(Vector3.Distance(transform.position, caller.transform.position) <= gatherRadius){
            
            clicksLeft--;
            HandleClick();
            //Gather();
        }
    }
}
