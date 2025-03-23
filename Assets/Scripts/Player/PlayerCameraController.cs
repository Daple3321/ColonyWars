using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class PlayerCameraController : MonoBehaviour
{
    public CinemachineOrbitalFollow cmFollow;
    public CinemachineCamera cinemachineCamera;
    public CinemachinePositionComposer positionComposer;
    public Camera mainCamera;
    
    
    [Space(5), Header("Camera controls")]
    public InputAction zoomAction;
    public Vector2 zoomLimits;
    public float zoomSpeed;
    public InputAction panAction;
    public float panSensetivity;
    

    [Space(10), Header("FOV settings")]
    public float currentFov;
    public float fov_default = 65f;
    public float fov_running = 75f; // сделать через multiplier (фов же можно будет настроить)
    public AnimationCurve fovCurve;

    private PlayerFollow playerFollow;

    void Awake()
    {
        enabled = false;
    }

    public void Init(PlayerFollow playerFollow)
    {
        //cmFollow = transform.Find("CinemachineCamera").GetComponent<CinemachineOrbitalFollow>();
        positionComposer = transform.Find("CinemachineCamera").GetComponent<CinemachinePositionComposer>();
        cinemachineCamera = transform.Find("CinemachineCamera").GetComponent<CinemachineCamera>();
        mainCamera = transform.Find("Main Camera").GetComponent<Camera>();
        
        zoomAction = InputSystem.actions.FindAction("Zoom");
        panAction = InputSystem.actions.FindAction("CameraPan_Mouse");
        
        this.playerFollow = playerFollow;
        cinemachineCamera.Target.TrackingTarget = playerFollow.transform;

        mainCamera.transform.SetParent(null);
        cinemachineCamera.transform.SetParent(null);

        ResetFov();

        enabled = true;
    }

    void Update()
    {
        HandleCameraControl();
    }

    public void HandleCameraControl()
    {
        HandleCameraPanning();
        HandleCameraZoom();
    }

    public void HandleCameraZoom()
    {
        float scrollY = zoomAction.ReadValue<float>();
        if (scrollY > 0 && positionComposer.CameraDistance > zoomLimits.x)
        {
            positionComposer.CameraDistance -= zoomSpeed * Time.deltaTime;
        }
        else if (scrollY < 0 && positionComposer.CameraDistance < zoomLimits.y)
        {
            positionComposer.CameraDistance += zoomSpeed * Time.deltaTime;
        }
    }

    Vector2 lastMousePos = new Vector2();
    public void HandleCameraPanning()
    {
        Vector2 mousePos = Input.mousePosition;
        float delta = 0f;
        if (panAction.WasPressedThisFrame())
        {
            lastMousePos = mousePos;
        }

        if (panAction.IsPressed())
        {
            delta = mousePos.x - lastMousePos.x;
            //new Vector3(0, delta * panSensetivity, 0)
            cinemachineCamera.transform.RotateAround(playerFollow.transform.position, Vector3.up, delta*Time.deltaTime*panSensetivity);
            lastMousePos = Input.mousePosition;
        }
    }
    
    public IEnumerator ChangeFov(float from, float target, float waitTime)
    {
        float elapsedTime = 0f;
        while (elapsedTime < waitTime)
        {
            float normalizedProgess = elapsedTime / waitTime;
            float easing = fovCurve.Evaluate(normalizedProgess);
            cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(from, target, easing);

            currentFov = cinemachineCamera.Lens.FieldOfView;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cinemachineCamera.Lens.FieldOfView = target;
        currentFov = cinemachineCamera.Lens.FieldOfView;
    }

    public void ResetFov()
    {
        cinemachineCamera.Lens.FieldOfView = fov_default;
        currentFov = cinemachineCamera.Lens.FieldOfView;
    }
}
