using System;
using StudyTimerApp.Core.Interfaces;

namespace StudyTimerApp.Services;

public class WebViewMediaPlayerService : IMediaPlayerService
{
    private readonly Action<string> _playAction;
    private readonly Action _pauseAction;
    private readonly Action _resumeAction;
    private readonly Action _stopAction;
    private readonly Action<double> _setVolumeAction;

    public WebViewMediaPlayerService(Action<string> playAction, Action pauseAction, Action resumeAction, Action stopAction, Action<double> setVolumeAction)
    {
        _playAction = playAction;
        _pauseAction = pauseAction;
        _resumeAction = resumeAction;
        _stopAction = stopAction;
        _setVolumeAction = setVolumeAction;
    }

    public void Play(string url)
    {
        _playAction(url);
    }

    public void Pause()
    {
        _pauseAction();
    }

    public void Resume()
    {
        _resumeAction();
    }

    public void Stop()
    {
        _stopAction();
    }

    public void SetVolume(double volume)
    {
        _setVolumeAction(volume);
    }
}
