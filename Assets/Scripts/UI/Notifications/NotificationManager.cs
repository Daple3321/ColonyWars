using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IngameDebugConsole;
using Cysharp.Threading.Tasks;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager i { get; private set; }
    void Awake()
    {
        if (i != null){
            Destroy(this);
        }
        else{
            i = this;
        }
    }
    
    public Canvas notificationCanvas;
    public GameObject notificationPrefab;
    
    public RectTransform container;
    public VerticalLayoutGroup verticalLayout;
    
    public int maxNotifications = 5;
    public List<Notification> notifications;
    
    public void Init()
    {
        notifications = new();
        DebugLogConsole.AddCommandInstance( "notif", "Test notification", nameof(TestAdd), this );
    }
    private async void TestAdd(){
        await AddNotification("Test", "CONTENT", 8, Color.red, Color.white);
    }
    
    void Update()
    {
        if(notifications.Count > 0){
            HandleNotifications();
        }
    }

    private void HandleNotifications(){
        UpdateNotifications();
    }
    private void UpdateNotifications()
    {
        for(int i = 0; i < notifications.Count; i++)
        {
            notifications[i].UpdateProgress();
            
            if(notifications[i].IsOver()){
                Destroy(notifications[i].gameObject);
                notifications.RemoveAt(i);
            }
        }
    }
    
    public async void Add(string header, string content, float time = 7, Color outlineColor = default, Color headerColor = default, Sprite icon = null)
    {
        await AddNotification(header, content, time, outlineColor, headerColor, icon);
    }
    private async UniTask<Notification> AddNotification(string header, string content, float time = 7, Color outlineColor = default, Color headerColor = default, Sprite icon = null)
    {
        GameObject go = Instantiate(notificationPrefab);
        go.transform.SetParent(container);
        Notification n = go.GetComponent<Notification>();
        n.Init(header, content, time, outlineColor, headerColor, icon);
        notifications.Add(n);
        n.OnClick += OnNotificationClicked;
        
        verticalLayout.childControlHeight = true;
        verticalLayout.childForceExpandHeight = true;
        //await UniTask.Delay(50);
        await UniTask.WaitForEndOfFrame();
        verticalLayout.childControlHeight = false;
        verticalLayout.childForceExpandHeight = false;
        
        return n;
    }
    
    private void OnNotificationClicked(Notification n)
    {
        n.MarkForDestroy();
    }
}
