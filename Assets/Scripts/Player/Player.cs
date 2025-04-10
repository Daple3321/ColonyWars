using Unity.Burst;
using UnityEngine;

[RequireComponent(typeof(StateMachine))]
public class Player : MonoBehaviour, IDamageable
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

        if (Input.GetKeyDown(KeyCode.F))
        {
            Vector3 mousePos = Input.mousePosition;
            Ray mouseRay = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit mouseHit;
            if (Physics.Raycast(mouseRay, out mouseHit, 50, groundLayer))
            {
                squadManager.SquadOrder(mouseHit.point);
            }
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            squadManager.TryAssembleSquad();

            TakeDamage(2, 100);
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
}


public interface IDamageable
{
    void TakeDamage(float damage, float knockback = 0f);
    void Death();
}