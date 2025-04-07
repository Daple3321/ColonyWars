using System;

public class EventBus
{
    public EventBus() { }

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

    // ------------------------ PLAYER ---------------------------
    public Action PlayerDeath;
    public Action<float, float> PlayerHealthChanged;
    public Action<float> PlayerDamaged;

    public Action<float, float> PlayerStaminaChanged;
    // ----------------------------------------------------------
}
