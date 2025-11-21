namespace DroneMonitor.Core.Models;

[Flags]
public enum DroneStatusFlags
{
    None = 0b0,
    HasGaps = 0b1,
    StaticAlt = 0b10
}