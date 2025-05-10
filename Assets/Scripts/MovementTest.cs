using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementTest : MonoBehaviour
{
    public CinemachineOrbitalFollow cmFollow;
    public CinemachineCamera cinemachineCamera;
    public CinemachinePositionComposer positionComposer;
    public Transform cameraTransform;
    public CharacterController characterController;
    
    // public float currentSpeed = 5f;
    // public float fallSpeed = 10f;
    // public float rotationSpeed = 10f;
    
    [Header("Movement Parameters")]
    [Tooltip("Скорость передвижения игрока")]
    public float moveSpeed = 5.0f;
    [Tooltip("Скорость поворота игрока, чтобы он смотрел в направлении движения")]
    public float rotationSpeed = 720.0f; // Градусов в секунду
    [Tooltip("Сила прыжка")]
    public float jumpForce = 8.0f;
    [Tooltip("Сила гравитации")]
    public float gravity = 20.0f;
    [Tooltip("Коэффициент сглаживания для поворота к направлению движения")]
    public float turnSmoothTime = 0.1f;
    
    [Header("Ground Check")]
    [Tooltip("Радиус проверки нахождения на земле")]
    public float groundCheckRadius = 0.3f;
    [Tooltip("Смещение точки проверки нахождения на земле относительно центра объекта")]
    public Vector3 groundCheckOffset = new Vector3(0, -0.8f, 0); // Подстройте под вашего персонажа
    [Tooltip("Маска слоев, которые считаются землей")]
    public LayerMask groundLayer;
    
    [Header("External Forces")]
    [Tooltip("Коэффициент затухания внешней силы (как быстро игрок остановится после толчка)")]
    public float externalForceDamping = 5.0f;
    
    private Vector3 _playerVelocity; // Скорость игрока, включая гравитацию и прыжок
    private Vector3 _externalForce;  // Внешние силы, действующие на игрока (например, отталкивание)
    private bool _isGrounded;
    private float _turnSmoothVelocity;
    
    public PlayerCameraController playerCamera;
    public PlayerFollow playerFollow;
    public TestCameraController cameraController;
    public Rigidbody rb;
    
    [Space(10)]
    [Header("Actions")]
    public InputAction moveAction;
    public InputAction runAction;
    private Controls controls;
    public Texture2D customCursor;
    void Awake()
    {
        //GameAssets.Init();
        rb = GetComponent<Rigidbody>();
        controls = new Controls();
        controls.Player.Enable();
        //controls = GameAssets.controls;
        
        cameraTransform = Camera.main.transform;
        moveAction = controls.Player.Move;
        runAction = controls.Player.Sprint;
        characterController = GetComponent<CharacterController>();
        
        //CrosshairManager.i.Init();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        //playerCamera.Init(playerFollow);
        
        cameraController.Init(controls);
        //Cursor.SetCursor(customCursor, new Vector2(36, 36), CursorMode.Auto);
    }

    void Update()
    {
        HandleMovement();
        HandleGravityAndJump();
        ApplyExternalForces();
        ApplyMovement();
    }

    void FixedUpdate()
    {
        //HandleMovementAndRotation();
        //HandleRotationToVelocity();
        CheckIfGrounded();
    }
    private void CheckIfGrounded()
    {
        Vector3 spherePosition = transform.position + groundCheckOffset;
        _isGrounded = Physics.CheckSphere(spherePosition, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);
    }
    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Направление движения относительно ввода пользователя
        Vector3 inputDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        if (inputDirection.magnitude >= 0.1f)
        {
            // Получаем угол поворота относительно направления камеры
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            // Сглаживаем поворот персонажа
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Направление движения с учетом поворота камеры
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            _playerVelocity.x = moveDirection.x * moveSpeed;
            _playerVelocity.z = moveDirection.z * moveSpeed;
        }
        else
        {
            // Если нет ввода, постепенно останавливаем горизонтальное движение
            _playerVelocity.x = Mathf.Lerp(_playerVelocity.x, 0, Time.deltaTime * externalForceDamping); // Используем externalForceDamping для плавности остановки
            _playerVelocity.z = Mathf.Lerp(_playerVelocity.z, 0, Time.deltaTime * externalForceDamping);
        }
    }
    private void HandleGravityAndJump()
    {
        if (_isGrounded)
        {
            // Если на земле, сбрасываем вертикальную скорость (чтобы не накапливалась гравитация)
            // Небольшое отрицательное значение помогает "приклеить" к земле
            _playerVelocity.y = -2f;

            if (Input.GetKey(KeyCode.Space))
            {
                //_playerVelocity.y = jumpForce;
                AddForce(Vector3.up*jumpForce);
            }
        }
        else
        {
            // Применяем гравитацию, если не на земле
            _playerVelocity.y -= gravity * Time.deltaTime;
        }
    }
    private void ApplyExternalForces()
    {
        if (_externalForce.magnitude > 0.01f)
        {
            characterController.Move(_externalForce * Time.deltaTime);
            // Затухание внешней силы
            _externalForce = Vector3.Lerp(_externalForce, Vector3.zero, externalForceDamping * Time.deltaTime);
        }
    }

    private void ApplyMovement()
    {
        // Объединяем движение от ввода, гравитацию/прыжок и внешние силы
        Vector3 finalVelocity = new Vector3(_playerVelocity.x, _playerVelocity.y, _playerVelocity.z);
        characterController.Move(finalVelocity * Time.deltaTime);
    }
    
    /// <summary>
    /// Применяет внешнюю силу к игроку (например, отталкивание).
    /// </summary>
    /// <param name="force">Вектор силы.</param>
    public void AddForce(Vector3 force)
    {
        // Если сила направлена в основном вверх (например, подбрасывание),
        // то добавляем ее к вертикальной скорости, чтобы она сочеталась с гравитацией.
        // Иначе, добавляем как горизонтальную внешнюю силу.
        if (Mathf.Abs(force.y) > Mathf.Abs(force.x) + Mathf.Abs(force.z)) // Проверяем, доминирует ли Y-компонента
        {
            _playerVelocity.y += force.y; // Добавляем к текущей вертикальной скорости (влияет на прыжок/падение)
             _externalForce += new Vector3(force.x, 0, force.z); // Горизонтальную часть добавляем как обычно
        }
        else
        {
            _externalForce += force;
        }
    }

    // Отрисовка сферы для проверки земли в редакторе для удобства настройки
    void OnDrawGizmosSelected()
    {
        Gizmos.color = _isGrounded ? Color.green : Color.red;
        Vector3 spherePosition = transform.position + groundCheckOffset;
        Gizmos.DrawWireSphere(spherePosition, groundCheckRadius);
    }

    /*private void HandleMovementAndRotation()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        
        Vector3 moveDir = new Vector3(-moveInput.x, 0, moveInput.y);

        Vector3 forwardVec = cameraTransform.rotation * Vector3.forward;
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
            //moveDir.y = -fallSpeed * Time.deltaTime;
            moveDir.y = Mathf.Lerp(0, -fallSpeed, Time.deltaTime);
        }
                
        characterController.Move(moveDir);
        //rb.MovePosition(transform.position + moveDir);
        //rb.AddForce(moveDir);
        
        Debug.DrawRay(transform.position, moveInput * 10, Color.magenta);
        Debug.DrawRay(transform.position, crossProd * 2, Color.red);
        Debug.DrawRay(transform.position, moveDir * 5, Color.cyan);
    }*/
    
    Vector2 _moveInput;
    /*private void HandleRotationToVelocity()
    {
        _moveInput = moveAction.ReadValue<Vector2>();
        Vector3 moveDir = new Vector3(-_moveInput.x, 0, _moveInput.y);
        
        Vector3 forwardVec = cameraTransform.rotation * Vector3.forward;
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
    }*/
    
    /*private void HandleRotationToMouse()
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
    }*/
    
    
    
    // public void ZoomTowardPoint2(InputAction.CallbackContext context)
    // {
    //     if (!context.performed)
    //         return;
       
    //     float zoomSpeed1 = 50;
    //     float minZoom = 0;
    //     float maxZoom = 1000;

    //     var scrollwheelValue = -context.ReadValue<Vector2>().y;
    //     Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
    //     RaycastHit point;
    //     Physics.Raycast(ray, out point, 1000);
    //     Vector3 Scrolldirection = ray.GetPoint(5);

    //     float step = zoomSpeed1 * Time.deltaTime;

    //     // Allows zooming in and out via the mouse wheel
    //     if (scrollwheelValue > 0 && Scrolldirection.y > minZoom)
    //     {
    //         MoveTowardsPointOribital(transform.position, Scrolldirection, scrollwheelValue * step);
    //     }
    //     if (scrollwheelValue < 0 && Scrolldirection.y < maxZoom)
    //     {
    //         MoveTowardsPointOribital(transform.position, Scrolldirection, scrollwheelValue * step);
    //     }
    // }

    // private void MoveTowardsPointOribital(Vector3 current, Vector3 target, float maxDistanceDelta)
    // {
    //     //Finds the vector we want to move towards and then gives us a point on that vector
    //     Vector3 desiredPoint;
    //     Vector3 a = target - current;
    //     float magnitude = a.magnitude;
    //     if (magnitude <= maxDistanceDelta || magnitude == 0f)
    //     {
    //         desiredPoint = target;
    //     }
    //     desiredPoint = current + a / magnitude * maxDistanceDelta;
      
    //     float heightDeltaRatio = desiredPoint.y / mainCamera.transform.position.y;
    //     //adjusts the orbit to bring why down to correct height
    //     zoomDistance *= heightDeltaRatio;
    //     UpdateRadiusHeightofRigs();
    //     mainCamera.GetComponent<CinemachineBrain>().ManualUpdate();
      
    //     //need to correct for new x and z after updated rigs
    //     Vector3 finalDesiredChange = desiredPoint - cinemachineFreeLook.transform.position;
    //     finalDesiredChange = new Vector3(finalDesiredChange.x, 0, finalDesiredChange.z);
    //     //moves focalPoint which in turn moves Orbital Camera by desired amount while
    //     focalPointRB.transform.position += finalDesiredChange;
    // }

    // private void UpdateRadiusHeightofRigs()
    // {
    //     for (int i = 0; i < 3; i++)
    //     {
    //         cmFollow.m_Orbits[i].m_Radius = orbitRadii[i] * zoomDistance;
    //         cinemachineFreeLook.m_Orbits[i].m_Height = orbitHeight[i] * zoomDistance;
    //     }
    // }
}
