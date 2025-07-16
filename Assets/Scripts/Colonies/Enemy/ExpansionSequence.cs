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
    
    public bool shuffleDaySequence = false;
    public bool shuffleNightSequence = false;
    
    private TimeManager timeManager;
    private float timeMultiplier;
    
    private EnemyColony ownerColony;
    public void Init(TimeManager timeManager, EnemyColony ownerColony)
    {
        this.timeManager = timeManager;
        timeMultiplier = timeManager.timeSettings.timeMultiplier;
        this.ownerColony = ownerColony;
        
        if(shuffleDaySequence){
            ArrayExtensionMethods.ShuffleList(daySeq);
        }
        if(shuffleNightSequence){
            ArrayExtensionMethods.ShuffleList(nightSeq);
        }
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
        
        if(isDay){
            HandleDaySequence();
        }
        else{
            HandleNightSequence();
        }
    }
    
    private void HandleDaySequence(bool isSimulation = false)
    {
        if(timeToNextAction > 0){
            timeToNextAction -= 1;
        }
        else if(timeToNextAction <= 0 && dayActionIndex < daySeq.Count-1){ // ещё не достигли конца
            PerformAction(daySeq[dayActionIndex], isSimulation);
            dayActionIndex++;
            
            currentAction = daySeq[dayActionIndex];
            SetActionInterval(currentAction);
        }
        else if(timeToNextAction <= 0 && dayActionIndex < daySeq.Count){ // достигли конца сиквенса
            PerformAction(daySeq[dayActionIndex], isSimulation);
            dayActionIndex = 0; // loop
            
            currentAction = daySeq[dayActionIndex];
            SetActionInterval(currentAction);
        }
    }
    
    private void HandleNightSequence(bool isSimulation = false)
    {
        if(timeToNextAction > 0){
            timeToNextAction -= 1;
        }
        else if(timeToNextAction <= 0 && nightActionIndex < nightSeq.Count-1){
            PerformAction(nightSeq[nightActionIndex], isSimulation);
            nightActionIndex++;
            
            currentAction = nightSeq[nightActionIndex];
            SetActionInterval(currentAction);
        }
        else if(timeToNextAction <= 0 && nightActionIndex < nightSeq.Count){
            PerformAction(nightSeq[nightActionIndex], isSimulation);
            nightActionIndex = 0; // loop
            
            currentAction = nightSeq[nightActionIndex];
            SetActionInterval(currentAction);
        }
    }
    
    private void PerformAction(ColonyAction action, bool isSimulation = false)
    {
        ownerColony.PerformAction(action, isSimulation);
    }
    
    private void SetActionInterval(ColonyAction action)
    {
        timeToNextAction = action.interval - (action.interval*actionSpeedMultiplier);
    }
    
    public void SimulateDays(int days)
    {
        int minutesInDay = Mathf.CeilToInt((timeManager.timeSettings.sunsetHour - timeManager.timeSettings.sunriseHour)*60);
        Debug.Log($"Minutes in day: {minutesInDay}");
        int minutesInNight = Mathf.CeilToInt((24-(timeManager.timeSettings.sunsetHour - timeManager.timeSettings.sunriseHour))*60);
        Debug.Log($"Minutes in night: {minutesInNight}");
        
        int daysLeft = days;
        while(daysLeft > 0)
        {
            for(int i = minutesInDay; i > 0; i--)
            {
                HandleDaySequence(true);
                //Debug.Log("Simulated day action");
            }
            for(int i = minutesInNight; i > 0; i--)
            {
                HandleNightSequence(true);
                //Debug.Log("Simulated night action");
            }
            
            daysLeft--;
        }
    }
}
