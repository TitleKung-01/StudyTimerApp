using System;
using StudyTimerApp.Core.Interfaces;

namespace StudyTimerApp.Services;

public class WebViewMediaPlayerService : IMediaPlayerService
{
    private readonly Action<string> _playAction;
    private readonly Action _stopAction;

    public WebViewMediaPlayerService(Action<string> playAction, Action stopAction)
    {
        _playAction = playAction;
        _stopAction = stopAction;
    }

    public void Play(string url)
    {
        _playAction(url);
    }

    public void Stop()
    {
        _stopAction();
    }
}
