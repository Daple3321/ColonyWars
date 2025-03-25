public abstract class CharacterState
{
    protected StateMachine stateMachine;
    protected CharacterState nextState;
    protected CharacterState previousState;
    
    public abstract void Enter();
    public abstract void Update();
    public virtual void FixedUpdate() { }
    public virtual void Back(){}
    public abstract void Exit();
}
