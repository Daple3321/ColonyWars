using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementTest : MonoBehaviour
{
    public CinemachineOrbitalFollow cmFollow;
    public CinemachineCamera cinemachineCamera;
    public CinemachinePositionComposer positionComposer;
    public Camera mainCamera;
    public CharacterController characterController;
    
    public float currentSpeed = 5f;
    public float fallSpeed = 10f;
    public float rotationSpeed = 10f;
    
    [Space(10)]
    [Header("Actions")]
    public InputAction moveAction;
    public InputAction runAction;
    private Controls controls;
    public Texture2D customCursor;
    void Start()
    {
        //GameAssets.Init();
        controls = new Controls();
        controls.Player.Enable();
        //controls = GameAssets.controls;
        
        mainCamera = Camera.main;
        moveAction = controls.Player.Move;
        runAction = controls.Player.Sprint;
        characterController = GetComponent<CharacterController>();
        
        //CrosshairManager.i.Init();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        //Cursor.SetCursor(customCursor, new Vector2(36, 36), CursorMode.Auto);
    }

    void Update()
    {
        HandleMovementAndRotation();
        HandleRotationToVelocity();
    }

    private void HandleMovementAndRotation()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        
        Vector3 moveDir = new Vector3(-moveInput.x, 0, moveInput.y);

        Vector3 forwardVec = mainCamera.transform.rotation * Vector3.forward;
        forwardVec = Quaternion.AngleAxis(-90, Vector3.up) * forwardVec;
        Vector3 crossProd = Vector3.Cross(transform.up, forwardVec);
        Debug.DrawRay(transform.position, forwardVec*5, Color.green);

        Quaternion moveRot = Quaternion.FromToRotation(moveDir, crossProd);
        if (moveInput.sqrMagnitude > 0){
            moveDir = moveRot * Vector3.forward * currentSpeed * Time.deltaTime;
        }
        else{
            moveDir = Vector3.zero;
        }

        if (!characterController.isGrounded){
            moveDir.y = -fallSpeed * Time.deltaTime;
        }
                
        characterController.Move(moveDir);
        
        Debug.DrawRay(transform.position, moveInput * 10, Color.magenta);
        Debug.DrawRay(transform.position, crossProd * 2, Color.red);
        Debug.DrawRay(transform.position, moveDir * 5, Color.cyan);
    }
    
    Vector2 _moveInput;
    private void HandleRotationToVelocity()
    {
        _moveInput = moveAction.ReadValue<Vector2>();
        Vector3 moveDir = new Vector3(-_moveInput.x, 0, _moveInput.y);
        
        Vector3 forwardVec = mainCamera.transform.rotation * Vector3.forward;
        forwardVec = Quaternion.AngleAxis(-90, Vector3.up) * forwardVec;
        Vector3 crossProd = Vector3.Cross(transform.up, forwardVec);
        Debug.DrawRay(transform.position, forwardVec*5, Color.green);

        Quaternion moveRot = Quaternion.FromToRotation(moveDir, crossProd);
        moveDir = moveRot * Vector3.forward * currentSpeed * Time.deltaTime;
        Quaternion rotDir = Quaternion.LookRotation(moveDir, Vector3.up);
        
        //Vector3 idleDir = transform.forward;
        //Quaternion idleRot = Quaternion.FromToRotation(idleDir, crossProd);
        //idleDir = idleRot * transform.forward;
        if(_moveInput != Vector2.zero){
            transform.rotation = Quaternion.Slerp(transform.rotation, rotDir, rotationSpeed * Time.deltaTime);
        }
        // else{
        //     transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(idleDir, Vector3.up), rotationSpeed * Time.deltaTime);
        // }
    }
    private void HandleRotationToMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        Ray cameraRay = mainCamera.ScreenPointToRay(mousePos);
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
    }
}
