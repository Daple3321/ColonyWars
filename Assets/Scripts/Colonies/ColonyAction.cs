using UnityEngine;

[System.Serializable]
public class ColonyAction
{
    public ColonyActionType actionType;
    
    [Tooltip("In-game minutes")]
    public int interval = 60; // minutes
}

public enum ColonyActionType
{
    Expand,
    Defense,
    Storage,
    Barracks,
    Resources,
    RaidSquad,
    Capture,
}