using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Notification : MonoBehaviour, IPointerClickHandler
{
    public Bar progressBar;
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI descText;
    
    public Image outline;
    public Image bg;
    
    private float timeLeft;
    private bool isOver = false;
    public float notificationTime;
    public void Init(string header, string content, float time = 7, Color outlineColor = default, Color headerColor = default)
    {
        
        
        headerText.text = header;
        descText.text = content;
        
        outline.color = outlineColor;
        headerText.color = headerColor;
        
        this.timeLeft = time;
        notificationTime = time;
    }
    
    public bool UpdateProgress()
    {
        if(timeLeft > 0){
            timeLeft -= Time.deltaTime;
            progressBar.UpdateBar(timeLeft, notificationTime);
            return false;
        }
        else{
            isOver = true;
            return true;
        }
    }
    public void MarkForDestroy(){
        isOver = true;
    }
    public bool IsOver(){
        return isOver;
    }
    
    public event Action<Notification> OnClick;
    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke(this);
    }
    
    
}
