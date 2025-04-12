using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Observer<T>
{
    [SerializeField] T value;
    [SerializeField] UnityEvent<T> onValueChanged;

    public T Value { // Allocates GC for some reason
        get => value;
        set => Set(value);
    }

    public Observer(T value, UnityAction<T> callback = null)
    {
        this.value = value;
        onValueChanged = new UnityEvent<T>();
        if (callback != null) onValueChanged.AddListener(callback);
    }

    public void Set(T value)
    {
        if (Equals(this.value, value)) return;
        this.value = value;
        Invoke();
    }

    public void Invoke()
    {
        //Debug.Log($"Invoking {onValueChanged.GetPersistentEventCount()} listeners");
        onValueChanged.Invoke(value);
    }

    public void AddListener(UnityAction<T> callback)
    {
        if (callback == null) return;
        if (onValueChanged == null) onValueChanged = new UnityEvent<T>();

        onValueChanged.AddListener(callback);
    }

    public void RemoveListener(UnityAction<T> callback)
    {
        if (callback == null) return;
        if (onValueChanged == null) return;

        onValueChanged.RemoveListener(callback);
    }

    public void RemoveAllListeners()
    {
        if (onValueChanged == null) return;

        onValueChanged.RemoveAllListeners();
    }

    public void Dispose()
    {
        RemoveAllListeners();
        onValueChanged = null;
        value = default;
    }
}



public class ObserverTest<T>
{
    [SerializeField] T value;
    public Action<T> ValueChanged;

    public T Value {
        get => value;
        set => Set(value);
    }

    public ObserverTest(T value, Action<T> callback = null)
    {
        this.value = value;
        //ValueChanged = new EventHandler<T>(null, );
        //if (callback != null) ValueChanged.AddListener(callback);
        if (callback != null) ValueChanged += callback;
    }

    public void Set(T value)
    {
        if (Equals(this.value, value)) return;
        this.value = value;
        Invoke();
    }

    public void Invoke()
    {
        //Debug.Log($"Invoking {ValueChanged.GetInvocationList()} listeners");
        ValueChanged?.Invoke(value);
    }

    // public void AddListener(Action<T> callback)
    // {
    //     if (callback == null) return;
    //     if (ValueChanged == null) ValueChanged = new Action<T>();

    //     ValueChanged += callback;
    // }

    // public void RemoveListener(UnityAction<T> callback)
    // {
    //     if (callback == null) return;
    //     if (ValueChanged == null) return;

    //     ValueChanged.RemoveListener(callback);
    // }

    // public void RemoveAllListeners()
    // {
    //     if (ValueChanged == null) return;

    //     ValueChanged.RemoveAllListeners();
    // }

    public void Dispose()
    {
        //RemoveAllListeners();
        ValueChanged = null;
        value = default;
    }
}