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
    
    public Action<float, float> UpdateCallback;
    // сделать больше настроек (c текстом/без, цвет, эффекты, изменение цвета)
    public virtual void Init(Affiliation affiliation = Affiliation.None)
    {
        if(affiliation == Affiliation.Enemy){
            barImg.color = GameAssets.colors.enemyHealthbar;
        }
        else if(affiliation == Affiliation.Player){
            barImg.color = GameAssets.colors.friendlyHealthbar;
        }
        //rectTransform = GetComponent<RectTransform>();
    }

    public virtual void UpdateBar(float val, float maxVal)
    {
        bar.localScale = new Vector3(val / maxVal, 1, 1);

        if (barTxt != null)
        {
            barTxt.SetText($"{val:F1}/{maxVal:F0}");
        }
    }
}
