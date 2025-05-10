using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    public CinemachineOrbitalFollow cmFollow;
    public CinemachineCamera cinemachineCamera;
    public CinemachinePositionComposer positionComposer;
    public CinemachineInputAxisController cinemachineInputAxisController;
    public CinemachineThirdPersonFollow cinemachineThirdPerson; 
    public static CinemachineBasicMultiChannelPerlin camNoise;
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
    
    public bool rotationEnabled = true;

    private Transform playerFollow;

    void Awake()
    {
        enabled = false;
    }

    public void Init(PlayerFollow playerFollow)
    {
        //cmFollow = transform.Find("CinemachineCamera").GetComponent<CinemachineOrbitalFollow>();
        //positionComposer = transform.Find("CinemachineCamera").GetComponent<CinemachinePositionComposer>();
        cinemachineCamera = GameObject.Find("CinemachineCamera").GetComponent<CinemachineCamera>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        camNoise = cinemachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineInputAxisController = cinemachineCamera.GetComponent<CinemachineInputAxisController>();
        cinemachineThirdPerson = cinemachineCamera.GetComponent<CinemachineThirdPersonFollow>();
        Noise(0, 0);
        EventBus.i.PlayerDeath += () => {Noise(0,0);};

        controls = GameAssets.controls;
        //zoomAction = InputSystem.actions.FindAction("Zoom");
        //panAction = InputSystem.actions.FindAction("CameraPan_Mouse");

        this.playerFollow = playerFollow.transform;
        cinemachineCamera.Target.TrackingTarget = playerFollow.transform;

        mainCamera.transform.SetParent(null);
        cinemachineCamera.transform.SetParent(null);

        ResetFov();

        xRotate = cinemachineCamera.transform.rotation.eulerAngles.x;
        yRotate = cinemachineCamera.transform.rotation.eulerAngles.y;
        
        EventBus.i.OnInteractivePanelOpened += () => {
            //cinemachineInputAxisController.enabled = false;
            rotationEnabled = false;
        };
        EventBus.i.OnInteractivePanelClosed += () => {
            //cinemachineInputAxisController.enabled = true;
            rotationEnabled = true;
        };
        
        enabled = true;
    }

    void Update()
    {
        HandleCameraControl();
    }

    public void HandleCameraControl()
    {
        if(rotationEnabled){
            HandleCameraRotation();
            HandleCameraZoom();
        }
    }

    public void HandleCameraZoom()
    {
        float scrollY = controls.Player.Zoom.ReadValue<float>();
        if (scrollY > 0 && cinemachineThirdPerson.CameraDistance > zoomLimits.x)
        {
            cinemachineThirdPerson.CameraDistance -= zoomSpeed * Time.deltaTime;
        }
        else if (scrollY < 0 && cinemachineThirdPerson.CameraDistance < zoomLimits.y)
        {
            cinemachineThirdPerson.CameraDistance += zoomSpeed * Time.deltaTime;
        }
    }

    Vector2 lastMousePos = new Vector2();
    float xRotate = 0;
    float yRotate = 0;
    public void HandleCameraRotation()
    {
        // Vector2 panDir = controls.Player.CameraPan.ReadValue<Vector2>();
        // if (panDir.x > 0)
        // {
        //     //cinemachineCamera.transform.RotateAround(playerFollow.transform.position, Vector3.up, Time.deltaTime * panSensetivity);
        //     yRotate += Time.deltaTime * panSensetivity;
        //     cinemachineCamera.transform.eulerAngles = new Vector3(xRotate, yRotate, 0.0f);
        // }
        // else if (panDir.x < 0)
        // {
        //     yRotate -= Time.deltaTime * panSensetivity;
        //     cinemachineCamera.transform.eulerAngles = new Vector3(xRotate, yRotate, 0.0f);
        //     //cinemachineCamera.transform.RotateAround(playerFollow.transform.position, Vector3.up, Time.deltaTime * -panSensetivity);
        // }


        Vector2 mousePos = Input.mousePosition;
        float deltaY;
        Vector2 mouseDelta;
        // if (controls.Player.CameraTilt_Mouse.WasPressedThisFrame())
        // {
        //     lastMousePos = mousePos;
        //     Cursor.lockState = CursorLockMode.Locked;
        // }
        // if(controls.Player.CameraTilt_Mouse.WasReleasedThisFrame()){
        //     Cursor.lockState = CursorLockMode.None;
        // }
        //Vector3 dirToCamera = transform.position - cinemachineCamera.transform.position;
        //float cameraAngle = Vector3.Angle(transform.up, dirToCamera);
        //Debug.Log($"Camera angle: {cameraAngle}");
        
        
        mouseDelta = Input.mousePositionDelta;
        deltaY = mousePos.y - lastMousePos.y;
        xRotate += -mouseDelta.y * Time.deltaTime * tiltSensetivity;
        xRotate = Mathf.Clamp(xRotate, tiltAngleLimits.x, tiltAngleLimits.y);
        playerFollow.eulerAngles = new Vector3(xRotate, yRotate, 0.0f);
        
        float deltaX = mousePos.x - lastMousePos.x;
        if(deltaX > 0){
            yRotate += mouseDelta.x * Time.deltaTime * panSensetivity;
            playerFollow.eulerAngles = new Vector3(xRotate, yRotate, 0.0f);
        }
        else if(deltaX < 0){
            yRotate += mouseDelta.x * Time.deltaTime * panSensetivity;
            playerFollow.eulerAngles = new Vector3(xRotate, yRotate, 0.0f);
        }
        //cinemachineCamera.transform.RotateAround(playerFollow.transform.position, Vector3.right, -delta * Time.deltaTime * tiltSensetivity);
        //lastMousePos = Input.mousePosition;
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
    
    public static IEnumerator CameraShake(float shakeIntensity = 5f, float shakeTiming = 0.5f)
    {
        Noise(1, shakeIntensity);
        yield return new WaitForSeconds(shakeTiming);
        Noise(0, 0);
    }
    public static void Noise(float amplitudeGain, float frequencyGain)
    {
        camNoise.AmplitudeGain = amplitudeGain;  
        camNoise.FrequencyGain = frequencyGain;
    }   
}
