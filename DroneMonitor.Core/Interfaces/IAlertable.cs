namespace DroneMonitor.Core.Interfaces;

public interface IAlertable
{
    bool HasAlerts { get; }
    List<string> Alerts { get; }
}