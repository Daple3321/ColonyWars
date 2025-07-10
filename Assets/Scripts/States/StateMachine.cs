using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public UnitState currentState;
    public UnitState startState;
    
    public Unit owner;

    void Awake(){
        enabled = false;
    }
    
    public void StartStates(){ // technically REstart
        if (startState != null){
            ChangeState(startState);
        }
        else{
            Debug.LogError($"No starting state selected", owner.gameObject);
        }
    }

    public void Init(Unit owner)
    {
        this.owner = owner;
        
        // if(startState is PatrolState ps){
        //     ps.InitRoute(this);
        // }
        
        if (startState != null){
            ChangeState(startState);
        }
        enabled = true;
    }

    void Update()
    {
        if (currentState != null)
            currentState.UpdateState(this);
    }

    void FixedUpdate()
    {
        if (currentState != null)
            currentState.FixedUpdate();
    }

    public void ChangeState(UnitState newState)
    {
        //Debug.Log($"State changed to: {newState}");
        if (currentState != null)
            currentState.Exit();
        currentState = newState;
        if (currentState != null)
            currentState.Enter(this);
    }
}
