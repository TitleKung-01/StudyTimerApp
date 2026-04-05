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
    private string _newMediaUrl = "";

    public System.Collections.ObjectModel.ObservableCollection<TrackInfo> MusicQueue { get; } = new();

    [ObservableProperty]
    private TrackInfo? _currentTrack;

    [ObservableProperty]
    private bool _isLoadingMusic;

    [ObservableProperty]
    private double _volume = 0.5;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanPlayMusic))]
    [NotifyPropertyChangedFor(nameof(CanPauseMusic))]
    [NotifyPropertyChangedFor(nameof(HasTrackOrQueue))]
    private bool _isMusicPlaying;

    public bool CanEditSettings => !IsRunning;

    public bool CanPlayMusic => CurrentTrack != null && !IsMusicPlaying;
    public bool CanPauseMusic => CurrentTrack != null && IsMusicPlaying;
    public bool HasTrackOrQueue => CurrentTrack != null || MusicQueue.Count > 0;

    public MainViewModel(ITimerService timerService, IMediaPlayerService mediaPlayerService, IMusicResolverService musicResolverService)
    {
        _timerService = timerService;
        _mediaPlayerService = mediaPlayerService;
        _musicResolverService = musicResolverService;

        _timerService.TimeChanged += OnTimeChanged;
        _timerService.TimerFinished += OnTimerFinished;

        _timerService.SetTime(25 * 60);
    }

    partial void OnVolumeChanged(double value)
    {
        _mediaPlayerService.SetVolume(value);
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
        IsMusicPlaying = false;
        OnPropertyChanged(nameof(CanEditSettings));
    }

    [RelayCommand]
    private async Task StartAsync()
    {
        if (!_timerService.IsRunning && _timerService.RemainingSeconds > 0)
        {
            if (CurrentTrack == null && MusicQueue.Count > 0)
            {
                await PlayNextInQueueAsync();
            }
            else if (CurrentTrack != null && !IsMusicPlaying)
            {
                // Resume
                _mediaPlayerService.Resume(); 
                IsMusicPlaying = true;
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
        IsMusicPlaying = false;
    }

    [RelayCommand]
    private void Reset()
    {
        _timerService.Reset();
        IsRunning = false;
        _mediaPlayerService.Stop();
        IsMusicPlaying = false;
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

    [RelayCommand]
    private void TogglePlayPauseMusic()
    {
        if (CurrentTrack == null) return;

        if (IsMusicPlaying)
        {
            _mediaPlayerService.Pause();
            IsMusicPlaying = false;
        }
        else
        {
            _mediaPlayerService.Resume();
            IsMusicPlaying = true;
        }
    }

    [RelayCommand]
    private void PauseMusic()
    {
        if (CurrentTrack != null)
        {
            _mediaPlayerService.Pause();
            IsMusicPlaying = false;
        }
    }

    [RelayCommand]
    private void ResumeMusic()
    {
        if (CurrentTrack != null)
        {
            _mediaPlayerService.Resume();
            IsMusicPlaying = true;
        }
    }

    [RelayCommand]
    private void StopMusic()
    {
        _mediaPlayerService.Stop();
        CurrentTrack = null;
        IsMusicPlaying = false;
    }

    [RelayCommand]
    private async Task AddToQueueAsync()
    {
        if (!string.IsNullOrWhiteSpace(NewMediaUrl))
        {
            var url = NewMediaUrl;
            NewMediaUrl = "";

            IsLoadingMusic = true;
            try
            {
                var trackInfo = await _musicResolverService.ResolveTrackAsync(url);
                if (trackInfo != null)
                {
                    MusicQueue.Add(trackInfo);
                    
                    if (IsRunning && CurrentTrack == null)
                    {
                        await PlayNextInQueueAsync();
                    }
                }
            }
            finally
            {
                IsLoadingMusic = false;
            }
        }
    }

    [RelayCommand]
    private void RemoveFromQueue(TrackInfo track)
    {
        if (MusicQueue.Contains(track))
        {
            MusicQueue.Remove(track);
        }
    }

    [RelayCommand]
    private async Task SkipMusicAsync()
    {
        _mediaPlayerService.Stop();
        CurrentTrack = null;
        IsMusicPlaying = false;
        await PlayNextInQueueAsync();
    }

    [RelayCommand]
    private async Task TrackEndedAsync()
    {
        await SkipMusicAsync();
    }

    private async Task PlayNextInQueueAsync()
    {
        if (MusicQueue.Count > 0)
        {
            var nextTrack = MusicQueue[0];
            MusicQueue.RemoveAt(0);

            CurrentTrack = nextTrack;

            if (CurrentTrack != null)
            {
                _mediaPlayerService.Play(CurrentTrack.StreamUrl);
                _mediaPlayerService.SetVolume(Volume);
                IsMusicPlaying = true;
            }
        }
    }
}
