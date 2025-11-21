namespace DroneMonitor.Core.Interfaces;

public interface IDroneMessage
{
    DateTime Timestamp { get; }
    string MacAddress { get; }
    string BasicId { get; }
    double? PressureAltitudeFeet { get; }
    int? MessageCounter { get; }
}