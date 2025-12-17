using System;
using DroneMonitor.Core.Models;

namespace DroneMonitor.Analytics.Events
{
    public class TrackEventHub
    {
        public event EventHandler<DroneMessage>? MessageObserved;

        public void ProcessTrack(BasicDroneTrack track)
        {
            if (track == null) return;

            foreach (DroneMessage m in track)
            {
                var handler = MessageObserved;
                if (handler != null)
                    handler(this, m);
            }
        }
    }
}