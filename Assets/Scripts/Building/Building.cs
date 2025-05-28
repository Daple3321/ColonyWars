using System;
using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

public abstract class Building : MonoBehaviour, IDamageable, IClickable
{
    public BuildingData buildingData;
    
    public float health;
    public float maxHealth;
    
    public Action<float, float> OnHealthChanged;
    public Action<Building> OnBuildingDestroyed;
    
    public float interactionRange = 8;
    
    
    public Affiliation affiliation;
    
    [Space(10), Header("Build settings")]
    public float buildTime;
    public bool built = false;
    public float buildProgress = 0;
    
    public Cell cell;
    public Vector3Int cellIndex;
    public List<Cell> capturedCells;
    
    protected Renderer[] meshes;
    protected Material mat;
    protected MaterialPropertyBlock propertyBlock;

    public virtual IEnumerator Build()
    {
        float timeLeft = 0;
        while (timeLeft < buildTime)
        {
            buildProgress = timeLeft/buildTime;
            
            ChangeColor(Color.Lerp(Color.black, Color.white, buildProgress));
            timeLeft += Time.deltaTime;
            yield return null;
        }
        
        built = true;
        buildProgress = 1f;
        ChangeColor(Color.white);
    }
    
    public virtual void Init(BuildingData data)
    {
        meshes = transform.GetComponentsInChildren<MeshRenderer>();
        //mat = meshes[0].material;
        propertyBlock = new MaterialPropertyBlock();
        
        cellIndex = ColoniesManager.i.grid.WorldToCell(transform.position);
        cell = ColoniesManager.i.gridManager.GetCell(cellIndex.x, cellIndex.z);
        capturedCells = new List<Cell>();
        
        this.buildingData = data;
    }
    
    Sequence colorSeq;
    public virtual void TakeDamage(float damage, Vector3 knockback = new Vector3())
    {
        health -= damage;
        OnHealthChanged?.Invoke(health, maxHealth);
        
        colorSeq.Complete();
        colorSeq = Sequence.Create()
            .Chain(Tween.Custom(0f, 1f, duration: 0.2f, onValueChange: newVal => {
                        propertyBlock.SetColor("_BaseColor", Color.Lerp(Color.white, Color.red, newVal));
                        meshes[0].SetPropertyBlock(propertyBlock);
                    }))
            .Chain(Tween.Custom(1f, 0f, duration: 0.2f, onValueChange: newVal => {
                    propertyBlock.SetColor("_BaseColor", Color.Lerp(Color.white, Color.red, newVal));
                    meshes[0].SetPropertyBlock(propertyBlock);
                }));
        
            //.Chain(Tween.MaterialColor(mat, Color.red, 0.2f))
            //.Chain(Tween.MaterialColor(mat, Color.white, 0.2f));
        PopUpManager.i.Spawn(transform.position+new Vector3(0, 2f, 0), Color.white)
        .text = damage.ToString("F0");
        
        if(health <= 0){
            Death();
        }
    }
    public virtual void Death(){
        OnBuildingDestroyed?.Invoke(this);
        cell.OnCellBuildingsChanged?.Invoke();
        OnDeath();
        Destroy(gameObject);
    }
    public virtual void OnDeath()
    {
        
    }
    
    public virtual void CaptureCellInstant(int x, int y){
        Cell capturedCell = ColoniesManager.i.gridManager.GetCell(x, y);
        capturedCell.Capture(affiliation);
        capturedCells.Add(capturedCell);
        
        capturedCell.OnCaptured += (c, aff) => {if(aff != affiliation){ // callback hell
            capturedCells.Remove(c);
        }};
    }

    public virtual void OnClick(Player caller)
    {
        if(built && affiliation == Affiliation.Player)
        {
            if(Vector3.Distance(transform.position, caller.transform.position) <= interactionRange){
                //Debug.Log($"Clicked on {buildingData.buildingName}");
                caller.buildingPanelManager.CreatePanel(this);
            }
        }
    }
    
    public virtual void ChangeColor(Color color)
    {
        foreach(MeshRenderer mesh in meshes){
            propertyBlock.SetColor("_BaseColor", color);
            //mesh.material.SetColor("_BaseColor", color);
            mesh.SetPropertyBlock(propertyBlock);
        }
    }
    
    public virtual void PrepareUI()
    {
        
    }
    public virtual void ClearUI()
    {
        
    }
    
    public virtual BuildingPanel CreatePanel(RectTransform parentContainer)
    {
        GameObject go = Instantiate(GameAssets.buildingPanel, parentContainer);
        BuildingPanel panel = go.GetComponent<BuildingPanel>();
        //panel.Init(this);
        return panel;
    }
}
