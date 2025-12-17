using DroneMonitor.App.Services;
using DroneMonitor.Core.Models;
using DroneMonitor.Analytics;
using DroneMonitor.Analytics.Exceptions;
using DroneMonitor.Analytics.Events;
using DroneMonitor.Analytics.Extensions;

namespace DroneMonitor.App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string path = (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
                            ? args[0]
                            : "/mnt/data/WiresharkDissectedIDs2.csv";

                DroneDataManager manager = new DroneDataManager();

                // --- Collect mode ---
                if (args.Length > 1 && args[1].Equals("collect", StringComparison.OrdinalIgnoreCase))
                {
                    DroneMessage msg = CollectOneMessageFromConsole();
                    manager.AppendMessage(path, msg);

                    Console.WriteLine("Collected 1 message and appended to CSV.");
                    return;
                }


                int loaded = manager.LoadFromCsv(path, skipHeader: true);
                Console.WriteLine("Loaded " + loaded + " messages from " + path);
                Console.WriteLine();

                List<BasicDroneTrack> tracks = manager.GetTracks(minMessages: 1, sortByDuration: false);

                tracks.Sort();


                // Generic MetricAccumulator<T>
                MetricAccumulator<BasicDroneTrack> acc = new MetricAccumulator<BasicDroneTrack>();

                for (int i = 0; i < tracks.Count; i++)
                {
                    acc.Add(tracks[i]);
                }

                Console.WriteLine("MetricAccumulator stored tracks (generic): " + acc.Count);
                Console.WriteLine();

                // IEnumerable / IEnumerator usage; iterator
                BasicDroneTrack biggest = tracks[0];
                for (int i = 1; i < tracks.Count; i++)
                {
                    if (tracks[i].TotalMessages > biggest.TotalMessages)
                    {
                        biggest = tracks[i];
                    }
                }

                int msgCount = 0;
                foreach (DroneMessage m in biggest)
                {
                    msgCount++;
                }

                Console.WriteLine("Foreach count (itertor) over the biggest track: " + msgCount);
                Console.WriteLine();

                // --- Event demo ---
                TrackEventHub hub = new TrackEventHub();

                int basicIdCount = 0;
                int locationCount = 0;

                hub.MessageObserved += (sender, msg) =>
                {
                    if (msg.MessageType == DroneMessageType.BasicId)
                    {
                        basicIdCount++;
                    }
                    else if (msg.MessageType == DroneMessageType.LocationVector)
                    {
                        locationCount++;
                    }
                };

                // Process one track and raise events
                hub.ProcessTrack(biggest);

                Console.WriteLine("Event demo:");
                Console.WriteLine("  Basic ID messages observed: " + basicIdCount);
                Console.WriteLine("  Location/Vector messages observed: " + locationCount);
                Console.WriteLine();

                Console.WriteLine("Extension method demo:");
                Console.WriteLine("  Message rate (via extension): "
                                  + biggest.MessageRate().ToString("F1") + " msgs/s");
                Console.WriteLine();



                if (tracks.Count >= 2)
                {
                    BasicDroneTrack merged = tracks[0] + tracks[1];
                    Console.WriteLine("Merged track messages: " + merged.TotalMessages);
                    Console.WriteLine();
                }

                ReportGenerator report = new ReportGenerator();
                report.PrintTracks(tracks);

                Console.WriteLine("Done.");
            }
            catch (DroneDataException e)
            {
                Console.WriteLine("DATA ERROR: " + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("UNEXPECTED ERROR: " + e.Message);
            }

        }

        private static DroneMessage CollectOneMessageFromConsole()
        {
            Console.Write("MAC (Source): ");
            string mac = Console.ReadLine() ?? "";

            Console.Write("Basic ID (optional): ");
            string id = Console.ReadLine() ?? "";

            Console.Write("Pressure Altitude feet (optional): ");
            string altStr = Console.ReadLine() ?? "";

            Console.Write("Message Type (Basic ID / Location/Vector / Operator ID / System): ");
            string typeStr = Console.ReadLine() ?? "";

            Console.Write("Message Counter (optional): ");
            string ctrStr = Console.ReadLine() ?? "";

            double? alt = null;
            if (double.TryParse(altStr, out double aTmp))
                alt = aTmp;

            int? ctr = null;
            if (int.TryParse(ctrStr, out int cTmp))
                ctr = cTmp;

            DroneMessageType type = DroneMessageType.Unknown;
            if (typeStr.Equals("Basic ID", StringComparison.OrdinalIgnoreCase))
                type = DroneMessageType.BasicId;
            else if (typeStr.Equals("Location/Vector", StringComparison.OrdinalIgnoreCase))
                type = DroneMessageType.LocationVector;
            else if (typeStr.Equals("Operator ID", StringComparison.OrdinalIgnoreCase))
                type = DroneMessageType.OperatorId;
            else if (typeStr.Equals("System", StringComparison.OrdinalIgnoreCase))
                type = DroneMessageType.System;

            DroneMessage msg = new DroneMessage(
                DateTime.UtcNow,
                mac,
                id,
                alt,
                type,
                ctr
            );

            return msg;
        }

    }
}
