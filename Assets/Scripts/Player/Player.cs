using System;
using PrimeTween;
using TMPro;
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
    public PlayerBuilding playerBuilding;
    public PlayerAnimation playerAnimation;
    public PlayerUI playerUI;
    public BuildingPanelManager buildingPanelManager;
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
    [SerializeField] private Material mat;
    [SerializeField] private Animator animator;
    public void InitPlayer()
    {
        mat = GetComponentInChildren<Renderer>().material;
        CrosshairManager.i.Init();
        CrosshairManager.SwitchCrosshair(true);
        
        cameraController.Init(playerFollow);
        playerAnimation.Init(animator);
        playerAiming = new PlayerAiming();
        playerMovement.Init(cameraController, playerAiming);
        playerInventory.Init(this);
        playerCombat.Init(this, playerAnimation);
        playerBuilding.Init(playerInventory);
        squadManager.Init();
        playerUI.Init();
        
        buildingPanelManager = GameController.i.buildingPanelManager;
        buildingPanelManager.Init();
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
                    clickable.OnClick(this);
                }
            }
        }
    }
    
    public void TakeDamage(float damage, Vector3 knockback = new Vector3())
    {
        health -= damage;
        //onPlayerDamaged?.Invoke(health, maxHealth);
        playerMovement.AddForce(knockback);
        
        StartCoroutine(PlayerCameraController.CameraShake(9, 0.35f));
        Flash(Color.red, 0.1f);
        //TextMeshProUGUI popUp = PopUpManager.i.Spawn(transform.position+new Vector3(0, 2f, 0), Color.red);
        //popUp.text = damage.ToString("F1");
        
        EventBus.i.PlayerHealthChanged?.Invoke(health, maxHealth);
        EventBus.i.PlayerDamaged?.Invoke(damage);
        if (health <= 0)
        {
            Death();
        }
    }
    Sequence colorSeq;
    public void Flash(Color flashColor, float fadeTime = 0.2f)
    {
        mat.SetColor("_FlashColor", flashColor);
        
        colorSeq.Complete();
        // colorSeq = Sequence.Create()
        //     .Chain(Tween.MaterialProperty(mat, , Color.red, 0.2f))
        //     .Chain(Tween.MaterialColor(mat, Color.white, 0.2f));
        colorSeq = Sequence.Create()
            .Chain(Tween.Custom(0f, 1f, duration: fadeTime, onValueChange: newVal => mat.SetFloat("_FlashAmount", newVal)))
            .Chain(Tween.Custom(1f, 0f, duration: fadeTime, onValueChange: newVal => mat.SetFloat("_FlashAmount", newVal)));
    }

    public void Death()
    {
        gameObject.SetActive(false);
        EventBus.i.PlayerDeath?.Invoke();
        //Destroy(gameObject);
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
        if(controls.Player.SquadAssemble.WasPerformedThisFrame()){
            Command(CommandType.CREATE_SQUAD);
        }
        if(controls.Player.SquadRetreat.WasPerformedThisFrame()){
            Command(CommandType.FOLLOW, 5);
        }
        if(controls.Player.SquadClear.WasPerformedThisFrame()){
            Command(CommandType.CLEAR_SQUAD);
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
            case CommandType.CLEAR_SQUAD:
                squadManager.ClearSquad();
                break;
        }
        
        EventBus.i.PlayerCommandEnergyChanged?.Invoke(commandEnergy, maxCommandEnergy);
    }
}


public interface IDamageable
{
    void TakeDamage(float damage, Vector3 knockback = new Vector3());
    void Death();
}

public interface ICommander
{
    void Command(CommandType commandType, float commandPrice = 0f);
}

public interface IClickable
{
    void OnClick(Player caller);
}