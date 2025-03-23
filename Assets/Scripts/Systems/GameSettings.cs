using UnityEngine;

[System.Serializable]
public class GameSettings
{
    public WorldGenSettings worldGenSettings;
    public WorldType worldType;
    
    public GameDifficulty gameDifficulty;
}

public enum WorldType
{
    Default,
    Mountains,
    Plains,
}

public enum GameDifficulty
{
    VeryEasy,
    Easy,
    Normal,
    Hard,
    Impossible,
}