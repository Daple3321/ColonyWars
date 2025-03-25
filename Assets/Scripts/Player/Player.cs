using UnityEngine;

[RequireComponent(typeof(StateMachine))]
public class Player : MonoBehaviour, IDamageable
{
    public PlayerCameraController cameraController;
    public PlayerCombat playerCombat;
    public PlayerMovement playerMovement;
    public PlayerFollow playerFollow;
    public PlayerInventory playerInventory;
    public StateMachine stateMachine;

    public float health;
    public float maxHealth;


    public Transform rightHand;
    public Transform leftHand;


    public void InitPlayer()
    {
        cameraController.Init(playerFollow);
        playerMovement.Init(cameraController);
        playerInventory.Init(this);
        playerCombat.Init(this);
        stateMachine.Init(new WalkState());

        playerInventory.SelectItem(0);

        health = maxHealth;
        UpdateHealth();
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