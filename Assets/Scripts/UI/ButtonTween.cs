using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonTween : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    void Awake(){
        rectTransform = GetComponent<RectTransform>();
    }

    Tween scaleTween;
    public void OnPointerEnter(PointerEventData eventData)
    {
        scaleTween.Stop();
        scaleTween = Tween.Scale(rectTransform, 1.15f, 0.15f, Ease.OutCubic);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        scaleTween = Tween.Scale(rectTransform, 1, 0.15f, Ease.InCubic);
    }
}
