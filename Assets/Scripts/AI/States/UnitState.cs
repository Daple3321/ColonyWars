[System.Serializable]
public abstract class UnitState
{

    public StateMachine stateMachine;
    public UnitState nextState;
    public UnitState previousState;

    public Unit owner;
    
    public abstract void Enter();
    public abstract void Update();
    public virtual void FixedUpdate() { }
    public virtual void Back() { }
    public abstract void Exit();
}
