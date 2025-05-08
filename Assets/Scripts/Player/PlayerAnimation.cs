using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
    void Awake(){enabled = false;}

    private Animator animator;
    public void Init(Animator animator)
    {
        this.animator = animator;
        
        enabled = true;
    }
    
    public void SetLayer(int layerIndex, float weight)
    {
        animator.SetLayerWeight(layerIndex, weight);
    }
    
    Coroutine animationRoutine;
    public void OnWeaponSetup(Weapon wp)
    {
        switch(wp.weaponType)
        {
            case WeaponType.Pistol:
                SetLayer(2, 0);
                
                if(animationRoutine != null){
                    StopCoroutine(animationRoutine);
                }
                animationRoutine = StartCoroutine(FadeInLayer(1, 0.15f));
            break;
            
            case WeaponType.Rifle:
                SetLayer(1, 0);
                
                if(animationRoutine != null){
                    StopCoroutine(animationRoutine);
                }
                animationRoutine = StartCoroutine(FadeInLayer(2, 0.15f));
            break;
            
            case WeaponType.Sword:
                //SetLayer(1, 0);
                
                if(animationRoutine != null){
                    StopCoroutine(animationRoutine);
                }
                animationRoutine = StartCoroutine(FadeOutLayer(1, 0.15f));
                animationRoutine = StartCoroutine(FadeOutLayer(2, 0.15f));
            break;
        }
    }
    
    public void OnWeaponClear()
    {
        if(animationRoutine != null){
            StopCoroutine(animationRoutine);
        }
        
        animationRoutine = StartCoroutine(FadeOutLayer(1, 0.15f));
        animationRoutine = StartCoroutine(FadeOutLayer(2, 0.15f));
        //SetLayer(1, 0);
        //SetLayer(2, 0);
    }
    
    public IEnumerator FadeOutLayer(int layerIndex, float duration)
    {
        float time = 0f;
        float startingWeight = 1f;
        float targetWeight = 0f;

        while (time < duration)
        {
            float layerWeight = Mathf.Lerp(startingWeight, targetWeight, time / duration);
            
            animator.SetLayerWeight(layerIndex, layerWeight);
            
            time += Time.deltaTime;
            yield return null;
        }
        // Finish the coroutine and make sure to set the exact target weight.
        animator.SetLayerWeight(layerIndex, targetWeight);
    }
    
    public IEnumerator FadeInLayer(int layerIndex, float duration)
    {
        float time = 0f;
        float startingWeight = 0f;
        float targetWeight = 1f;

        while (time < duration)
        {
            float layerWeight = Mathf.Lerp(startingWeight, targetWeight, time / duration);
            
            animator.SetLayerWeight(layerIndex, layerWeight);
            
            time += Time.deltaTime;
            yield return null;
        }
        // Finish the coroutine and make sure to set the exact target weight.
        animator.SetLayerWeight(layerIndex, targetWeight);
    }
}
