using System;

public class EventBus
{
    public EventBus() { 
        _instance = this;
    }

    private static EventBus _instance;

    public static EventBus i
    {
        get
        {
            if (_instance == null)
                _instance = new EventBus();

            return _instance;
        }
    }
    
    
    // ----------------------- GLOBAL ----------------------------
    public Action<Colony> OnColonyCreated;
    public Action<Colony> OnColonyDestroyed;
    // -----------------------------------------------------------
    
    
    // ------------------------ PLAYER ---------------------------
    public Action PlayerDeath;
    public Action PlayerRespawn;
    public Action<float, float> PlayerHealthChanged;
    public Action<float> PlayerDamaged;
    public Action<float, float> PlayerCommandEnergyChanged;

    public Action<float, float> PlayerStaminaChanged;
    
    public Action<float> PlayerConcentrationChanged;
    
    public Action<float> PlayerAttackedWithAmmo; // arg: ammo left
    
    public Action PlayerBuildModeEnter;
    public Action<Building> OnPlayerBuild;
    
    // ----------- POINTS -------------------------
    public Action<float, float> OnPlayerPointsChanged;
    public Action<float, float> OnEnemyPointsChanged;
    public Action<float> OnPlayerYieldChanged;
    public Action<float> OnEnemyYieldChanged;
    // -----------------------------------------------
    public Action OnInteractionStop;
    public Action OnInteractivePanelOpened;
    public Action OnInteractivePanelClosed;
    
    /// <summary>
    /// When any panel is OPENED with inventory on it (For automatic inventory opening)
    /// </summary>
    public Action OnInventoryPanelOpened;
    /// <summary>
    /// When any panel is CLOSED with inventory on it (For automatic inventory closing)
    /// </summary>
    public Action OnInventoryPanelClosed;
    // ----------------------------------------------------------

    // ------------------------ WORLD ---------------------------
    
    public Action OnSunrise = delegate {};
    public Action OnSunset = delegate {};
    public Action OnMinuteChange = delegate { };
    
    
    // ----------------------------------------------------------
}
