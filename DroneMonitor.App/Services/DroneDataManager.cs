using DroneMonitor.Core.Models;
using DroneMonitor.Core.Utils;

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
            if (!File.Exists(path))
                throw new FileNotFoundException(path);

            int count = 0;
            using (StreamReader sr = new StreamReader(path))
            {
                string? line;
                bool first = true;

                while ((line = sr.ReadLine()) != null)
                {
                    if (first && skipHeader)
                    {
                        first = false;
                        continue;
                    }

                    DroneMessage? msg;
                    if (DroneMessageParser.TryParseCsvLine(line, out msg) && msg != null)
                    {
                        AddMessage(msg);
                        count++;
                    }
                }
            }

            return count;
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