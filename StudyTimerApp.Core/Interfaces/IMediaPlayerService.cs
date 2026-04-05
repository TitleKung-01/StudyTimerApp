namespace StudyTimerApp.Core.Interfaces;

public interface IMediaPlayerService
{
    void Play(string url);
    void Pause();
    void Resume();
    void Stop();
    void SetVolume(double volume);
}
