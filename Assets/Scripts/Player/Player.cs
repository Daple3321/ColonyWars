using System;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static EntityStatType;

public class Player : MonoBehaviour, IDamageable, ICommander
{
    public PlayerCameraController cameraController;
    public PlayerCombat playerCombat;
    public PlayerMovement movement;
    public PlayerFollow playerFollow;
    public PlayerAiming playerAiming;
    public PlayerInventory playerInventory;
    public PlayerItemUsing playerUsing;
    public PlayerBuilding playerBuilding;
    public PlayerAnimation playerAnimation;
    public PlayerUI ui;
    public AnimationEvents animationEvents;
    public BuildingPanelManager buildingPanelManager;
    public SquadManager squadManager;
    
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
    
    public int maxUnits;
    public Squad squad;
    
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
        
        squad = new Squad(maxUnits, gameObject);
        cameraController.Init(playerFollow);
        playerAnimation.Init(animator);
        animationEvents.Init();
        playerAiming = new PlayerAiming();
        movement.Init(this, cameraController, playerAiming);
        playerInventory.Init(this);
        playerUsing.Init(this);
        playerCombat.Init(this, playerAnimation);
        playerBuilding.Init(playerInventory);
        squadManager.Init(this);
        ui.Init();
        
        buildingPanelManager = GameController.i.buildingPanelManager;
        buildingPanelManager.Init();
        //stateMachine.Init(new WalkState());

        playerInventory.SelectItem(playerInventory.hotbar, 0);

        MouseTooltip.i.Init();
        
        controls = GameAssets.controls;
        controls.Squad.Enable();
        mouse = Mouse.current;
        mainCam = Camera.main;

        health = stats[maxHealth].Value;
        commandEnergy = stats[maxCommandEnergy].Value;
        EventBus.i.PlayerHealthChanged?.Invoke(health, stats[maxHealth].Value);
        EventBus.i.PlayerCommandEnergyChanged?.Invoke(commandEnergy, stats[maxCommandEnergy].Value);

        //TimeService.OnHourChange += x => Debug.Log($"Hour changed to: {x}. From player.");
        EventBus.i.OnSunrise += () => NotificationManager.i.Add("Sunrise!", "It's more safe now", 10, Color.blue, Color.white);
        EventBus.i.OnSunset += () => NotificationManager.i.Add("Night!", "Be careful, enemies might raid you.", 10, GameAssets.colors.blockedColor, Color.white);
    }

    void Update()
    {
        playerAiming.HandleAiming();

        HandleCommands();
        
        // mouse.leftButton.wasPressedThisFrame && !EventSystem.current.IsPointerOverGameObject()
        if (controls.Player.Interact.WasPressedThisFrame())
        {
            Vector3 mousePosition = mouse.position.ReadValue();
            Ray ray = mainCam.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, mouseClickLayers))
            {
                if(hit.collider.TryGetComponent(out IClickable clickable)){
                    clickable.OnClick(this);
                }
                
                // if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Resources") // HAND GATHERING
                // && Vector3.Distance(transform.position, hit.collider.transform.position) < 4f
                // && !playerCombat.toolEquiped){
                //     ResourceManager.i.PlayerHarvest(hit.collider.gameObject, 1);
                // }
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
        
        if(Input.GetKeyDown(KeyCode.Keypad5)){
            AddHealth(45f);
        }
    }
    
    public UniTask<DamageResult> TakeDamage<T>(float damage, T source, Vector3 knockback = new Vector3(), DamageType damageType = DamageType.Melee)
    {
        DamageResult result = DamageResult.Dealt;
        if(playerCombat.isBlocking && movement.HasStamina(5f)){
            result = DamageResult.Blocked;
            movement.AddStamina(-5f);
            Flash(Color.blue, 0.3f);
        }
        else
        {
            health -= damage;
            movement.AddForce(knockback);
            
            StartCoroutine(PlayerCameraController.CameraShake(9, 0.35f));
            Flash(Color.red, 0.1f);
            
            EventBus.i.PlayerHealthChanged?.Invoke(health, stats[maxHealth].Value);
            EventBus.i.PlayerDamaged?.Invoke(damage);
            if (health <= 0){
                Death();
            }
        }
        
        return UniTask.FromResult(result);
    }
    
    public void AddHealth(float amount)
    {
        health += amount;
        health = Math.Clamp(health, 0, stats[maxHealth].Value);
        EventBus.i.PlayerHealthChanged?.Invoke(health, stats[maxHealth].Value);
        
        Flash(Color.green, 0.2f);
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
        if(controls.Squad.CommandMenu.WasPerformedThisFrame()){
            squadManager.commandMenu.Show();
            // if (controls.Squad.CommandMenu.IsPressed() && !squadManager.commandMenu.isOpen){
            // }
        }
        if(controls.Squad.CommandMenu.WasReleasedThisFrame() && squadManager.commandMenu.isOpen){
            squadManager.commandMenu.Hide();
        }
        
        if (controls.Squad.SquadMoveOrder.WasPerformedThisFrame()){
            Command(CommandType.HOMEPOS, 5);
        }
        if (controls.Squad.SquadAssembleMenu.WasPerformedThisFrame()){
            squadManager.SwitchAssemblePanel();
            //Command(CommandType.CREATE_SQUAD);
            //TakeDamage(2, 100);
        }
        if(controls.Squad.SquadAssemble.WasPerformedThisFrame()){
            Command(CommandType.CREATE_SQUAD);
        }
        if(controls.Squad.SquadRetreat.WasPerformedThisFrame()){
            Command(CommandType.FOLLOW, 5);
        }
        if(controls.Squad.SquadClear.WasPerformedThisFrame()){
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
        return squad.units.Count > 0;
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
    public UniTask<DamageResult> TakeDamage<T>(float damage, T source, Vector3 knockback = new Vector3(), DamageType damageType = DamageType.Melee);
    public (float health, float maxHealth) GetHealth();
    void Death();
}
public enum DamageResult : byte
{
    Dealt,
    Blocked,
    Missed,
    Killed,
    
}
public enum DamageType : byte
{
    Melee,
    Ranged,
    Explosive,
    Destructive, // for buildings
    // stab, slash, pierce ...
}

public interface ICommander
{
    void Command(CommandType commandType, float commandPrice = 0f);
}

public interface IClickable
{
    void OnClick(Player caller);
}