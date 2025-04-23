using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(StateMachine))]
public class Player : MonoBehaviour, IDamageable, ICommander
{
    public PlayerCameraController cameraController;
    public PlayerCombat playerCombat;
    public PlayerMovement playerMovement;
    public PlayerFollow playerFollow;
    public PlayerAiming playerAiming;
    public PlayerInventory playerInventory;
    public PlayerUI playerUI;
    public SquadManager squadManager;
    public StateMachine stateMachine;

    public float health;
    public float maxHealth;


    public Transform rightHand;
    public Transform leftHand;

    public LayerMask groundLayer;
    public LayerMask mouseClickLayers;
    
    public float commandEnergy;
    public float maxCommandEnergy;
    public float commandEnergyRegenSpeed;
    
    private Controls controls;
    private Mouse mouse;
    private Camera mainCam;
    public void InitPlayer()
    {
        cameraController.Init(playerFollow);
        playerAiming = new PlayerAiming();
        playerMovement.Init(cameraController, playerAiming);
        playerInventory.Init(this);
        playerCombat.Init(this);
        squadManager.Init();
        playerUI.Init();
        //stateMachine.Init(new WalkState());

        playerInventory.SelectItem(playerInventory.hotbar, 0);

        MouseTooltip.i.Init();
        
        controls = GameAssets.controls;
        mouse = Mouse.current;
        mainCam = Camera.main;

        health = maxHealth;
        commandEnergy = maxCommandEnergy;
        EventBus.i.PlayerHealthChanged?.Invoke(health, maxHealth);
        EventBus.i.PlayerCommandEnergyChanged?.Invoke(commandEnergy, maxCommandEnergy);

        //TimeService.OnHourChange += x => Debug.Log($"Hour changed to: {x}. From player.");
        EventBus.i.OnSunrise += () => Debug.Log($"Sunrise!");
        EventBus.i.OnSunset += () => Debug.Log($"Sunset.");
    }

    void Update()
    {
        playerAiming.HandleAiming();

        HandleCommands();
        
        if (mouse.leftButton.wasPressedThisFrame && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector3 mousePosition = mouse.position.ReadValue();
            Ray ray = mainCam.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, mouseClickLayers))
            {
                if(hit.collider.TryGetComponent(out IClickable clickable)){
                    clickable.OnClick(gameObject);
                }
            }
        }
    }
    
    public void TakeDamage(float damage, float knockback = 0f)
    {
        health -= damage;
        //onPlayerDamaged?.Invoke(health, maxHealth);
        EventBus.i.PlayerHealthChanged?.Invoke(health, maxHealth);
        EventBus.i.PlayerDamaged?.Invoke(damage);
        if (health <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        EventBus.i.PlayerDeath?.Invoke();
        Destroy(gameObject);
    }
    
    public void HandleCommands()
    {
        if (controls.Player.SquadMoveOrder.WasPressedThisFrame()){
            Command(CommandType.HOMEPOS, 5);
        }
        if (controls.Player.SquadAssembleMenu.WasPressedThisFrame()){
            squadManager.SwitchAssemblePanel();
            //Command(CommandType.CREATE_SQUAD);
            //TakeDamage(2, 100);
        }
        if(controls.Player.SquadAssemble.WasPerformedThisFrame())
        {
            Command(CommandType.CREATE_SQUAD);
        }
        if(Input.GetKeyDown(KeyCode.B)){
            Command(CommandType.FOLLOW, 5); // retreat
        }
        
        HandleCommandEnergy();
    }
    public void HandleCommandEnergy()
    {
        if(commandEnergy < maxCommandEnergy){
            commandEnergy += commandEnergyRegenSpeed * Time.deltaTime;
            
            EventBus.i.PlayerCommandEnergyChanged?.Invoke(commandEnergy, maxCommandEnergy);
        }
        
    }
    public bool HasSquad(){
        return squadManager.HasSquad();
    }
    public bool CanCommand(float commandPrice){
        return commandEnergy >= commandPrice;
    }
    public void Command(CommandType commandType, float commandPrice = 0f)
    {
        if(commandPrice > 0 && !CanCommand(commandPrice)){
            Debug.Log("Not enough command energy");
            return;
        }
        
        switch(commandType){
            case CommandType.FOLLOW:
                if(!HasSquad())
                    return;
                squadManager.FollowOrder();
                commandEnergy -= commandPrice;
                break;
            case CommandType.HOMEPOS:
                if(!HasSquad())
                    return;
            
                Vector3 mousePos = Input.mousePosition;
                Ray mouseRay = Camera.main.ScreenPointToRay(mousePos);
                RaycastHit mouseHit;
                if (Physics.Raycast(mouseRay, out mouseHit, 50, groundLayer))
                {
                    squadManager.HomePosOrder(mouseHit.point);
                }
                
                commandEnergy -= commandPrice;
                break;
            case CommandType.CREATE_SQUAD:
                squadManager.TryAssembleSquad();
                break;
        }
        
        EventBus.i.PlayerCommandEnergyChanged?.Invoke(commandEnergy, maxCommandEnergy);
    }
}


public interface IDamageable
{
    void TakeDamage(float damage, float knockback = 0f);
    void Death();
}

public interface ICommander
{
    void Command(CommandType commandType, float commandPrice = 0f);
}

public interface IClickable
{
    void OnClick(GameObject caller);
}