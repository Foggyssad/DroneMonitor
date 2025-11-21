using System.Globalization;
using DroneMonitor.Core.Models;

namespace DroneMonitor.Core.Utils
{
    public static class DroneMessageParser
    {
        // Expected CSV columns:
        // 0 Arrival Time
        // 1 Source (MAC)
        // 2 ID (Basic ID)
        // 3 UA Pressure Altitude
        // 4 Message Type (string)
        // 5 Message Counter
        public static bool TryParseCsvLine(string line, out DroneMessage? message)
        {
            message = null;

            if (string.IsNullOrWhiteSpace(line))
                return false;

            string[] tokens = SplitCsv(line);
            if (tokens.Length < 6)
                return false;

            if (tokens[0] == "Arrival Time")
                return false;

            // Format: 2025-11-09T23:09:43.582122000+0000
            DateTime ts;
            if (!TryParseWiresharkTime(tokens[0], out ts))
                return false;

            string mac = tokens[1].Trim();
            if (string.IsNullOrEmpty(mac))
                return false;

            string basicId = tokens[2].Trim();

            double? altFeet = null;
            double altTmp;
            if (double.TryParse(tokens[3], NumberStyles.Any, CultureInfo.InvariantCulture, out altTmp))
                altFeet = altTmp;

            DroneMessageType msgType = ParseType(tokens[4]);

            int? counter = null;
            int cTmp;
            if (int.TryParse(tokens[5], NumberStyles.Integer, CultureInfo.InvariantCulture, out cTmp))
                counter = cTmp;

            message = new DroneMessage(ts, mac, basicId, altFeet, msgType, counter);
            return true;
        }

        private static DroneMessageType ParseType(string s)
        {
            s = (s ?? "").Trim();

            switch (s)
            {
                case string x when x.Equals("Basic ID", StringComparison.OrdinalIgnoreCase):
                    return DroneMessageType.BasicId;

                case string x when x.Equals("Location/Vector", StringComparison.OrdinalIgnoreCase):
                    return DroneMessageType.LocationVector;

                case string x when x.Equals("Operator ID", StringComparison.OrdinalIgnoreCase):
                    return DroneMessageType.OperatorId;

                case string x when x.Equals("System", StringComparison.OrdinalIgnoreCase):
                    return DroneMessageType.System;

                default:
                    return DroneMessageType.Unknown;
            }
        }

        private static bool TryParseWiresharkTime(string s, out DateTime ts)
        {
            ts = default;
            s = s.Trim();

            // 2025-11-09T23:09:43.582122000+0000 -> 2025-11-09T23:09:43.582122000+00:00
            if (s.Length >= 5 && (s[^5] == '+' || s[^5] == '-'))
                s = s.Insert(s.Length - 2, ":");

            DateTimeOffset dateTimeOffset;
            if (!DateTimeOffset.TryParse(
                    s,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal,
                    out dateTimeOffset))
                return false;

            ts = dateTimeOffset.UtcDateTime;
            return true;
        }

        private static string[] SplitCsv(string line)
        {
            System.Collections.Generic.List<string> result =
            new System.Collections.Generic.List<string>();

            bool inQuotes = false;
            
            System.Text.StringBuilder cur =
            new System.Text.StringBuilder();

            foreach (char ch in line)
            {
                if (ch == '"')
                {
                    inQuotes = !inQuotes;
                    continue;
                }

                if (ch == ',' && !inQuotes)
                {
                    result.Add(cur.ToString());
                    cur.Clear();
                }
                else
                {
                    cur.Append(ch);
                }
            }

            result.Add(cur.ToString());
            return result.ToArray();
        }
    }
}