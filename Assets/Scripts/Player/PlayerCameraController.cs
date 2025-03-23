using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    public CinemachineOrbitalFollow cmFollow;
    public CinemachineCamera cinemachineCamera;

    public float currentFov;
    public float fov_default = 65f;
    public float fov_running = 75f; // сделать через multiplier (фов же можно будет настроить)

    public AnimationCurve fovCurve;

    void Awake()
    {
        enabled = false;
    }

    public void Init()
    {
        cmFollow = transform.Find("CinemachineCamera").GetComponent<CinemachineOrbitalFollow>();
        cinemachineCamera = transform.Find("CinemachineCamera").GetComponent<CinemachineCamera>();

        ResetFov();

        enabled = true;
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
