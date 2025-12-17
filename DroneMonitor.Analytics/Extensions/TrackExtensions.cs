using System;
using DroneMonitor.Core.Models;
using DroneMonitor.Core.Interfaces;

namespace DroneMonitor.Analytics.Extensions
{
    public static class TrackExtensions
    {
        // Extended C# type: extension method on IDroneTrack
        public static double MessageRate(this IDroneTrack track)
        {
            if (track == null)
            {
                return 0.0;
            }

            double sec = track.Duration.TotalSeconds;
            if (sec <= 0.0)
            {
                return track.TotalMessages;
            }

            return track.TotalMessages / sec;
        }

        // Extension deconstructor (for assignment)
        // Usage: var (key, total, dur) = track;
        public static void Deconstruct(this BasicDroneTrack track,
                                       out string key,
                                       out int totalMessages,
                                       out TimeSpan duration)
        {
            key = track.Key;
            totalMessages = track.TotalMessages;
            duration = track.Duration;
        }
    }
}