using System.Threading.Tasks;

namespace StudyTimerApp.Core.Interfaces;

public interface IMusicResolverService
{
    Task<TrackInfo?> ResolveTrackAsync(string url);
}
