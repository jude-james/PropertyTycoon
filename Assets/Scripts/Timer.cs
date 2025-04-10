using System;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    public static bool HasEnded { get; private set; }
    
    private float _startTime;
    private float _remainingTime;
    private TimeSpan _timeSpan;
    
    private void Start()
    {
        _startTime = Menu.GetTime();
        if (_startTime == 0)
        {
            Destroy(gameObject);
        }
        
        _remainingTime = _startTime * 60;
    }

    private void Update()
    {
        if (_remainingTime > 0)
        {
            _remainingTime -= Time.deltaTime;
            _timeSpan = TimeSpan.FromSeconds(_remainingTime);
            var formattedTime = $"{(int)_timeSpan.TotalHours:D2}:{_timeSpan.Minutes:D2}:{_timeSpan.Seconds:D2}";
            text.SetText(formattedTime);
        }
        else
        {
            HasEnded = true;
        }
    }
}