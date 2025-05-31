using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PrimeTween;
using TMPro;
using UnityEngine;

public class WorldUI : MonoBehaviour
{
    public static WorldUI i;
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
    private List<RectTransform> elements;
    
    public BuildingHoverPanel buildingHover;
    private Camera mainCam;
    public void Init()
    {
        popUpPrefab = Resources.Load<GameObject>("UI/PopUpCanvas");
        buildingHover = Instantiate(GameAssets.buildingHoverPanel).GetComponentInChildren<BuildingHoverPanel>();
        buildingHover.Hide(null);
        elements = new List<RectTransform>();
        mainCam = Camera.main;
        
        elements.Add(buildingHover.GetComponent<RectTransform>());
    }

    void Start()
    {
        //UniTask.RunOnThreadPool(UpdateTexts);
    }

    void Update()
    {
        UpdateTexts();
    }
    private void UpdateTexts()
    {
        for(int i = 0; i < elements.Count; i++)
        {
            if(elements[i] != null && elements[i].gameObject.activeInHierarchy){
                Vector3 rotDir = mainCam.transform.position - elements[i].position;
                elements[i].rotation = Quaternion.LookRotation(-rotDir, Vector3.up);
            }
            else if(elements[i] == null){
                elements.RemoveAt(i);
            }
        }
    }
    
    public WorldBar CreateWorldBar()
    {
        WorldBar bar = Instantiate(GameAssets.worldSpaceBar).GetComponentInChildren<WorldBar>();
        
        if (!elements.Contains(bar.rectTransform)){
            elements.Add(bar.rectTransform);
        }
        
        return bar;
    }
    
    public TextMeshProUGUI DamagePopup(Vector3 pos, Color textColor)
    {
        GameObject go = PoolManager.Get(popUpPrefab);
        go.transform.position = pos;
        
        TextMeshProUGUI txt = go.GetComponentInChildren<TextMeshProUGUI>();
        txt.color = textColor;
        txt.rectTransform.localPosition = Vector3.zero;

        if (!elements.Contains(txt.rectTransform)){
            elements.Add(txt.rectTransform);
        }
        
        Vector3 randPos = txt.rectTransform.position;
        randPos += new Vector3(Random.Range(-1f, 1f), Random.Range(1f, 3f), Random.Range(-1f, 1f));
        Sequence seq = Sequence.Create().OnComplete(()=> PoolManager.Release(go))
        .Group(Tween.PunchScale(txt.rectTransform, new Vector3(1.1f, 1.1f, 1.1f), duration: 0.35f, 7f, easeBetweenShakes: Ease.OutBack))
        .Group(Tween.Position(txt.rectTransform, randPos, duration: 0.4f, Ease.InOutBack))
        .Insert(0.7f, Tween.Color(txt, new Color(0,0,0,0), duration: 0.35f, Ease.InCirc));
        
        //Tween scaleTween = Tween.PunchScale(txt.rectTransform, new Vector3(1.1f, 1.1f, 1.1f), duration: 0.7f, 7f, easeBetweenShakes: Ease.OutBack);
        //scaleTween.OnComplete(()=> PoolManager.Release(go));
        
        return txt;
    }
    
    public TextMeshProUGUI SpawnPopup(Vector3 pos, Color textColor)
    {
        GameObject go = PoolManager.Get(popUpPrefab);
        go.transform.position = pos;
        
        TextMeshProUGUI txt = go.GetComponentInChildren<TextMeshProUGUI>();
        txt.color = textColor;
        txt.rectTransform.localPosition = Vector3.zero;

        if (!elements.Contains(txt.rectTransform)){
            elements.Add(txt.rectTransform);
        }
        
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
    public TextMeshProUGUI SpawnPopup(Vector3 pos, Color textColor, Vector3 punchScale, float time = 0.35f, float freq = 7f)
    {
        GameObject go = PoolManager.Get(popUpPrefab);
        go.transform.position = pos;
        //texts.Add(go.GetComponent<RectTransform>());
        
        TextMeshProUGUI txt = go.GetComponentInChildren<TextMeshProUGUI>();
        txt.color = textColor;
        txt.rectTransform.localPosition = Vector3.zero;
        
        if (!elements.Contains(txt.rectTransform)){
            elements.Add(txt.rectTransform);
        }
        
        Vector3 randPos = txt.rectTransform.position;
        randPos += new Vector3(Random.Range(-1f, 1f), Random.Range(0.7f, 2f), Random.Range(-1f, 1f));
        Sequence seq = Sequence.Create().OnComplete(()=> PoolManager.Release(go))
        .Group(Tween.PunchScale(txt.rectTransform, punchScale, duration: time, freq, easeBetweenShakes: Ease.OutBack))
        .Group(Tween.Position(txt.rectTransform, randPos, duration: time, Ease.InOutBack))
        .Chain(Tween.Position(txt.rectTransform, randPos + new Vector3(0, -4, 0), duration: time, Ease.InOutBack))
        .Insert(0.7f, Tween.Color(txt, new Color(0,0,0,0), duration: time, Ease.InCirc));
        
        //Tween scaleTween = Tween.PunchScale(txt.rectTransform, new Vector3(1.1f, 1.1f, 1.1f), duration: 0.7f, 7f, easeBetweenShakes: Ease.OutBack);
        //scaleTween.OnComplete(()=> PoolManager.Release(go));
        
        return txt;
    }
}
