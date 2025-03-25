using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public CharacterState currentState;

    void Awake()
    {
        enabled = false;
    }

    public void Init(CharacterState startState)
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

    public void ChangeState(CharacterState newState)
    {
        if (currentState != null)
            currentState.Exit();
        currentState = newState;
        if (currentState != null)
            currentState.Enter();
    }
}
