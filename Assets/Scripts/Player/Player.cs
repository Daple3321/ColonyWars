using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerCameraController cameraController;
    public PlayerCombat playerCombat;
    public PlayerMovement playerMovement;

    public PlayerFollow playerFollow;

    public void InitPlayer()
    {
        cameraController.Init(playerFollow);
        playerMovement.Init(cameraController);
        playerCombat.Init();
    }
}
