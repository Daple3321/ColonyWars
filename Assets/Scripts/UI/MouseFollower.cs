using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    [SerializeField] private Canvas canvas;

    [SerializeField] public InventorySlot item;

    void Awake()
    {
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
    }
}
