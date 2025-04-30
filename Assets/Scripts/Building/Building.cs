using System.Collections;
using UnityEngine;

public abstract class Building : MonoBehaviour, IDamageable, IClickable
{
    public BuildingData buildingData;
    
    public float health;
    public float maxHealth;
    
    public float interactionRange = 8;
    
    
    public Affiliation affiliation;
    
    [Space(10), Header("Build settings")]
    public float buildTime;
    public bool built = false;
    public float buildProgress = 0;
    
    private MeshRenderer[] meshes;

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
    
    public void Init()
    {
        meshes = transform.GetComponentsInChildren<MeshRenderer>();
        
        
    }
    
    public virtual void TakeDamage(float damage, float knockback = 0)
    {
        health -= damage;
        if(health <= 0){
            Death();
        }
    }
    public virtual void Death(){
        Destroy(gameObject);
    }

    public void OnClick(GameObject caller)
    {
        if(Vector3.Distance(transform.position, caller.transform.position) <= interactionRange){
            Debug.Log($"Clicked on {buildingData.buildingName}");
        }
    }
    
    public virtual void ChangeColor(Color color)
    {
        foreach(MeshRenderer mesh in meshes){
            mesh.material.SetColor("_BaseColor", color);
        }
    }
}
