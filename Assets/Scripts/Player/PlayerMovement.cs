using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Space(5), Header("Running")]
    public float currentSpeed;
    public ModVar curSpeed;
    public float walkSpeed;
    public float runSpeed;
    public float currentStamina;
    public float staminaRegenSpeed;
    public float maxStamina;

    public float fallSpeed;

    [Header("State vars")]
    public bool canRegenStamina;
    public static bool isRunning;
    public bool runningAllowed = true;
    
    [Header("Movement Parameters")]
    //[Tooltip("Скорость передвижения игрока")]
    //public float moveSpeed = 5.0f;
    [Tooltip("Скорость поворота игрока, чтобы он смотрел в направлении движения")]
    public float rotationSpeed = 720.0f; // Градусов в секунду
    [Tooltip("Сила прыжка")]
    public float jumpForce = 8.0f;
    public float jumpStaminaDrain = 0.5f;
    public float jumpDelay = 0.3f;
    public bool canJump = true;
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
    

    [Space(10)]
    [Header("Actions")]
    public InputAction moveAction;
    public InputAction runAction;
    private Controls controls;

    private PlayerCameraController cameraController;
    private CharacterController characterController;
    private Camera mainCamera;
    private Animator animator;

    public Terrain terrain;
    
    void Awake()
    {
        controls = GameAssets.controls;
        enabled = false;
    }

    private PlayerAiming playerAiming;
    public void Init(PlayerCameraController cameraController, PlayerAiming playerAiming)
    {
        //Cursor.lockState = CursorLockMode.Confined;
        //Cursor.visible = true;

        this.cameraController = cameraController;
        this.playerAiming = playerAiming;
        mainCamera = cameraController.mainCamera;
        animator = GetComponent<Animator>();

        moveAction = controls.Player.Move;
        runAction = controls.Player.Sprint;
        characterController = GetComponent<CharacterController>();

        currentSpeed = walkSpeed;
        curSpeed = new ModVar(walkSpeed);
        
        terrain = GameObject.Find("Terrain").GetComponent<Terrain>();

        enabled = true;
    }

    protected void OnEnable()
    {
        controls.Player.Enable();
        //PlayerAiming.OnAimStart += () => runningAllowed = false;
        //PlayerAiming.OnAimEnd += () => runningAllowed = true;
        PlayerAiming.OnAim += PlayerAiming_OnAim;
    }

    protected void OnDisable()
    {
        controls.Player.Disable();
        PlayerAiming.OnAim -= PlayerAiming_OnAim;
        //PlayerAiming.OnAimStart -= () => runningAllowed = false;
        //PlayerAiming.OnAimEnd -= () => runningAllowed = true;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log($"SampleHeight: {terrain.SampleHeight(transform.position)}");
            
            Ray ray = new Ray(new Vector3(transform.position.x, 50, transform.position.z), Vector3.down);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                Debug.Log($"Angle: {Vector3.Angle(Vector3.up, hit.normal)}");
            }
        }
        
        
        HandleRunning();
        HandleMovement();
        if(PlayerAiming.isAiming){
            HandleRotationToMouse();
        }
        else{
            HandleRotationToVelocity();
        }
        HandleGravityAndJump();
        ApplyExternalForces();
        ApplyMovement();
        HandleAnimation();
        
        
        //HandleMovementAndRotation();
    }
    void FixedUpdate()
    {
        CheckIfGrounded();
    }
    private void CheckIfGrounded()
    {
        Vector3 spherePosition = transform.position + groundCheckOffset;
        _isGrounded = Physics.CheckSphere(spherePosition, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);
    }
    public bool CanJump(){
        return currentStamina >= jumpStaminaDrain;
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
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
            // Сглаживаем поворот персонажа
            //float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, turnSmoothTime);
            //transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Направление движения с учетом поворота камеры
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            _playerVelocity.x = moveDirection.x * currentSpeed;
            _playerVelocity.z = moveDirection.z * currentSpeed;
        }
        else
        {
            // Если нет ввода, постепенно останавливаем горизонтальное движение
            _playerVelocity.x = Mathf.Lerp(_playerVelocity.x, 0, Time.deltaTime * externalForceDamping); // Используем externalForceDamping для плавности остановки
            _playerVelocity.z = Mathf.Lerp(_playerVelocity.z, 0, Time.deltaTime * externalForceDamping);
        }
    }
    private void HandleRotationToVelocity()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        
        // Направление движения относительно ввода пользователя
        Vector3 inputDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        
        if (inputDirection.magnitude >= 0.1f)
        {
            // Получаем угол поворота относительно направления камеры
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
            // Сглаживаем поворот персонажа
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
    }
    private void HandleAnimation()
    {
        float speedX = Mathf.Clamp(_playerVelocity.x, -2, 2);
        float speedZ = Mathf.Clamp(_playerVelocity.z, -2, 2);
        if(!isRunning){
            speedX = Mathf.Clamp(_playerVelocity.x, -1, 1);
            speedZ = Mathf.Clamp(_playerVelocity.z, -1, 1);
        }
        animator.SetFloat("Speed X", Mathf.Abs(speedX), 0.1f, Time.deltaTime);
        animator.SetFloat("Speed Z", Mathf.Abs(speedZ), 0.1f, Time.deltaTime);
    }
    private void HandleGravityAndJump()
    {
        if (_isGrounded)
        {
            // Если на земле, сбрасываем вертикальную скорость (чтобы не накапливалась гравитация)
            // Небольшое отрицательное значение помогает "приклеить" к земле
            _playerVelocity.y = -2f;

            if (Input.GetKey(KeyCode.Space) && CanJump())
            {
                //_playerVelocity.y = jumpForce;
                AddForce(Vector3.up*jumpForce);
                
                if(canJump){
                    currentStamina -= jumpStaminaDrain;
                    EventBus.i.PlayerStaminaChanged?.Invoke(currentStamina, maxStamina);
                    canJump = false;
                    StartCoroutine(JumpDelay());
                }
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

#if UNITY_EDITOR
    // Отрисовка сферы для проверки земли в редакторе для удобства настройки
    void OnDrawGizmosSelected()
    {
        Gizmos.color = _isGrounded ? Color.green : Color.red;
        Vector3 spherePosition = transform.position + groundCheckOffset;
        Gizmos.DrawWireSphere(spherePosition, groundCheckRadius);
    }
#endif

    private void HandleMovementAndRotation()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        //animator.SetFloat("Speed X", moveInput.x);
        //animator.SetFloat("Speed Z", moveInput.y);
        
        Vector3 moveDir = new Vector3(-moveInput.x, 0, moveInput.y);
        //moveDir = moveDir.normalized * moveSpeed * Time.deltaTime;

        //Vector3 forwardVec = Quaternion.AngleAxis(90, Vector3.up) * (mainCamera.transform.position - transform.position);
        Vector3 forwardVec = mainCamera.transform.rotation * Vector3.forward;
        forwardVec = Quaternion.AngleAxis(-90, Vector3.up) * forwardVec;
        Vector3 crossProd = Vector3.Cross(transform.up, forwardVec);
        Debug.DrawRay(transform.position, forwardVec*5, Color.green);

        Quaternion moveRot = Quaternion.FromToRotation(moveDir, crossProd);
        if (moveInput.sqrMagnitude > 0)
        {
            moveDir = moveRot * Vector3.forward * currentSpeed * Time.deltaTime;
        }
        else
        {
            moveDir = Vector3.zero;
        }

        if (!characterController.isGrounded)
        {
            moveDir.y = -fallSpeed * Time.deltaTime;
        }
                
        characterController.Move(moveDir);
        
        float speedX = Mathf.Clamp(characterController.velocity.x, -2, 2);
        float speedZ = Mathf.Clamp(characterController.velocity.z, -2, 2);
        if(!isRunning){
            speedX = Mathf.Clamp(characterController.velocity.x, -1, 1);
            speedZ = Mathf.Clamp(characterController.velocity.z, -1, 1);
        }
        animator.SetFloat("Speed X", Mathf.Abs(speedX), 0.1f, Time.deltaTime);
        animator.SetFloat("Speed Z", Mathf.Abs(speedZ), 0.1f, Time.deltaTime);
        
        Debug.DrawRay(transform.position, crossProd * 2, Color.red);
        Debug.DrawRay(transform.position, moveDir * 5, Color.cyan);
    }
    
    Vector2 _moveInput;
    //Quaternion currentRotation;
    /*private void HandleRotationToVelocity()
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
    }*/
    private void HandleRotationToMouse() // хрень полная в этой новой системе
    {
        //Vector3 mousePos = Input.mousePosition;
        //Ray cameraRay = mainCamera.ScreenPointToRay(mousePos);
        // Определяем высоту персонажа (плоскость, на которой он стоит)
        //float planeY = transform.position.y;
        // Вычисляем, где луч пересекает эту высоту
        //float t = (planeY - cameraRay.origin.y) / cameraRay.direction.y;
        //Vector3 worldMousePos = cameraRay.origin + t * cameraRay.direction;
        // Убираем возможные отклонения по высоте
        //worldMousePos.y = transform.position.y;
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(worldMousePos - transform.position, Vector3.up), rotationSpeed * Time.deltaTime);
        //Debug.DrawLine(transform.position, worldMousePos, Color.yellow);
        
        Vector3 mousePosition = Input.mousePosition;
        // 2. Устанавливаем Z-координату для ScreenToWorldPoint.
        // Это расстояние от камеры до плоскости, на которую мы "проецируем" мышь.
        mousePosition.z = 10f; // distanceFromCamera
        
        // 3. Преобразуем экранные координаты мыши в мировые координаты
        Vector3 targetWorldPoint = mainCamera.ScreenToWorldPoint(mousePosition);
        // 4. Рассчитываем направление от персонажа к этой точке
        Vector3 directionToLook = targetWorldPoint - transform.position;
        // 5. Игнорируем разницу по оси Y для вращения только по горизонтали
        directionToLook.y = 0;

        if (directionToLook.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToLook.normalized); // Нормализуем для чистоты
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    
    public void RotateToMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        // 2. Устанавливаем Z-координату для ScreenToWorldPoint.
        // Это расстояние от камеры до плоскости, на которую мы "проецируем" мышь.
        mousePosition.z = 10f; // distanceFromCamera
        // 3. Преобразуем экранные координаты мыши в мировые координаты
        Vector3 targetWorldPoint = mainCamera.ScreenToWorldPoint(mousePosition);
        // 4. Рассчитываем направление от персонажа к этой точке
        Vector3 directionToLook = targetWorldPoint - transform.position;
        // 5. Игнорируем разницу по оси Y для вращения только по горизонтали
        directionToLook.y = 0;

        if (directionToLook.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToLook.normalized); // Нормализуем для чистоты
            //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = targetRotation;
        }
    }

    private void PlayerAiming_OnAim(bool aimStarted)
    {
        if (aimStarted)
        {
            runningAllowed = false;
            //ChangeSpeed(-40f);
            curSpeed.Add(-40f);
            currentSpeed = curSpeed.Get();
        }
        else
        {
            runningAllowed = true;
            curSpeed.Remove();
            currentSpeed = curSpeed.Get();
            //ResetSpeed();
        }
    }
    // public void ChangeSpeed(float percent)
    // {
    //     currentSpeed = walkSpeed + (walkSpeed * (percent / 100));
    // }
    public void ResetSpeed()
    {
        currentSpeed = walkSpeed;
    }
    
    public bool CanRun()
    {
        if (currentStamina > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private Coroutine _fovRoutine;
    private void HandleRunning()
    {
        if (runAction.IsPressed() && !isRunning && CanRun() && runningAllowed)
        {
            StopCoroutine(nameof(StaminaRegenDelayed));
            canRegenStamina = false;
            currentSpeed = runSpeed;
            isRunning = true;
            
            if (_fovRoutine != null) // Просто переделать всё под PrimeTween
            {
                StopCoroutine(_fovRoutine);
            }
            _fovRoutine = StartCoroutine(cameraController.ChangeFov(cameraController.currentFov, cameraController.fov_running, 0.3f));
        }
        else if (!runAction.IsPressed() && isRunning)
        {
            StartCoroutine(StaminaRegenDelayed(1.5f));
            //currentSpeed = walkSpeed;
            currentSpeed = curSpeed.Get();
            isRunning = false;
            
            if (_fovRoutine != null)
            {
                StopCoroutine(_fovRoutine);
            }
            _fovRoutine = StartCoroutine(cameraController.ChangeFov(cameraController.currentFov, cameraController.fov_default, 0.3f));
        }
        else if (runAction.IsPressed() && isRunning && !CanRun())
        {
            StartCoroutine(StaminaRegenDelayed(1.5f));
            //currentSpeed = walkSpeed;
            currentSpeed = curSpeed.Get();
            isRunning = false;
            
            if (_fovRoutine != null)
            {
                StopCoroutine(_fovRoutine);
            }
            _fovRoutine = StartCoroutine(cameraController.ChangeFov(cameraController.currentFov, cameraController.fov_default, 0.3f));
        }

        HandleStamina();

        //Debug.Log($"Stamina: {currentStamina}");
    }

    private void HandleStamina()
    {
        if (isRunning)
        {
            currentStamina -= 1 * Time.deltaTime;
            EventBus.i.PlayerStaminaChanged?.Invoke(currentStamina, maxStamina);
        }

        if (!isRunning && canRegenStamina && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenSpeed * Time.deltaTime;
            EventBus.i.PlayerStaminaChanged?.Invoke(currentStamina, maxStamina);
        }
    }

    private IEnumerator StaminaRegenDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        canRegenStamina = true;
    }
    
    
    private IEnumerator JumpDelay(){
        
        yield return new WaitForSeconds(jumpDelay);
        canJump = true;
    }
}

public class ModVar
{
    public float baseValue;
    public Stack<float> stack;

    public ModVar(float startingValue)
    {
        stack = new Stack<float>();
        
        baseValue = startingValue;
        stack.Push(startingValue);
    }

    public void Add(float percent)
    {
        float newValue = stack.Peek() + (stack.Peek() * percent / 100);
        stack.Push(newValue);
        // foreach (float item in stack)
        // {
        //     Debug.Log("stack item: " + item);
        // }
    }

    public void Remove()
    {
        stack.Pop();
    }

    public float Get()
    {
        // foreach (float item in stack)
        // {
        //     Debug.Log("stack item: " + item);
        // }
        return stack.Peek();
    }
}
