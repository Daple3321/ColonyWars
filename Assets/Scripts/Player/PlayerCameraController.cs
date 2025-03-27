using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    public CinemachineOrbitalFollow cmFollow;
    public CinemachineCamera cinemachineCamera;
    public CinemachinePositionComposer positionComposer;
    public Camera mainCamera;
    private Controls controls;
    
    
    [Space(5), Header("Camera controls")]
    //public InputAction zoomAction;
    public Vector2 zoomLimits;
    public float zoomSpeed;
    //public InputAction panAction;
    [Header("Tilt")]
    public float tiltSensetivity;
    public Vector2 tiltAngleLimits;
    [Header("Panning")]
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

        controls = GameAssets.controls;
        //zoomAction = InputSystem.actions.FindAction("Zoom");
        //panAction = InputSystem.actions.FindAction("CameraPan_Mouse");
        
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
        HandleCameraRotation();
        //HandleCameraPanning();
        HandleCameraZoom();
    }

    public void HandleCameraZoom()
    {
        float scrollY = controls.Player.Zoom.ReadValue<float>();
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
    float xRotate = 0;
    float yRotate = 0;
    public void HandleCameraRotation() // это просто ужас.
    {
        Vector2 panDir = controls.Player.CameraPan.ReadValue<Vector2>();
        if (panDir.x > 0)
        {
            //cinemachineCamera.transform.RotateAround(playerFollow.transform.position, Vector3.up, Time.deltaTime * panSensetivity);
            yRotate += Time.deltaTime * panSensetivity;
            cinemachineCamera.transform.eulerAngles = new Vector3(xRotate, yRotate, 0.0f);
        }
        else if (panDir.x < 0)
        {
            yRotate -= Time.deltaTime * panSensetivity;
            cinemachineCamera.transform.eulerAngles = new Vector3(xRotate, yRotate, 0.0f);
            //cinemachineCamera.transform.RotateAround(playerFollow.transform.position, Vector3.up, Time.deltaTime * -panSensetivity);
        }


        Vector2 mousePos = Input.mousePosition;
        float delta;
        if (controls.Player.CameraTilt_Mouse.WasPressedThisFrame())
        {
            lastMousePos = mousePos;
        }
        //Vector3 dirToCamera = transform.position - cinemachineCamera.transform.position;
        //float cameraAngle = Vector3.Angle(transform.up, dirToCamera);
        //Debug.Log($"Camera angle: {cameraAngle}");
        if (controls.Player.CameraTilt_Mouse.IsPressed())
        {
            delta = mousePos.y - lastMousePos.y;
            xRotate += delta * Time.deltaTime * tiltSensetivity;
            xRotate = Mathf.Clamp(xRotate, tiltAngleLimits.x, tiltAngleLimits.y);
            cinemachineCamera.transform.eulerAngles = new Vector3(xRotate, yRotate, 0.0f);
            //cinemachineCamera.transform.RotateAround(playerFollow.transform.position, Vector3.right, -delta * Time.deltaTime * tiltSensetivity);
            //lastMousePos = Input.mousePosition;
        }
    }

    public void HandleCameraPanning()
    {
        Vector2 panDir = controls.Player.CameraPan.ReadValue<Vector2>();
        if (panDir.x > 0)
        {
            cinemachineCamera.transform.RotateAround(playerFollow.transform.position, Vector3.up, Time.deltaTime * panSensetivity);
        }
        else if (panDir.x < 0)
        {
            cinemachineCamera.transform.RotateAround(playerFollow.transform.position, Vector3.up, Time.deltaTime * -panSensetivity);
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
