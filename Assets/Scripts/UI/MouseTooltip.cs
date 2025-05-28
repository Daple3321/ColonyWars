using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PrimeTween;

public class MouseTooltip : MonoBehaviour
{
    public static MouseTooltip i{ get; private set; }
    
    [SerializeField]
    private Camera uiCamera;
    
    public TextMeshProUGUI tooltipHeader;
    private TextMeshProUGUI tooltipText;
    public CanvasGroup canvasGroup;
    [SerializeField] private RectTransform backgroundTransform;
    [SerializeField] private RectTransform canvasTransform;

    public Vector2 offset;
    public float textPaddingSize;

    private void Awake()
    {
        if (i != null)
        {
            Destroy(this);
        }
        else
        {
            i = this;
        }

        enabled = false;
        //backgroundTransform = transform.Find("background").GetComponent<RectTransform>();
        //tooltipHeader = transform.Find("Header_txt").GetComponent<TextMeshProUGUI>();
        //tooltipText = transform.Find("tooltip_txt").GetComponent<TextMeshProUGUI>();
        //parentTransform = transform.parent.GetComponent<RectTransform>();

        //ShowToolTip($"Speed 1", "Speed: 30 --> 35 \nlongsdgsodjsotjsdtoisjdto " + $"\n\n SOMEAST ASTSDASD");
        //HideTooltip();
    }

    public void Init()
    {
        //backgroundTransform = transform.Find("").GetComponent<RectTransform>();
        uiCamera = Camera.main;
        tooltipHeader = transform.Find("tooltip_header").GetComponent<TextMeshProUGUI>();
        tooltipText = transform.Find("tooltip_desc").GetComponent<TextMeshProUGUI>();
        HideTooltip();

        transform.SetAsLastSibling();
        
        EventBus.i.OnInteractivePanelClosed += HideTooltip;

        enabled = true;
    }
    
    void Update()
    {
        //Vector2 localPoint;
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(parentTransform, Input.mousePosition, uiCamera, out localPoint);
        //transform.localPosition = localPoint + offset;
        
        Vector3 pos = new Vector3(Input.mousePosition.x + offset.x, Input.mousePosition.y + offset.y, 0);
        if(pos.x + backgroundTransform.rect.width > canvasTransform.rect.width){
            pos.x = canvasTransform.rect.width - backgroundTransform.rect.width;
        }
        if(pos.y + backgroundTransform.rect.height > canvasTransform.rect.height){
            pos.y = canvasTransform.rect.height - backgroundTransform.rect.height;
        }

        transform.position = pos;
    }

    //Sequence seq;
    Tween alphaFade;
    private void ShowToolTip(string headerString, string tooltipString)
    {
        gameObject.SetActive(true);

        // seq.Stop();
        // seq = Sequence.Create()
        //.Group(Tween.Alpha(canvasGroup, 1f, 0.1f, Ease.InSine));
        //.Group(Tween.LocalPositionX(transform, 0, 0.15f, Ease.OutCubic));
        alphaFade.Stop();
        canvasGroup.alpha = 0f;
        alphaFade = Tween.Alpha(canvasGroup, 1f, 0.1f, Ease.InSine);


        tooltipHeader.text = headerString;
        tooltipText.text = tooltipString;

        // if(tooltipString.Contains('\n'))
        // {
        //     Vector2 textSize = new Vector2(tooltipText.preferredWidth, tooltipText.preferredHeight);
        //     tooltipText.rectTransform.sizeDelta = textSize;
        // }

        //Vector2 textSize = new Vector2(tooltipText.preferredWidth, tooltipText.preferredHeight);
        //tooltipText.rectTransform.sizeDelta = textSize;
        // Vector2 headerSize = new Vector2(tooltipHeader.preferredWidth, tooltipHeader.preferredHeight);
        // tooltipHeader.rectTransform.sizeDelta = headerSize;

        // float widthDiff = 0f;
        // if(tooltipHeader.preferredWidth > tooltipText.preferredWidth)
        // {
        //     widthDiff = tooltipHeader.preferredWidth - tooltipText.preferredWidth;
        // }
        //Vector2 backgroundSize = new Vector2(tooltipText.preferredWidth + textPaddingSize + widthDiff, tooltipText.preferredHeight + tooltipHeader.preferredHeight + textPaddingSize);
        //backgroundTransform.sizeDelta = backgroundSize;
    }
    public static void ShowTooltip_Static(string headerString, string tooltipString)
    {
        i.ShowToolTip(headerString, tooltipString);
    }
    public static void HideTooltip_Static()
    {
        i.HideTooltip();
    }
    
    private void HideTooltip()
    {
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }
}
