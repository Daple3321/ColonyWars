using PrimeTween;
using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    [SerializeField] private Canvas canvas;

    [SerializeField] public InventorySlot item;
    private RectTransform rectTransform;

    public float rotationMultiplier;
    public float rotSpeed;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = transform.root.GetComponent<Canvas>();
        item = GetComponentInChildren<InventorySlot>();
    }

    void Start()
    {
        //transform.SetAsFirstSibling();   
    }

    public void SetData(Sprite sprite, int quantity, InventoryItem itemToPut)
    {
        item.SetData(sprite, quantity, itemToPut);
    }

    void Update()
    {
        Vector3 mouseDelta = Input.mousePositionDelta;
        //Debug.Log(mouseDelta);
        float xRot = mouseDelta.x * rotationMultiplier;
        float yRot = mouseDelta.y * rotationMultiplier;
        Quaternion targetRot = Quaternion.Euler(yRot, xRot, 0);
        //rectTransform.rotation = Quaternion.Slerp(targetRot, Quaternion.identity, Time.deltaTime * rotSpeed);
        rectTransform.rotation = Quaternion.RotateTowards(rectTransform.rotation, targetRot, rotSpeed * Time.deltaTime);
        
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            Input.mousePosition,
            canvas.worldCamera,
            out position
        );
        transform.position = canvas.transform.TransformPoint(position);
    }

    public void Toggle(bool val)
    {
        //Debug.Log($"Item toggled {val}");
        gameObject.SetActive(val);

        // if (val == true)
        // {
        //     //Tween.LocalRotation(rectTransform, new Vector3(0, 0, 13), 0.35f, Ease.OutCubic);
        // }
        // else
        // {
        //     rectTransform.rotation = Quaternion.identity;
        // }
    }
}
