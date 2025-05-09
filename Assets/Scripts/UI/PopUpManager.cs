using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;

public class PopUpManager : MonoBehaviour
{
    public static PopUpManager i;
    void Awake()
    {
        if (i != null){
            Destroy(this);
        }
        else{
            i = this;
        }
    }
    
    private GameObject popUpPrefab;
    
    [SerializeField]
    private List<RectTransform> texts;
    
    private Camera mainCam;
    public void Init()
    {
        popUpPrefab = Resources.Load<GameObject>("UI/PopUpCanvas");
        texts = new List<RectTransform>();
        mainCam = Camera.main;
    }

    void Update()
    {
        UpdateTexts();
    }
    private void UpdateTexts()
    {
        for(int i = 0; i < texts.Count; i++)
        {
            if(texts[i] != null){
                Vector3 rotDir = mainCam.transform.position - texts[i].position;
                texts[i].rotation = Quaternion.LookRotation(-rotDir, Vector3.up);
            }
            else{
                texts.RemoveAt(i);
            }
        }
    }
    
    public TextMeshProUGUI Spawn(Vector3 pos, Color textColor)
    {
        GameObject go = PoolManager.Get(popUpPrefab);
        go.transform.position = pos;
        texts.Add(go.GetComponent<RectTransform>());
        
        TextMeshProUGUI txt = go.GetComponentInChildren<TextMeshProUGUI>();
        txt.color = textColor;
        txt.rectTransform.localPosition = Vector3.zero;
        
        Vector3 randPos = txt.rectTransform.position;
        randPos += new Vector3(Random.Range(-1f, 1f), Random.Range(1f, 3f), Random.Range(-1f, 1f));
        Sequence seq = Sequence.Create().OnComplete(()=> PoolManager.Release(go))
        .Group(Tween.PunchScale(txt.rectTransform, new Vector3(1.1f, 1.1f, 1.1f), duration: 0.35f, 7f, easeBetweenShakes: Ease.OutBack))
        .Group(Tween.Position(txt.rectTransform, randPos, duration: 0.4f, Ease.InOutBack))
        .Chain(Tween.Position(txt.rectTransform, randPos + new Vector3(0, -4, 0), duration: 0.5f, Ease.InOutBack))
        .Insert(0.7f, Tween.Color(txt, new Color(0,0,0,0), duration: 0.35f, Ease.InCirc));
        
        //Tween scaleTween = Tween.PunchScale(txt.rectTransform, new Vector3(1.1f, 1.1f, 1.1f), duration: 0.7f, 7f, easeBetweenShakes: Ease.OutBack);
        //scaleTween.OnComplete(()=> PoolManager.Release(go));
        
        return txt;
    }
}
