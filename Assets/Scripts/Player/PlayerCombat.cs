using System;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Weapon currentWeapon;

    void Awake()
    {
        enabled = false;
    }

    private PlayerInventory playerInventory;
    public void Init(PlayerInventory playerInventory)
    {
        this.playerInventory = playerInventory;
        playerInventory.OnItemSelected += PlayerInventory_OnItemSelected;
        enabled = true;
    }

    private void PlayerInventory_OnItemSelected(object sender, PlayerInventory.OnItemSelectedEventArgs e)
    {
        if (e.selectedItem != null && e.selectedItem is Weapon wp)
        {
            currentWeapon = wp;
            SetupWeapon();
        }
        else
        {
            currentWeapon = null;
            ClearWeapon();
        }
    }

    void Update()
    {
        HandleWeapon();
    }

    private void SetupWeapon()
    {
        Debug.Log($"Selected weapon: {currentWeapon.itemName}.");
    }

    private void ClearWeapon()
    {
        
    }

    private void HandleWeapon()
    {

    }
    
    public void Attack()
    {

    }
}
