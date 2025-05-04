using System;
using System.Collections;
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
    
    protected MeshRenderer[] meshes;

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
        
        this.buildingData = data;
    }
    
    public virtual void TakeDamage(float damage, float knockback = 0)
    {
        health -= damage;
        OnHealthChanged?.Invoke(health, maxHealth);
        if(health <= 0){
            Death();
        }
    }
    public virtual void Death(){
        OnBuildingDestroyed?.Invoke(this);
        Destroy(gameObject);
    }

    public virtual void OnClick(Player caller)
    {
        if(Vector3.Distance(transform.position, caller.transform.position) <= interactionRange){
            //Debug.Log($"Clicked on {buildingData.buildingName}");
            caller.buildingPanelManager.CreatePanel(this);
        }
    }
    
    public virtual void ChangeColor(Color color)
    {
        foreach(MeshRenderer mesh in meshes){
            mesh.material.SetColor("_BaseColor", color);
        }
    }
    
    public virtual BuildingPanel CreatePanel(RectTransform parentContainer)
    {
        GameObject go = Instantiate(GameAssets.buildingPanel, parentContainer);
        BuildingPanel panel = go.GetComponent<BuildingPanel>();
        panel.Init(this);
        return panel;
    }
}
