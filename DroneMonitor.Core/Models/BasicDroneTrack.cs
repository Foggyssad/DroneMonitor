using DroneMonitor.Core.Interfaces;

namespace DroneMonitor.Core.Models
{
    public sealed class BasicDroneTrack : DroneTrack, IComparable<BasicDroneTrack>, IAlertable
    {
        private static readonly TimeSpan MaxAllowedGap;
        private static readonly int StaticAltWindow;
        private int _duplicateCount = 0;

        static BasicDroneTrack()
        {
            MaxAllowedGap = TimeSpan.FromMilliseconds(300);
            StaticAltWindow = 20;
        }

        public DroneStatusFlags Status { get; private set; } = DroneStatusFlags.None;

        public List<string> Alerts { get; } = new List<string>();

        public bool HasAlerts
        {
            get { return Alerts.Count > 0; }
        }

        public int DuplicateMessages
        {
            get { return _duplicateCount; }
        }

        public BasicDroneTrack(string key) : base(key) { }

        public int CompareTo(BasicDroneTrack? other)
        {
            if (other == null) return 1;
            return this.TotalMessages.CompareTo(other.TotalMessages);
        }

        protected override void OnMessageAdded(DroneMessage message)
        {
            if (message is null)
                return;
            
            DetectDuplicate(message);
            DetectGap();
            DetectStaticAltitude();
        }

        private void DetectDuplicate(DroneMessage message)
        {
            if (_messages.Count < 2)
                return;
            
            DroneMessage last = _messages[_messages.Count - 1];
            DroneMessage prev = _messages[_messages.Count - 2];

            if (last.Equals(prev))
            {
                _duplicateCount++;
            }
        }

        private void DetectGap()
        {
            if (_messages.Count < 2)
                return;

            DroneMessage prev = _messages[_messages.Count - 2];
            DroneMessage last = _messages[_messages.Count - 1];

            TimeSpan dt = last.Timestamp - prev.Timestamp;

            switch (dt)
            {
                case TimeSpan gap when gap > MaxAllowedGap:
                    Status |= DroneStatusFlags.HasGaps;
                    AddAlertOnce("Message gaps detected");
                    break;
                default:
                    break;
            }
        }

        private void DetectStaticAltitude()
        {
            if (_messages.Count < StaticAltWindow)
                return;
            List<double> window = new List<double>();

            int startIndex = _messages.Count - StaticAltWindow;

            for (int i = startIndex; i < _messages.Count; i++)
            {
                double? alt = _messages[i].PressureAltitudeFeet;

                if (alt.HasValue)
                {
                    window.Add(alt.Value);
                }
            }

            if (window.Count < 2)
            {
                return;
            }

            window.Sort((a, b) => a.CompareTo(b));

            double min = window[0];
            double max = window[window.Count - 1];

            if (Math.Abs(max - min) < 0.5)
            {
                Status |= DroneStatusFlags.StaticAlt;
                AddAlertOnce("Altitude appears constant");
            }

        }

        private void AddAlertOnce(string text)
        {
            if (!Alerts.Contains(text))
                Alerts.Add(text);
        }

        public static BasicDroneTrack operator +(BasicDroneTrack? a, BasicDroneTrack? b)
        {
            if (a is null && b is null)
                throw new ArgumentNullException("Both tracks are null.");

            if (a is null) return b!;
            if (b is null) return a;

            BasicDroneTrack merged = new BasicDroneTrack(a.Key);

            foreach (DroneMessage m in a.Messages)
                merged.Add(m);
            foreach (DroneMessage m in b.Messages)
                merged.Add(m);

            return merged;
        }

    }
}
