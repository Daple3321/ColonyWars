using UnityEngine;

public class AimCrosshair : Crosshair
{
    private RectTransform rectTransform;
    
    public override void Init()
    {
        base.Init();
        rectTransform = GetComponent<RectTransform>();
        EventBus.i.PlayerConcentrationChanged += UpdateCrosshair;
    }
    
    public void UpdateCrosshair(float concentraion)
    {
        float sizeDelta = Mathf.Lerp(15f, 60f, concentraion);
        rectTransform.sizeDelta = new Vector2(sizeDelta, sizeDelta);
    }
}
