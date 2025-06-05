using UnityEngine;

[System.Serializable]
public abstract class UnitState
{
    public StateMachine stateMachine;
    public UnitState nextState;
    public UnitState previousState;
    public void Init(UnitState nextState, UnitState previousState)
    {
        this.nextState = nextState;
        this.previousState = previousState;
    }

    public Unit owner;
    
    public abstract void Enter();
    public abstract void Update();
    public virtual void FixedUpdate() { }
    public virtual void Back() { stateMachine.ChangeState(previousState); }
    public abstract void Exit();
}
