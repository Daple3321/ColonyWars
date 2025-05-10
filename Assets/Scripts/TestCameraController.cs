using System;
using Unity.Cinemachine;
using UnityEngine;

public class TestCameraController : MonoBehaviour
{
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
    
    public CinemachineThirdPersonFollow cinemachineThirdPerson; 
    public Transform playerFollow;
    
    private Controls controls;
    public void Init(Controls c)
    {
        controls = c;
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
        playerFollow.transform.eulerAngles = new Vector3(xRotate, yRotate, 0.0f);
        
        float deltaX = mousePos.x - lastMousePos.x;
        if(deltaX > 0){
            yRotate += mouseDelta.x * Time.deltaTime * panSensetivity;
            playerFollow.transform.eulerAngles = new Vector3(xRotate, yRotate, 0.0f);
        }
        else if(deltaX < 0){
            yRotate += mouseDelta.x * Time.deltaTime * panSensetivity;
            playerFollow.transform.eulerAngles = new Vector3(xRotate, yRotate, 0.0f);
        }
        //cinemachineCamera.transform.RotateAround(playerFollow.transform.position, Vector3.right, -delta * Time.deltaTime * tiltSensetivity);
        //lastMousePos = Input.mousePosition;
    }
}
