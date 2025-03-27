using System;
using UnityEngine;

public class PlayerAiming
{
    //public static event Action OnAimStart; // static?
    //public static event Action OnAimEnd;
    public static event Action<bool> OnAim;

    private static bool isAiming = false;

    public PlayerAiming()
    {
        isAiming = false;
        //OnAimStart = null;
        //OnAimEnd = null;
        OnAim = null;
        controls = GameAssets.controls;
    }

    private Controls controls;
    public void HandleAiming()
    {
        if (controls.Player.Aim.WasPressedThisFrame())
        {
            StartAiming();
        }
        if (controls.Player.Aim.WasReleasedThisFrame())
        {
            EndAiming();
        }
    }

    private void StartAiming()
    {
        isAiming = true;
        OnAim?.Invoke(true);
        //OnAimStart?.Invoke();
    }

    private void EndAiming()
    {
        isAiming = false;
        OnAim?.Invoke(false);
        //OnAimEnd?.Invoke();
    }
}
