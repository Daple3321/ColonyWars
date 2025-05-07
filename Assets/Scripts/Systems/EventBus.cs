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
    public Action<float, float> PlayerHealthChanged;
    public Action<float> PlayerDamaged;
    public Action<float, float> PlayerCommandEnergyChanged;

    public Action<float, float> PlayerStaminaChanged;
    
    public Action<float> PlayerConcentrationChanged;
    
    public Action PlayerBuildModeEnter;
    // ----------------------------------------------------------

    // ------------------------ WORLD ---------------------------
    
    public Action OnSunrise = delegate {};
    public Action OnSunset = delegate {};
    public Action<int> OnHourChange = delegate { };
    
    
    // ----------------------------------------------------------
}
