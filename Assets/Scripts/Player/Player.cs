using UnityEngine;

public class Player : MonoBehaviour, IDamagable
{
    public PlayerCameraController cameraController;
    public PlayerCombat playerCombat;
    public PlayerMovement playerMovement;
    public PlayerFollow playerFollow;

    public float health;
    public float maxHealth;


    public void InitPlayer()
    {
        cameraController.Init(playerFollow);
        playerMovement.Init(cameraController);
        playerCombat.Init();
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


public interface IDamagable
{
    void TakeDamage(int damage);
    void Death();
}