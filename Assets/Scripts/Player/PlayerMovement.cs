using System.Collections;
using Unity.Cinemachine;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerMovement : MonoBehaviour
{
    public float currentSpeed;
    public float walkSpeed;
    [Space(5), Header("Running")]
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
    

    [Space(10)]
    [Header("Actions")]
    public InputAction moveAction;
    public InputAction runAction;
    public InputAction zoomAction;

    private CharacterController characterController;
    private CinemachineOrbitalFollow cmFollow;
    private Camera cm;

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        cm = Camera.main;
        cmFollow = GameObject.Find("CinemachineCamera").GetComponent<CinemachineOrbitalFollow>();

        moveAction = InputSystem.actions.FindAction("Move");
        zoomAction = InputSystem.actions.FindAction("Zoom");
        runAction = InputSystem.actions.FindAction("Sprint");
        characterController = GetComponent<CharacterController>();

        currentSpeed = walkSpeed;
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

        HandleRunning();
        HandleMovementAndRotation();
    }

    private void HandleMovementAndRotation()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 moveDir = new Vector3(-moveInput.x, 0, moveInput.y);
        //moveDir = moveDir.normalized * moveSpeed * Time.deltaTime;

        Vector3 forwardVec = Quaternion.AngleAxis(90, Vector3.up) * (cm.transform.position - transform.position);
        Vector3 crossProd = Vector3.Cross(transform.up, forwardVec);

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

    private void HandleRunning()
    {
        if (runAction.IsPressed() && !isRunning && CanRun())
        {
            StopCoroutine(nameof(StaminaRegenDelayed));
            canRegenStamina = false;
            currentSpeed = runSpeed;
            isRunning = true;
        }
        else if (!runAction.IsPressed() && isRunning)
        {
            StartCoroutine(StaminaRegenDelayed(1.5f));
            currentSpeed = walkSpeed;
            isRunning = false;
        }
        else if (runAction.IsPressed() && isRunning && !CanRun())
        {
            StartCoroutine(StaminaRegenDelayed(1.5f));
            currentSpeed = walkSpeed;
            isRunning = false;
        }

        HandleStamina();

        Debug.Log($"Stamina: {currentStamina}");
    }

    private void HandleStamina()
    {
        if (isRunning)
        {
            currentStamina -= 1 * Time.deltaTime;
        }

        if (!isRunning && canRegenStamina)
        {
            currentStamina += staminaRegenSpeed * Time.deltaTime;
        }
    }

    private IEnumerator StaminaRegenDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        canRegenStamina = true;
    }
    
}
