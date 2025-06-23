using TMPro;
using UnityEngine;

public class WorldBar : Bar
{
    public TextMeshProUGUI barDesc;
    
    public float maxDrawDistance;
    public Transform followTarget;
    public Transform parentTransform;
    public Vector3 offset;
    
    public override void Init(Affiliation affiliation = Affiliation.None, Gradient color = null)
    {
        base.Init(affiliation, color);
    }
    
    public void InitWorldBar(Transform target, Canvas parentCanvas, string barDescription = "")
    {
        followTarget = target;
        this.parentCanvas = parentCanvas;
        barDesc.text = barDescription;
    }
    
    public void SetDescription(string desc = "")
    {
        barDesc.text = desc;
    }
    
    public bool CheckDisctance(){
        if(Vector3.SqrMagnitude(followTarget.position - mainCam.transform.position) > maxDrawDistance*maxDrawDistance){
            return false;
        }
        else{
            return true;
        }
    }
    
    /*void Update()
    {
        //if(followTarget != null)
           //parentTransform.position = mainCam.WorldToScreenPoint(followTarget.position + offset);
        
        // if(Vector3.SqrMagnitude(followTarget.position - mainCam.transform.position) > maxDrawDistance*maxDrawDistance){
        //     barDesc.gameObject.SetActive(false);
        //     barImg.enabled = false;
        //     bg.enabled = false;
        // }
        // else{
        //     barDesc.gameObject.SetActive(true);
        //     barImg.enabled = true;
        //     bg.enabled = true;
        // }
        
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
    }*/
}
