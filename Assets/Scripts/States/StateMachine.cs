using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public UnitState currentState;

    void Awake()
    {
        enabled = false;
    }

    public void Init(UnitState startState)
    {
        ChangeState(startState);
        enabled = true;
    }

    void Update()
    {
        if (currentState != null)
            currentState.Update();
    }

    void FixedUpdate()
    {
        if (currentState != null)
            currentState.FixedUpdate();
    }

    public void ChangeState(UnitState newState)
    {
        if (currentState != null)
            currentState.Exit();
        currentState = newState;
        if (currentState != null)
            currentState.Enter();
    }
}
