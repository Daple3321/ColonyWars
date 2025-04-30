using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerAiming
{
    //public static event Action OnAimStart; // static?
    //public static event Action OnAimEnd;
    public static event Action<bool> OnAim;

    //private static bool isAiming = false;
    public static GameObject worldMouseFollower;

    public PlayerAiming()
    {
        //isAiming = false;
        //OnAimStart = null;
        //OnAimEnd = null;
        OnAim = null;
        cm = Camera.main;
        controls = GameAssets.controls;
        worldMouseFollower = new GameObject("WorldMouseFollower");
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
    
    private static Camera cm;
    public static bool Raycast(LayerMask layerMask, out RaycastHit hit)
    {
        hit = new RaycastHit();
        if(!EventSystem.current.IsPointerOverGameObject())
        {
            Vector3 mousePos = Input.mousePosition;
            Ray mouseRay = cm.ScreenPointToRay(mousePos);
            RaycastHit mouseHit;
            if (Physics.Raycast(mouseRay, out mouseHit, Mathf.Infinity, layerMask))
            {
                
                hit = mouseHit;
                return true;
            }
        }
        
        return false;
    }

    private void StartAiming()
    {
        //isAiming = true;
        OnAim?.Invoke(true);
        //OnAimStart?.Invoke();
    }

    private void EndAiming()
    {
        //isAiming = false;
        OnAim?.Invoke(false);
        //OnAimEnd?.Invoke();
    }
}
