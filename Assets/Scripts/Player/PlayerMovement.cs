using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public float fallSpeed;
    public float rotationSpeed;


    public InputAction moveAction;

    private CharacterController characterController;
    private Camera cm;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        
        cm = Camera.main;
        
        moveAction = InputSystem.actions.FindAction("Move");
        characterController = GetComponent<CharacterController>();
    }


    void Update()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y);

        moveDir = moveDir.normalized * moveSpeed * Time.deltaTime;

        if (!characterController.isGrounded)
        {
            moveDir.y = -fallSpeed * Time.deltaTime;
        }

        Debug.DrawRay(transform.position, moveDir * 10, Color.green);

        characterController.Move(moveDir);
    }
}
