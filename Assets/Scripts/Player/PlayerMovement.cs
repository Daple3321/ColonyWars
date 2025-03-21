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
        Cursor.visible = true;
        
        cm = Camera.main;
        
        moveAction = InputSystem.actions.FindAction("Move");
        characterController = GetComponent<CharacterController>();
    }


    void Update()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        //Vector3 moveDir = new Vector3(moveInput.x, 0, 0);
        //moveDir = moveDir.normalized * moveSpeed * Time.deltaTime;
        // if (!characterController.isGrounded)
        // {
        //     moveDir.y = -fallSpeed * Time.deltaTime;
        // }
        //Debug.DrawRay(transform.position, moveDir * 5, Color.magenta);
        
        Vector3 inputDir = new Vector3(moveInput.x, 0, moveInput.y);
        Vector3 moveDirFixed = transform.rotation * inputDir;
        moveDirFixed = moveDirFixed.normalized * moveSpeed * Time.deltaTime;
        if (!characterController.isGrounded)
        {
            moveDirFixed.y = -fallSpeed * Time.deltaTime;
        }
        //Debug.DrawRay(transform.position, moveDirFixed * 5, Color.blue);


        //Vector3 projectedVec = Vector3.Project(rotationVector, transform.forward);
        //float angleToRot = Vector3.Angle(transform.forward, crossProd);
        //Debug.Log(angleToRot);
        Vector3 rotationVector = Quaternion.AngleAxis(90, Vector3.up) * (cm.transform.position - transform.position);
        Vector3 crossProd = Vector3.Cross(transform.up, rotationVector);
        // if (Mathf.Abs(characterController.velocity.magnitude) > 0.5f) // блокировка вращения если velocity маленький
        // {
            
        // }
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(crossProd, Vector3.up), rotationSpeed * Time.deltaTime);

        Ray screenPoint = cm.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(screenPoint.origin, screenPoint.direction, Color.magenta);

        //Debug.DrawRay(transform.position, crossProd * 4, Color.green);
        //Debug.DrawRay(cm.transform.position, cm.transform.forward * 4, Color.red);

        characterController.Move(moveDirFixed);
    }
}
