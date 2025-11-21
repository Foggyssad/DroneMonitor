// See https://aka.ms/new-console-template for more information
using DroneMonitor.App.Services;
using DroneMonitor.Core.Models;

namespace DroneMonitor.App
{
    internal class Program
    {
        static void Main(string[] args)
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
