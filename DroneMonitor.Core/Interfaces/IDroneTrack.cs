using DroneMonitor.Core.Models;

namespace DroneMonitor.Core.Interfaces;

public interface IDroneTrack
{
    string Key { get; }
    IReadOnlyList<DroneMessage> Messages { get; }

    int TotalMessages { get; }
    TimeSpan Duration { get; }

    void Add(DroneMessage message);
}