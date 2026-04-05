using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudyTimerApp.Core.Interfaces;

namespace StudyTimerApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ITimerService _timerService;
    private readonly IMediaPlayerService _mediaPlayerService;
    private readonly IMusicResolverService _musicResolverService;

    [ObservableProperty]
    private string _timeDisplay = "00:00:00";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanEditSettings))]
    private bool _isRunning;

    [ObservableProperty]
    private string _mediaUrl = "";

    [ObservableProperty]
    private TrackInfo? _currentTrack;

    [ObservableProperty]
    private bool _isLoadingMusic;

    public bool CanEditSettings => !IsRunning;

    public MainViewModel(ITimerService timerService, IMediaPlayerService mediaPlayerService, IMusicResolverService musicResolverService)
    {
        _timerService = timerService;
        _mediaPlayerService = mediaPlayerService;
        _musicResolverService = musicResolverService;

        _timerService.TimeChanged += OnTimeChanged;
        _timerService.TimerFinished += OnTimerFinished;

        _timerService.SetTime(25 * 60);
    }

    private void OnTimeChanged(int remainingSeconds)
    {
        var timeSpan = TimeSpan.FromSeconds(remainingSeconds);
        TimeDisplay = timeSpan.ToString(@"hh\:mm\:ss");
    }

    private void OnTimerFinished()
    {
        IsRunning = false;
        _mediaPlayerService.Stop();
        OnPropertyChanged(nameof(CanEditSettings));
    }

    [RelayCommand]
    private async Task StartAsync()
    {
        if (!_timerService.IsRunning && _timerService.RemainingSeconds > 0)
        {
            if (!string.IsNullOrWhiteSpace(MediaUrl) && CurrentTrack == null)
            {
                IsLoadingMusic = true;
                CurrentTrack = await _musicResolverService.ResolveTrackAsync(MediaUrl);
                IsLoadingMusic = false;

                if (CurrentTrack != null)
                {
                    _mediaPlayerService.Play(CurrentTrack.StreamUrl);
                }
            }
            else if (CurrentTrack != null)
            {
                // Resume
                _mediaPlayerService.Play(CurrentTrack.StreamUrl); // Or just a play without url if MediaElement handles it, but passing URL is fine
            }
        }

        _timerService.Start();
        IsRunning = true;
    }

    [RelayCommand]
    private void Stop()
    {
        _timerService.Stop();
        IsRunning = false;
        _mediaPlayerService.Stop();
    }

    [RelayCommand]
    private void Reset()
    {
        _timerService.Reset();
        IsRunning = false;
        _mediaPlayerService.Stop();
        CurrentTrack = null;
    }

    [RelayCommand]
    private void SetTime(string minutesStr)
    {
        if (int.TryParse(minutesStr, out int minutes))
        {
            _timerService.SetTime(minutes * 60);
        }
    }
}
