using UnityEngine;

public class WorldBar : Bar
{
    public float maxDrawDistance;
    public Transform followTarget;
    public Vector3 offset;
    
    public override void Init(Affiliation affiliation = Affiliation.None)
    {
        base.Init(affiliation);
    }
    
    public void InitWorldBar(Transform target, Canvas parentCanvas)
    {
        followTarget = target;
        this.parentCanvas = parentCanvas;
    }
    
    void Update()
    {
        if(followTarget != null)
           transform.position = mainCam.WorldToScreenPoint(followTarget.position + offset);
        
        if(Vector3.SqrMagnitude(followTarget.position- mainCam.transform.position)> maxDrawDistance*maxDrawDistance){
            barImg.enabled = false;
            bg.enabled = false;
        }
        else{
            barImg.enabled = true;
            bg.enabled = true;
        }
        
        // if(followTarget != null){
        //     Vector3 output = Vector2.zero;
        //     RectTransformUtility.ScreenPointToWorldPointInRectangle(
        //         parentCanvas,
        //         followTarget.position,
        //         mainCam,
        //         out output);
            
        //     rectTransform.position = output;
        // }
        
        
        // Vector2 myPositionOnScreen = mainCam.WorldToScreenPoint(followTarget.position);
        // transform.position = myPositionOnScreen;
        
        // float scaleFactor = parentCanvas.scaleFactor;
        
        // Vector2 finalPosition = new Vector2 (myPositionOnScreen.x / scaleFactor , myPositionOnScreen.y / scaleFactor);
        // rectTransform.anchoredPosition = finalPosition;
        
        
        // Vector3 uiElementPosition = mainCam.WorldToScreenPoint(rectTransform.position);
        // Vector3 newWorldPosition = mainCam.ScreenToWorldPoint(uiElementPosition);
        // rectTransform.position = newWorldPosition;
    }
}
