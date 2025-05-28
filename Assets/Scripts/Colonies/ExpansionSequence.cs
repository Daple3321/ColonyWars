using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExpansionSequence", menuName = "Scriptable Objects/Expansion sequence")]
public class ExpansionSequence : ScriptableObject
{
    public List<ColonyAction> daySeq;
    public int dayActionIndex = 0;
    public List<ColonyAction> nightSeq;
    public int nightActionIndex = 0;
    
    public ColonyAction currentAction;
    public float timeToNextAction = 0f;
    
    public float actionSpeedMultiplier = 0f;
    
    private TimeManager timeManager;
    private float timeMultiplier;
    
    private EnemyColony ownerColony;
    public void Init(TimeManager timeManager, EnemyColony ownerColony)
    {
        this.timeManager = timeManager;
        timeMultiplier = timeManager.timeSettings.timeMultiplier;
        this.ownerColony = ownerColony;
    }
    
    public void UpdateSequence(bool isDay) // tick action timers
    {
        if(isDay && currentAction == null){ // setup
            currentAction = daySeq[dayActionIndex];
            SetActionInterval(currentAction);
        }
        else if(!isDay && currentAction == null){
            currentAction = nightSeq[nightActionIndex];
            SetActionInterval(currentAction);
        }
        
        if(isDay)
        {
            if(timeToNextAction > 0){
                timeToNextAction -= 1;
            }
            else if(timeToNextAction <= 0 && dayActionIndex < daySeq.Count-1){ // ещё не достигли конца
                PerformAction(daySeq[dayActionIndex]);
                dayActionIndex++;
                
                currentAction = daySeq[dayActionIndex];
                SetActionInterval(currentAction);
            }
            else if(timeToNextAction <= 0 && dayActionIndex < daySeq.Count){ // достигли конца сиквенса
                PerformAction(daySeq[dayActionIndex]);
                dayActionIndex = 0; // loop
                
                currentAction = daySeq[dayActionIndex];
                SetActionInterval(currentAction);
            }
            
        }
        else
        {
            if(timeToNextAction > 0){
                timeToNextAction -= 1;
            }
            else if(timeToNextAction <= 0 && nightActionIndex < nightSeq.Count-1){
                PerformAction(nightSeq[nightActionIndex]);
                nightActionIndex++;
                
                currentAction = nightSeq[nightActionIndex];
                SetActionInterval(currentAction);
            }
            else if(timeToNextAction <= 0 && nightActionIndex < nightSeq.Count){
                PerformAction(nightSeq[nightActionIndex]);
                nightActionIndex = 0; // loop
                
                currentAction = nightSeq[nightActionIndex];
                SetActionInterval(currentAction);
            }
        }
    }
    
    private void PerformAction(ColonyAction action)
    {
        ownerColony.PerformAction(action);
    }
    
    private void SetActionInterval(ColonyAction action)
    {
        timeToNextAction = action.interval - (action.interval*actionSpeedMultiplier);
    }
}
