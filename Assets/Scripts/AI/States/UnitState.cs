using UnityEngine;

public abstract class UnitState : ScriptableObject
{
    //public StateMachine stateMachine;
    //public UnitState nextState;
    //public UnitState previousState;
    // public void Init(UnitState nextState, UnitState previousState)
    // {
    //     this.nextState = nextState;
    //     this.previousState = previousState;
    // }

    //public Unit owner;
    
    public abstract void Enter(StateMachine stateMachine);
    public abstract void Update(StateMachine stateMachine);
    public virtual void FixedUpdate(){}
    //public virtual void Back(){}
    public abstract void Exit();
}
