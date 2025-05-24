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
    
    public Action PlayerBuildModeEnter;
    public Action<Building> OnPlayerBuild;
    
    public Action<float, float> OnPlayerPointsChanged;
    public Action<float, float> OnEnemyPointsChanged;
    
    public Action OnInteractivePanelOpened;
    public Action OnInteractivePanelClosed;
    // ----------------------------------------------------------

    // ------------------------ WORLD ---------------------------
    
    public Action OnSunrise = delegate {};
    public Action OnSunset = delegate {};
    public Action OnMinuteChange = delegate { };
    
    
    // ----------------------------------------------------------
}
