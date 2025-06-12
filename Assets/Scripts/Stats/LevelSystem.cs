using System;
using UnityEngine;

[System.Serializable]
public class LevelSystem
{
    private int level;
    private int skillPoints;
    private int experience;

    public event EventHandler OnExperienceChanged;
    public event EventHandler OnLevelChanged;
    public event EventHandler OnSkillPointsChanged;
    
    public LevelSystem()
    {
        // loading here 
        // level = SaveManager.i.state.level;
        // skillPoints = SaveManager.i.state.skillPoints;
        // experience = SaveManager.i.state.experience;
        level = 1;
        skillPoints = 0;
        experience = 0;
    }
    
    public void AddExperience(int amount)
    {
        experience += amount;
        //Journal.AddMessage($"+ {amount} xp".AddColor(Colors.i.legendaryColor));
        while(experience >= GetExperienceToNextLevel(level))
        {
            experience -= GetExperienceToNextLevel(level);
            level++;
            skillPoints++;
            
            //SaveManager.i.state.level = level;
            //SaveManager.i.state.skillPoints = skillPoints;
            
            OnLevelChanged?.Invoke(this, EventArgs.Empty);
        }
        //SaveManager.i.state.experience = experience;
        OnExperienceChanged?.Invoke(this, EventArgs.Empty);
    }
    
    public void ChangeLevel(int lvl){
        level = lvl;
        experience = 0;
    }
    
    public int GetLevel(){
        return level;
    }
    public int GetExperience(){
        return experience;
    }
    public float GetExperienceNormalized(){
        return (float)experience / GetExperienceToNextLevel(level);
    }
    public int GetSkillPoints(){
        return skillPoints;
    }
    public void AddSkillPoints(int amount){
        skillPoints += amount;
        //SaveManager.i.state.skillPoints = skillPoints;
        OnSkillPointsChanged?.Invoke(this, EventArgs.Empty);
    }
    public void SubtractSkillPoint(){
        if(skillPoints > 0){
            skillPoints--;
            OnSkillPointsChanged?.Invoke(this, EventArgs.Empty); ;
            //SaveManager.i.state.skillPoints = skillPoints;
        }
        else{
            Debug.LogWarning("Can't subtract skill point. Already zero.");
        }
    }
    public bool HasPoints(){
        return skillPoints > 0;
    }
    
    public int GetExperienceToNextLevel(int level)
    {
        return Mathf.RoundToInt(9*Mathf.Pow(level, 2)/10 - 9*level/10);
    }
}
