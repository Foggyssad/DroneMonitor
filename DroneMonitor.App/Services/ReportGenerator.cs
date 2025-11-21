using DroneMonitor.Core.Models;

namespace DroneMonitor.App.Services
{
    public class ReportGenerator
    {
        public void PrintTracks(List<BasicDroneTrack> tracks)
        {
            if (tracks == null || tracks.Count == 0)
            {
                Console.WriteLine("No tracks to report.");
                return;
            }

            foreach (BasicDroneTrack track in tracks)
            {
                Console.WriteLine("Device key: " + track.Key);
                Console.WriteLine("  Messages: " + track.TotalMessages);
                Console.WriteLine("  Duration: " + track.Duration);
                Console.WriteLine("  Duplicates: " + track.DuplicateMessages);

                DroneMessage? lastMsg = track.Messages.Count > 0
                                     ? track.Messages[track.Messages.Count - 1]
                                     : null;
                
                double duration_s = track.Duration.TotalSeconds;
                double msgRate = (duration_s > 0.0)
                               ? (track.TotalMessages / duration_s)
                               : track.TotalMessages;

                Console.WriteLine("  Message rate: " + msgRate.ToString("F1") + " msgs/s");

                if (lastMsg is DroneMessage lm)
                {
                    DateTime ts;
                    string mac;
                    int? mc;
                    lm.Deconstruct(out ts, out mac, out mc);

                    Console.WriteLine("  Last: " + ts.ToString("O") + " MAC=" + mac + " MC=" + (mc ?? -1));
                    
                    if (!string.IsNullOrEmpty(lm.BasicId))
                    {
                        Console.WriteLine("  Basic ID: " + lm.BasicId);
                    }

                    Console.WriteLine("  Alt: " + (lm.PressureAltitudeFeet ?? double.NaN));
                    Console.WriteLine("  Type: " + lm.MessageType);

                }

                if (track.HasAlerts)
                {
                    Console.WriteLine("  Alerts:");
                    foreach (string a in track.Alerts)
                        Console.WriteLine("   - " + a);
                }

                Console.WriteLine();
            }
        }
    }
}