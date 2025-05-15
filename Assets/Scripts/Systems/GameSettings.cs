using UnityEngine;

[System.Serializable]
public class GameSettings
{
    public WorldGenSettings worldGenSettings;
    public ObjectGenSettings objectGenSettings;
    public WorldType worldType;
    
    public ColoniesSpawnSettings coloniesSpawnSettings;
    
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