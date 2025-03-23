using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerCameraController cameraController;
    public PlayerCombat playerCombat;
    public PlayerMovement playerMovement;

    public void InitPlayer()
    {
        cameraController.Init();
        playerMovement.Init(cameraController);
        playerCombat.Init();
    }
}
