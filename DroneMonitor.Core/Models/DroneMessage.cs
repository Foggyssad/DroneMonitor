using DroneMonitor.Core.Interfaces;

public enum DroneMessageType
{
    Unknown = 0,
    BasicId = 1,
    LocationVector = 2,
    System = 3,
    OperatorId = 4
}

namespace DroneMonitor.Core.Models
{
    public class DroneMessage : IDroneMessage, IEquatable<DroneMessage>, IFormattable
    {
        public DateTime Timestamp { get; set; }
        public string MacAddress { get; set; } = string.Empty;
        public string BasicId { get; set; } = string.Empty;
        public double? PressureAltitudeFeet { get; set; }
        public int? MessageCounter { get; set; }
        public DroneMessageType MessageType { get; set; } = DroneMessageType.Unknown;

        public DroneMessage() { }

        public DroneMessage(
            DateTime timestamp,
            string macAddress,
            string basicId,
            double? pressureAltitudeFeet,
            DroneMessageType messageType,
            int? messageCounter)
        {
            Timestamp = timestamp;
            MacAddress = macAddress;
            BasicId = basicId;
            PressureAltitudeFeet = pressureAltitudeFeet;
            MessageType = messageType;
            MessageCounter = messageCounter;
        }

        public void Deconstruct(out DateTime timestamp, out string macAddress, out int? messageCounter)
        {
            timestamp = Timestamp;
            macAddress = MacAddress;
            messageCounter = MessageCounter;
        }

        public bool Equals(DroneMessage? other)
        {
            if (other is null)
                return false;

            return string.Equals(MacAddress, other.MacAddress, StringComparison.OrdinalIgnoreCase)
                                 && MessageCounter == other.MessageCounter
                                 && MessageType == other.MessageType;
        }
        public override bool Equals(object? obj)
        {
            return Equals(obj as DroneMessage);
        }

        public override int GetHashCode()
        {
            string normMac = MacAddress?.ToUpperInvariant() ?? string.Empty;
            int counter = MessageCounter ?? 0;

            return HashCode.Combine(normMac, counter, MessageType);
        }

        public static bool operator ==(DroneMessage? a, DroneMessage? b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (a is null || b is null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(DroneMessage? a, DroneMessage? b)
        {
            return !(a == b);
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return $"[{Timestamp:HH:mm:ss}] MAC={MacAddress}, Count={MessageCounter}, Type={MessageType}";
        }

        public override string ToString()
        {
            return ToString(null, null);
        }
    }
}