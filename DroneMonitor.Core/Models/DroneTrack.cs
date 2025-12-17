using System.Collections;
using DroneMonitor.Core.Interfaces;

namespace DroneMonitor.Core.Models;

public abstract class DroneTrack : IDroneTrack, IEnumerable<DroneMessage>
{
    protected readonly List<DroneMessage> _messages = new List<DroneMessage>();

    public string Key { get; }

    protected DroneTrack(string key)
    {
        Key = key;
    }

    public IReadOnlyList<DroneMessage> Messages
    {
        get { return _messages; }
    }

    public int TotalMessages
    {
        get { return _messages.Count; }
    }

    public TimeSpan Duration
    {
        get
        {
            if (_messages.Count < 2)
            {
                return TimeSpan.Zero;
            }

            List<DroneMessage> ordered = new List<DroneMessage>(_messages);
            ordered.Sort((a, b) => a.Timestamp.CompareTo(b.Timestamp));

            DateTime t0 = ordered[0].Timestamp;
            DateTime t_last = ordered[^1].Timestamp;

            return t_last - t0;
        }
    }

    public void Add(DroneMessage message)
    {
        _messages.Add(message);
        OnMessageAdded(message);
    }

    protected abstract void OnMessageAdded(DroneMessage message);

    public IEnumerator<DroneMessage> GetEnumerator()
    {
        return new DroneMessageEnumerator(_messages);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}