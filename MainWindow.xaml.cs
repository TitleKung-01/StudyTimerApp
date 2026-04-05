using System;
using System.Windows;
using StudyTimerApp.Application.Services;
using StudyTimerApp.Infrastructure.Services;
using StudyTimerApp.Services;
using StudyTimerApp.ViewModels;

namespace StudyTimerApp;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        
        // Setup Dependency Injection manually for the scope of Clean Architecture sample
        var timerService = new TimerService();
        var mediaPlayerService = new WebViewMediaPlayerService(PlayMedia, StopMedia);
        var musicResolverService = new YoutubeMusicResolverService();
        
        _viewModel = new MainViewModel(timerService, mediaPlayerService, musicResolverService);
        DataContext = _viewModel;
    }

    private void PlayMedia(string url)
    {
        Dispatcher.Invoke(() =>
        {
            if (string.IsNullOrWhiteSpace(url)) return;

            try
            {
                PlayerMediaElement.Source = new Uri(url);
                PlayerMediaElement.Play();
            }
            catch
            {
                // Invalid URL handling
            }
        });
    }

    private void StopMedia()
    {
        Dispatcher.Invoke(() =>
        {
            try
            {
                PlayerMediaElement.Stop();
                PlayerMediaElement.Source = null;
            }
            catch { }
        });
    }
}