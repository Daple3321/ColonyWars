using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WaveContainer
{
    public int durationMinutes;
    public List<Wave> waves;
    public GameDifficulty difficulty;
}
