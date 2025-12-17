using DroneMonitor.Core.Models;
using DroneMonitor.Core.Utils;
using DroneMonitor.Analytics.Exceptions;

namespace DroneMonitor.App.Services
{
    public class DroneDataManager
    {
        private readonly Dictionary<string, BasicDroneTrack> _tracks = new Dictionary<string, BasicDroneTrack>();

        public IReadOnlyDictionary<string, BasicDroneTrack> Tracks
        {
            get { return _tracks; }
        }

        public int LoadFromCsv(string path, bool skipHeader = true)
        {
            try
            {
                if (!File.Exists(path))
                {
                    throw new DataCollectionException("CSV file not found: " + path);
                }

                int count = 0;
                int lineNo = 0;

                using (StreamReader sr = new StreamReader(path))
                {
                    string? line;
                    bool first = true;

                    while ((line = sr.ReadLine()) != null)
                    {
                        lineNo++;

                        if (first && skipHeader)
                        {
                            first = false;
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }

                        DroneMessage? msg;
                        bool ok = DroneMessageParser.TryParseCsvLine(line, out msg);

                        if (!ok || msg == null)
                        {
                            throw new CsvParseException(lineNo, line, "Invalid CSV row or unsupported format.");
                        }

                        AddMessage(msg);
                        count++;
                    }
                }

                return count;
            }
            catch (CsvParseException)
            {
                throw;
            }
            catch (IOException io)
            {
                throw new DataCollectionException("I/O error while reading CSV: " + path, io);
            }
            catch (UnauthorizedAccessException ua)
            {
                throw new DataCollectionException("Access denied while reading CSV: " + path, ua);
            }
        }


        public void AddMessage(DroneMessage msg)
        {
            if (msg == null)
                return;

            string key = !string.IsNullOrEmpty(msg.MacAddress)
                ? msg.MacAddress
                : msg.BasicId;

            if (!_tracks.ContainsKey(key))
            {
                _tracks[key] = new BasicDroneTrack(key);
            }

            _tracks[key].Add(msg);
        }

        public void AddMessages(params DroneMessage[] messages)
        {
            if (messages == null) return;
            foreach (DroneMessage m in messages)
                AddMessage(m);
        }

        public void AppendMessage(string path, DroneMessage msg)
        {
            bool fileExists = File.Exists(path);

            using (StreamWriter sw = new StreamWriter(path, append: true))
            {
                if (!fileExists)
                {
                    sw.WriteLine("\"Arrival Time\",\"Source\",\"ID\",\"UA Pressure Altitude\",\"Message Type\",\"Message Counter\"");
                }

                string ts = msg.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffffff+0000");

                string line =
                    "\"" + ts + "\"," +
                    "\"" + msg.MacAddress + "\"," +
                    "\"" + msg.BasicId + "\"," +
                    "\"" + (msg.PressureAltitudeFeet?.ToString() ?? "") + "\"," +
                    "\"" + msg.MessageType + "\"," +
                    "\"" + (msg.MessageCounter?.ToString() ?? "") + "\"";

                sw.WriteLine(line);
            }
        }

        public List<BasicDroneTrack> GetTracks(
            int minMessages = 0,
            bool sortByDuration = false)
        {
            List<BasicDroneTrack> list = new List<BasicDroneTrack>();

            foreach (var kv in _tracks)
            {
                if (kv.Value.TotalMessages >= minMessages)
                    list.Add(kv.Value);
            }

            if (sortByDuration)
                list.Sort((a, b) => a.Duration.CompareTo(b.Duration));

            return list;
        }
    }
}