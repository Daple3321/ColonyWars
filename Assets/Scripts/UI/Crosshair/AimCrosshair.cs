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
        rectTransform.sizeDelta = new Vector2(concentraion, concentraion);
        Debug.Log("Updating crosshair");
    }

    void Update()
    {
        //UpdateCrosshair();
    }
}
