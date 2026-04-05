using System;

namespace StudyTimerApp.Core.Interfaces;

public interface ITimerService
{
    int RemainingSeconds { get; }
    bool IsRunning { get; }
    event Action<int>? TimeChanged;
    event Action? TimerFinished;

    void SetTime(int seconds);
    void Start();
    void Stop();
    void Reset();
}
