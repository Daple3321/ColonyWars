using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public float rotationSpeed;
    public float zoomSpeed;

    [Header("State vars")]
    public bool canRegenStamina;
    public static bool isRunning;
    public bool runningAllowed = true;
    

    [Space(10)]
    [Header("Actions")]
    public InputAction moveAction;
    public InputAction runAction;
    private Controls controls;

    private PlayerCameraController cameraController;
    private CharacterController characterController;
    private Camera mainCamera;

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
        Cursor.visible = true;

        this.cameraController = cameraController;
        this.playerAiming = playerAiming;
        mainCamera = cameraController.mainCamera;

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
        //Debug.DrawRay(transform.position, moveDir * 5, Color.magenta);
        // ------------------------------------------------- //
        // Vector2 moveInput = moveAction.ReadValue<Vector2>();
        // Vector3 inputDir = new Vector3(moveInput.x, 0, moveInput.y);
        // Vector3 moveDirFixed = transform.rotation * inputDir;
        // moveDirFixed = moveDirFixed.normalized * moveSpeed * Time.deltaTime;
        // if (!characterController.isGrounded)
        // {
        //     moveDirFixed.y = -fallSpeed * Time.deltaTime;
        // }
        //Debug.DrawRay(transform.position, moveDirFixed * 5, Color.blue);

        //Vector3 rotationVector = Quaternion.AngleAxis(90, Vector3.up) * (cm.transform.position - transform.position);
        //Vector3 crossProd = Vector3.Cross(transform.up, rotationVector);
        //Debug.DrawRay(transform.position, crossProd * 5, Color.red);

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

        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log($"SampleHeight: {terrain.SampleHeight(transform.position)}");

            // float normX = (float)1 / (float)terrain.terrainData.alphamapWidth;
            // float normY = (float)1 / (float)terrain.terrainData.alphamapHeight;

            // float height = terrain.terrainData.GetHeight(
            //     Mathf.RoundToInt(normY * terrain.terrainData.heightmapResolution), Mathf.RoundToInt(normX * terrain.terrainData.heightmapResolution));
            // Debug.Log($"Height in alphamap (5,5): {height}");
        }

        HandleRunning();
        HandleMovementAndRotation();
    }

    private void HandleMovementAndRotation()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
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
        Debug.DrawRay(transform.position, crossProd * 2, Color.red);
        Debug.DrawRay(transform.position, moveDir * 5, Color.cyan);

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
