using UnityEngine;

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
    
    public float commandEnergy;
    public float maxCommandEnergy;

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

        health = maxHealth;
        EventBus.i.PlayerHealthChanged?.Invoke(health, maxHealth);

        //TimeService.OnHourChange += x => Debug.Log($"Hour changed to: {x}. From player.");
        EventBus.i.OnSunrise += () => Debug.Log($"Sunrise!");
        EventBus.i.OnSunset += () => Debug.Log($"Sunset.");
    }

    void Update()
    {
        playerAiming.HandleAiming();

        if (Input.GetKeyDown(KeyCode.F)){
            Command(CommandType.HOMEPOS);
        }
        if (Input.GetKeyDown(KeyCode.V)){
            Command(CommandType.CREATE_SQUAD);
            TakeDamage(2, 100);
        }
        if(Input.GetKeyDown(KeyCode.B)){
            Command(CommandType.FOLLOW); // retreat
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

    public void Command(CommandType commandType)
    {
        switch(commandType){
            case CommandType.FOLLOW:
                squadManager.FollowOrder();
                break;
            case CommandType.HOMEPOS:
                Vector3 mousePos = Input.mousePosition;
                Ray mouseRay = Camera.main.ScreenPointToRay(mousePos);
                RaycastHit mouseHit;
                if (Physics.Raycast(mouseRay, out mouseHit, 50, groundLayer))
                {
                    squadManager.HomePosOrder(mouseHit.point);
                }
                break;
            case CommandType.CREATE_SQUAD:
                squadManager.TryAssembleSquad();
                break;
        }
    }
}


public interface IDamageable
{
    void TakeDamage(float damage, float knockback = 0f);
    void Death();
}

public interface ICommander
{
    void Command(CommandType commandType);
}