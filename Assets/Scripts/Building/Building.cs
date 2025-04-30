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
    public virtual IEnumerator Build()
    {
        float timeLeft = 0;
        while (timeLeft < buildTime)
        {
            timeLeft += Time.deltaTime;
            yield return null;
        }
        
        built = true;
    }
    
    public virtual void TakeDamage(float damage, float knockback = 0)
    {
        health -= damage;
        if(health <= 0){
            Death();
        }
    }
    public virtual void Death(){
        
    }

    public void OnClick(GameObject caller)
    {
        if(Vector3.Distance(transform.position, caller.transform.position) <= interactionRange){
            Debug.Log($"Clicked on {buildingData.buildingName}");
        }
    }
}
