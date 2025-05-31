using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TimeManager : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public TimeSettings timeSettings;

    [SerializeField] Light sun;
    [SerializeField] Light moon;
    [SerializeField] AnimationCurve lightIntensityCurve;
    [SerializeField] float maxSunIntensity = 1;
    [SerializeField] float maxMoonIntensity = 0.5f;

    [SerializeField] AnimationCurve sunTemperatureCurve;
    [SerializeField] float maxSunTemperature;

    [SerializeField] Color dayAmbientLight;
    [SerializeField] Color nightAmbientLight;
    [SerializeField] Volume volume;
    
    [SerializeField] Material skyboxMaterial;
    ColorAdjustments colorAdjustments;

    TimeService service;

    // void Start()
    // {
    //     service = new TimeService(timeSettings);
    //     volume.profile.TryGet(out colorAdjustments);
    // }

    void Awake()
    {
        enabled = false;
    }

    public void Init()
    {
        service = new TimeService(timeSettings);
        volume.profile.TryGet(out colorAdjustments);

        enabled = true;
    }


    void Update()
    {
        UpdateTimeOfDay();
        RotateSun();
        //RotateMoon();
        UpdateLightSettings();
        UpdateSkyBlend();

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            timeSettings.timeMultiplier *= 2;
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            timeSettings.timeMultiplier /= 2;
        }  
    }

    void UpdateLightSettings()
    {
        float dotProduct = Vector3.Dot(sun.transform.forward, Vector3.down);
        //Debug.Log("Sun DotProd: " + dotProduct);
        sun.intensity = Mathf.Lerp(0, maxSunIntensity, lightIntensityCurve.Evaluate(dotProduct));
        sun.colorTemperature = Mathf.Lerp(1500, maxSunTemperature, sunTemperatureCurve.Evaluate(dotProduct));
        moon.intensity = Mathf.Lerp(0, maxMoonIntensity, lightIntensityCurve.Evaluate(dotProduct));

        if (colorAdjustments == null) return;
        colorAdjustments.colorFilter.value = Color.Lerp(nightAmbientLight, dayAmbientLight, lightIntensityCurve.Evaluate(dotProduct));
    }
    
    void RotateSun()
    {
        float rotation = service.CalculateSunAngle();
        sun.transform.rotation = Quaternion.AngleAxis(rotation, Vector3.right);
    }
    void RotateMoon()
    {
        float rotation = -service.CalculateSunAngle();
        moon.transform.rotation = Quaternion.AngleAxis(rotation, Vector3.right);
    }
    
    void UpdateSkyBlend()
    {
        float dotProduct = Vector3.Dot(sun.transform.forward, Vector3.up);
        float blend = Mathf.Lerp(0, 1, lightIntensityCurve.Evaluate(dotProduct));
        skyboxMaterial.SetFloat("_Blend", blend);
    }
    
    void UpdateTimeOfDay()
    {
        service.UpdateTime(Time.deltaTime);
        if (timeText != null)
        {
            //timeText.text = service.CurrentTime.ToString("hh:mm");
            timeText.SetText(service.CurrentTime.ToShortTimeString()); // allocates gc
        }
    }
}
