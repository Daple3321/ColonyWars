using System;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Weapon currentWeapon;
    public WeaponWorldItem weaponWorld;

    void Awake()
    {
        enabled = false;
    }

    private Player player;
    private PlayerInventory playerInventory;
    private Controls controls;
    public void Init(Player player)
    {
        this.player = player;
        this.playerInventory = player.playerInventory;
        controls = GameAssets.controls;
        playerInventory.OnItemSelected += PlayerInventory_OnItemSelected;
        playerInventory.OnItemDropped += PlayerInventory_OnItemDropped;
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
            ClearWeapon();
        }
    }
    private void PlayerInventory_OnItemDropped(object sender, PlayerInventory.OnItemDroppedEventArgs e)
    {
        if (e.droppedItem == currentWeapon)
        {
            ClearWeapon();
        }
    }

    void Update()
    {
        if (currentWeapon != null)
        {
            HandleWeapon();
        }
    }

    private void SetupWeapon()
    {
        Debug.Log($"Selected weapon: {currentWeapon.itemName}.");
    }

    private void ClearWeapon()
    {
        currentWeapon = null;
        weaponWorld = null;
    }

    private void HandleWeapon()
    {
        if (controls.Player.Attack.IsPressed())
        {
            currentWeapon.Attack();
            weaponWorld.AttackEffects();
        }
    }
    
    public void Attack()
    {
        
    }
}
