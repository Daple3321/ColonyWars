using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    public RectTransform rectTransform;
    public Image bg;
    public RectTransform bar;
    public Image barImg;
    public TextMeshProUGUI barTxt;
    protected Canvas parentCanvas;
    protected Camera mainCam;
    public Gradient barColor;
    
    public Action<float, float> UpdateCallback;
    
    // TO-DO: сделать больше настроек (c текстом/без)
    /// <summary>
    /// Если не указать affiliation - будет использоваться уже заданный цвет
    /// </summary>
    /// <param name="affiliation"></param>
    public virtual void Init(Affiliation affiliation = Affiliation.None, Gradient color = null)
    {
        mainCam = Camera.main;
        
        if(color != null){
            barColor = color;
            barImg.color = barColor.Evaluate(1);
        }

        if(affiliation == Affiliation.Enemy){
            barImg.color = GameAssets.colors.enemyHealthbar.Evaluate(0);
            barColor = GameAssets.colors.enemyHealthbar;
        }
        else if(affiliation == Affiliation.Player){
            barImg.color = GameAssets.colors.friendlyHealthbar.Evaluate(0);
            barColor = GameAssets.colors.friendlyHealthbar;
        }
        
        //rectTransform = GetComponent<RectTransform>();
    }

    public virtual void UpdateBar(float val, float maxVal)
    {
        float normalizedVal = val / maxVal;
        barImg.fillAmount = normalizedVal;
        
        if(barColor != null){
            barImg.color = barColor.Evaluate(normalizedVal);
        }

        if (barTxt != null)
        {
            barTxt.SetText($"{val:F1}/{maxVal:F0}");
        }
    }
}
