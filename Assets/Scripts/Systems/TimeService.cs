using System;
using UnityEngine;

public class TimeService
{
    readonly TimeSettings settings;
    DateTime currentTime;
    readonly TimeSpan sunriseTime;
    readonly TimeSpan sunsetTime;
    
    //public event Action OnSunrise = delegate { };
    //public event Action OnSunset = delegate { };
    //public event Action<int> OnHourChange = delegate { };

    readonly Observer<bool> isDayTime;
    readonly Observer<int> currentHour;
    
    readonly ObserverTest<int> currentHourTest;

    public TimeService(TimeSettings settings)
    {
        this.settings = settings;
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(settings.startHour);
        sunriseTime = TimeSpan.FromHours(settings.sunriseHour);
        sunsetTime = TimeSpan.FromHours(settings.sunsetHour);

        isDayTime = new Observer<bool>(IsDayTime());
        currentHour = new Observer<int>(currentTime.Hour);
        
        currentHourTest = new ObserverTest<int>(currentTime.Hour);
        currentHourTest.ValueChanged += EventBus.i.OnHourChange.Invoke;
        //currentHourTest.ValueChanged += x => Debug.Log($"Hour changed {x}");

        //currentHour.AddListener(EventBus.i.OnHourChange.Invoke);
        isDayTime.AddListener(OnDayChange);
    }

    void OnDayChange(bool isDay)
    {
        if (isDay)
            EventBus.i.OnSunrise?.Invoke();
        else
            EventBus.i.OnSunset?.Invoke();
    }
    
    public void UpdateTime(float deltaTime)
    {
        currentTime = currentTime.AddSeconds(deltaTime * settings.timeMultiplier);
        isDayTime.Value = IsDayTime();
        currentHour.Value = currentTime.Hour;
        currentHourTest.Value = currentTime.Hour;
    }

    public float CalculateSunAngle()
    {
        bool isDay = IsDayTime();
        float startDegree = isDay ? 0 : 180;
        TimeSpan start = isDay ? sunriseTime : sunsetTime;
        TimeSpan end = isDay ? sunsetTime : sunriseTime;

        TimeSpan totalTime = CalculateDifference(start, end);
        TimeSpan elapsedTime = CalculateDifference(start, currentTime.TimeOfDay);

        double percentage = elapsedTime.TotalMinutes / totalTime.TotalMinutes;
        return Mathf.Lerp(startDegree, startDegree + 180, (float)percentage);
    }

    public DateTime CurrentTime => currentTime;

    bool IsDayTime() => currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime;

    TimeSpan CalculateDifference(TimeSpan from, TimeSpan to)
    {
        TimeSpan difference = to - from;
        return difference.TotalHours < 0 ? difference + TimeSpan.FromHours(24) : difference;
    }
}
