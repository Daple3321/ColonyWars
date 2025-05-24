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
    
    [SerializeField] private Collider col;
    [SerializeField] private Renderer rend;
    //[SerializeField] private Material mat;
    private MaterialPropertyBlock propertyBlock;

    void Awake()
    {
        //mat = rend.material;
        propertyBlock = new MaterialPropertyBlock();
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
        Item droppedResource = new Item(resource);
        
        WorldItem worldItem;
        worldItem = droppedResource.SpawnItem(transform.position+new Vector3(0, 2, 0),  1);
        worldItem.Drop(transform.up, 360, 1.5f);
        
        //GameController.p.playerInventory.TryAddItem(new Item(resource), 1);
        clicksLeft = clicksToGather;
    }
    
    Sequence colorSeq;
    public void Shake()
    {
        Tween.ShakeScale(transform, strength: new Vector3(1.1f, 1.1f, 1.1f), duration: 0.25f, frequency: 2);
        colorSeq.Complete();
        colorSeq = Sequence.Create()
            // .Chain(Tween.Custom(0f, 1f, duration: 0.15f, onValueChange: newVal => colorProgress = newVal))
            // .InsertCallback(0, ()=>{propertyBlock.SetColor("_Color", Color.Lerp(Color.white, Color.blue, colorProgress));})
            // .Chain(Tween.Custom(1f, 0f, duration: 0.15f, onValueChange: newVal => colorProgress = newVal))
            // .InsertCallback(0, ()=>{propertyBlock.SetColor("_Color", Color.Lerp(Color.white, Color.blue, colorProgress));})
            
            //.InsertCallback(0, ()=> rend.SetPropertyBlock(propertyBlock))
            .Chain(Tween.Custom(0f, 1f, duration: 0.15f, onValueChange: newVal => {
                    propertyBlock.SetColor("_BaseColor", Color.Lerp(Color.white, Color.blue, newVal));
                    rend.SetPropertyBlock(propertyBlock);
                }))
            .Chain(Tween.Custom(1f, 0f, duration: 0.15f, onValueChange: newVal => {
                    propertyBlock.SetColor("_BaseColor", Color.Lerp(Color.white, Color.blue, newVal));
                    rend.SetPropertyBlock(propertyBlock);
                }));
            // .ChainCallback(()=>{
            //     propertyBlock.SetColor("_BaseColor", Color.white);
            //     rend.SetPropertyBlock(propertyBlock);
            // });
            
            //.Chain(Tween.MaterialColor(mat, Color.blue, 0.15f))
            //.Chain(Tween.MaterialColor(mat, Color.white, 0.15f));
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
