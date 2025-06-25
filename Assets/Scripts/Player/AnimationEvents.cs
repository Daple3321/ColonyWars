using System;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    public event Action PerformAttack;
    public event Action OnAttackEnded;
    public event Action OnStep;

    void Awake(){enabled = false;}

    public void Init()
    {
        enabled = true;
    }
    
    public void Event_PerformAttack(){
        PerformAttack?.Invoke();
    }
    public void Event_OnAttackEnded(){
        OnAttackEnded?.Invoke();
    }
    public void Event_OnStep(){
        OnStep?.Invoke();
    }
}
