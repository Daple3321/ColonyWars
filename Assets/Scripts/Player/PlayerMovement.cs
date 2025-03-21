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
        // ----------- BASIC MOVEMENT ---------------------- //
        Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y);
        moveDir = moveDir.normalized * moveSpeed * Time.deltaTime;
        if (!characterController.isGrounded)
        {
            moveDir.y = -fallSpeed * Time.deltaTime;
        }
        //Debug.DrawRay(transform.position, moveDir * 5, Color.magenta);
        // ------------------------------------------------- //

        // Vector3 inputDir = new Vector3(moveInput.x, 0, moveInput.y);
        // Vector3 moveDirFixed = transform.rotation * inputDir;
        // moveDirFixed = moveDirFixed.normalized * moveSpeed * Time.deltaTime;
        // if (!characterController.isGrounded)
        // {
        //     moveDirFixed.y = -fallSpeed * Time.deltaTime;
        // }
        //Debug.DrawRay(transform.position, moveDirFixed * 5, Color.blue);


        // ---------- THIRD PERSON ROTATION ---------- //
        //Vector3 rotationVector = Quaternion.AngleAxis(90, Vector3.up) * (cm.transform.position - transform.position);
        //Vector3 crossProd = Vector3.Cross(transform.up, rotationVector);
        // if (Mathf.Abs(characterController.velocity.magnitude) > 0.5f) // блокировка вращения если velocity маленький
        // {

        // }
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(crossProd, Vector3.up), rotationSpeed * Time.deltaTime);
        // ------------------------------------------ //


        // float diffZ = cm.transform.position.y - transform.position.y;
        // Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, diffZ);
        // Vector3 screenPoint = cm.ScreenToWorldPoint(mousePos);
        // screenPoint.y = transform.position.y;
        // transform.LookAt(screenPoint);
        
        Vector3 mousePos = Input.mousePosition;
        Ray cameraRay = Camera.main.ScreenPointToRay(mousePos);
        // Определяем высоту персонажа (плоскость, на которой он стоит)
        float planeY = transform.position.y;
        // Вычисляем, где луч пересекает эту высоту
        float t = (planeY - cameraRay.origin.y) / cameraRay.direction.y;
        Vector3 worldMousePos = cameraRay.origin + t * cameraRay.direction;
        // Убираем возможные отклонения по высоте
        worldMousePos.y = transform.position.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(worldMousePos - transform.position, Vector3.up), rotationSpeed * Time.deltaTime);
        //transform.LookAt(worldMousePos);
        Debug.DrawLine(transform.position, worldMousePos, Color.yellow);

        //Vector3 dirToMouse = screenPoint - transform.position;
        //float angle = Mathf.Atan2(dirToMouse.z, dirToMouse.x) * Mathf.Rad2Deg;
        //Debug.DrawRay(transform.position, Quaternion.AngleAxis(-(angle-90), Vector3.up) * Vector3.forward , Color.yellow);
        //Debug.DrawRay(cm.transform.position, dirToMouse, Color.green);
        //transform.rotation = Quaternion.AngleAxis(-(angle - 90), Vector3.up);

        // Ray cameraRay = cm.ScreenPointToRay(Input.mousePosition);
        // Plane groundPlane = new Plane(transform.up, Vector3.zero);
        // float rayLength;
        // if (groundPlane.Raycast(cameraRay, out rayLength))
        // {
        //     Vector3 pointToLook = cameraRay.GetPoint(rayLength);
        //     Debug.DrawLine(cameraRay.origin, pointToLook, Color.yellow);

        //     transform.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z));
        // }

        characterController.Move(moveDir);
    }
}
