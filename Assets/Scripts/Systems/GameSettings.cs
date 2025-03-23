using UnityEngine;

[System.Serializable]
public class GameSettings
{
    public WorldGenSettings worldGenSettings;

    public GameDifficulty gameDifficulty;
}

public enum GameDifficulty
{
    VeryEasy,
    Easy,
    Normal,
    Hard,
    Impossible,
}