using System;
using AYellowpaper.SerializedCollections;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static EntityStatType;

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
    
    [SerializedDictionary("Stat Type", "Stat")]
    public SerializedDictionary<EntityStatType, Stat> stats;
    
    public float health;
    //public float maxHealth;


    public Transform rightHand;
    public Transform leftHand;
    public Transform shootPoint;

    public LayerMask groundLayer;
    public LayerMask mouseClickLayers;
    
    public float commandEnergy;
    //public float maxCommandEnergy;
    //public float commandEnergyRegenSpeed;
    
    public bool isInteracting = false;
    
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
        playerMovement.Init(this, cameraController, playerAiming);
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

        health = stats[maxHealth].Value;
        commandEnergy = stats[maxCommandEnergy].Value;
        EventBus.i.PlayerHealthChanged?.Invoke(health, stats[maxHealth].Value);
        EventBus.i.PlayerCommandEnergyChanged?.Invoke(commandEnergy, stats[maxCommandEnergy].Value);

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
                
                if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Resources")
                && Vector3.Distance(transform.position, hit.collider.transform.position) < 4f){
                    ResourceManager.i.PlayerHarvest(hit.collider.gameObject, 1);
                }
            }
        }
        
        // проверка на уже активный interaction должна быть и сброс его перед новым
        if(controls.Player.Interact.WasPressedThisFrame() && !isInteracting) // вообще по другому должно всё работать
        {
            Vector3 mousePosition = mouse.position.ReadValue();
            Ray ray = mainCam.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, mouseClickLayers))
            {
                if(hit.collider.TryGetComponent(out IInteractable interactable)){
                    isInteracting = interactable.Interact();
                }
            }
        }
        else if(controls.Player.Interact.WasPressedThisFrame() && isInteracting)
        {
            isInteracting = false;
            EventBus.i.OnInteractionStop?.Invoke();
        }
    }
    
    public void TakeDamage(float damage, GameObject source = null, Vector3 knockback = new Vector3())
    {
        health -= damage;
        //onPlayerDamaged?.Invoke(health, maxHealth);
        playerMovement.AddForce(knockback);
        
        StartCoroutine(PlayerCameraController.CameraShake(9, 0.35f));
        Flash(Color.red, 0.1f);
        //TextMeshProUGUI popUp = PopUpManager.i.Spawn(transform.position+new Vector3(0, 2f, 0), Color.red);
        //popUp.text = damage.ToString("F1");
        
        EventBus.i.PlayerHealthChanged?.Invoke(health, stats[maxHealth].Value);
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
        if(commandEnergy < stats[maxCommandEnergy].Value){
            commandEnergy += stats[commandEnergyRegenSpeed].Value * Time.deltaTime;
            
            EventBus.i.PlayerCommandEnergyChanged?.Invoke(commandEnergy, stats[maxCommandEnergy].Value);
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
        
        EventBus.i.PlayerCommandEnergyChanged?.Invoke(commandEnergy, stats[maxCommandEnergy].Value);
    }

    public (float health, float maxHealth) GetHealth()
    {
        return (health, stats[maxHealth].Value);
    }
}


public interface IDamageable
{
    void TakeDamage(float damage, GameObject source = null, Vector3 knockback = new Vector3());
    (float health, float maxHealth) GetHealth();
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