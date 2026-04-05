using System;
using System.Timers;
using StudyTimerApp.Core.Interfaces;
using Timer = System.Timers.Timer;

namespace StudyTimerApp.Application.Services;

public class TimerService : ITimerService, IDisposable
{
    private readonly Timer _timer;
    private int _initialSeconds;
    
    public int RemainingSeconds { get; private set; }
    public bool IsRunning { get; private set; }

    public event Action<int>? TimeChanged;
    public event Action? TimerFinished;

    public TimerService()
    {
        _timer = new Timer(1000);
        _timer.Elapsed += OnTimerElapsed;
    }

    public void SetTime(int seconds)
    {
        if (IsRunning) return;
        
        _initialSeconds = seconds;
        RemainingSeconds = seconds;
        TimeChanged?.Invoke(RemainingSeconds);
    }

    public void Start()
    {
        if (RemainingSeconds <= 0) return;
        
        IsRunning = true;
        _timer.Start();
    }

    public void Stop()
    {
        IsRunning = false;
        _timer.Stop();
    }

    public void Reset()
    {
        Stop();
        RemainingSeconds = _initialSeconds;
        TimeChanged?.Invoke(RemainingSeconds);
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        if (RemainingSeconds > 0)
        {
            RemainingSeconds--;
            TimeChanged?.Invoke(RemainingSeconds);
        }
        
        if (RemainingSeconds <= 0)
        {
            Stop();
            TimerFinished?.Invoke();
        }
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}
