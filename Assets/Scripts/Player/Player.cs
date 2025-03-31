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
    public StateMachine stateMachine;

    public float health;
    public float maxHealth;


    public Transform rightHand;
    public Transform leftHand;


    public void InitPlayer()
    {
        cameraController.Init(playerFollow);
        playerAiming = new PlayerAiming();
        playerMovement.Init(cameraController, playerAiming);
        playerInventory.Init(this);
        playerCombat.Init(this);
        stateMachine.Init(new WalkState());

        playerInventory.SelectItem(playerInventory.hotbar, 0);

        health = maxHealth;
        UpdateHealth();
    }

    void Update()
    {
        playerAiming.HandleAiming();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        UpdateHealth();
        if (health <= 0)
        {
            Death();
        }
    }

    public void UpdateHealth()
    {
        
    }
    public void Death()
    {
        Destroy(gameObject);
    }
}


public interface IDamageable
{
    void TakeDamage(int damage);
    void Death();
}