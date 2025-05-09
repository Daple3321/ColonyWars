using UnityEngine;
using UnityEngine.InputSystem;

public class CrosshairManager : MonoBehaviour
{
    public enum CrosshairType
    {
        Aim,
        Gather,
        Build,
        Interact,
    }
    
    public CrosshairType type;
    
    public Crosshair aimCrosshair;
    public Crosshair gatherCrosshair;
    public Crosshair buildCrosshair;
    public Crosshair interactCrosshair;
    
    public Texture2D customCursor;
    
    public static bool customCursorActive = false;
    [SerializeField] private RectTransform crosshairHolder;
    [SerializeField] private Canvas crosshairCanvas;
    
    public static CrosshairManager i {get; private set;}
    void Awake()
    {
        if (i != null){
            Destroy(this);
        }
        else{
            i = this;
        }
    }
    
    public static void ShowStatic() { i.Show(); }
    private void Show() { crosshairHolder.gameObject.SetActive(true); }
    public static void HideStatic() { i.Hide(); }
    private void Hide() { crosshairHolder.gameObject.SetActive(false); }
    
    public static void SwitchCrosshair(bool state){
        if(!state){
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            HideStatic();
            customCursorActive = false;
            //i.gameObject.SetActive(false);
        }
        else{
            //i.gameObject.SetActive(true);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            ShowStatic();
            customCursorActive = true;
        }
    }
    
    private void SetCrosshair(CrosshairType type)
    {
        
    }

    public void Init()
    {
        aimCrosshair.Init();
        gatherCrosshair.Init();
        Hide();
        
        Cursor.SetCursor(customCursor, new Vector2(36, 36), CursorMode.Auto);
        
        EventBus.i.OnInteractivePanelOpened += ()=>{
            if(customCursorActive){
                SwitchCrosshair(false);
            }
        };
        
        EventBus.i.OnInteractivePanelClosed += ()=>{
            if(!customCursorActive){
                SwitchCrosshair(true);
            }
        };
    }

    void Update()
    {
        // Vector2 position;
        // RectTransformUtility.ScreenPointToLocalPointInRectangle(
        //     (RectTransform)crosshairCanvas.transform,
        //     Input.mousePosition,
        //     crosshairCanvas.worldCamera,
        //     out position
        // );
        // crosshairHolder.position = crosshairCanvas.transform.TransformPoint(position);
    }
}
