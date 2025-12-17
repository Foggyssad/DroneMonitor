using DroneMonitor.Core.Models;
using DroneMonitor.Analytics.Extensions;

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
                string bestId = track.BestBasicId;
                if (!string.IsNullOrEmpty(bestId))
                {
                    Console.WriteLine("  Basic ID (UAS): " + bestId);
                }

                if (lastMsg is DroneMessage lm)
                {
                    DateTime ts;
                    string mac;
                    int? mc;
                    lm.Deconstruct(out ts, out mac, out mc);

                    DroneMessage snapshot = (DroneMessage)lastMsg.Clone();

                    Console.WriteLine("  Last snapshot: " + snapshot.Timestamp.ToString("O")
                                      + " MAC=" + snapshot.MacAddress
                                      + " MC=" + (snapshot.MessageCounter ?? -1));

                    Console.WriteLine("  Last: " + ts.ToString("O") + " MAC=" + mac + " MC=" + (mc ?? -1));
                    
                    if (!string.IsNullOrEmpty(lm.BasicId))
                    {
                        Console.WriteLine("  Basic ID: " + lm.BasicId);
                    }

                    Console.WriteLine("  Alt: " + (lm.PressureAltitudeFeet ?? double.NaN));
                    Console.WriteLine("  Type: " + lm.MessageType);

                    string k;
                    int total;
                    TimeSpan dur;
                    track.Deconstruct(out k, out total, out dur);

                    Console.WriteLine("  Deconstruct(ext): Key=" + k + " Total=" + total + " Duration=" + dur);
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