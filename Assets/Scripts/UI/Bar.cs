using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    public Image bg;
    public RectTransform bar;
    public TextMeshProUGUI barTxt;
    
    public Action<float, float> UpdateCallback;

    public void Init(/*Action<float, float> updateCallback*/) // bullshit REF?*!
    {
        //UpdateCallback = updateCallback;
        //UpdateCallback += UpdateBar;
        //updateCallback += UpdateBar;
    }

    public void UpdateBar(float val, float maxVal)
    {
        bar.localScale = new Vector3(val / maxVal, 1, 1);

        if (barTxt != null)
        {
            barTxt.SetText($"{val:F1}/{maxVal:F0}");
        }
    }
}
