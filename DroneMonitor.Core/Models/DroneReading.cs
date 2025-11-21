namespace DroneMonitor.Core.Models
{
    public class DroneReading
    {
        public DateTime Timestamp { get; }
        public string MacAddress { get; }
        public int? MessageCounter { get; }
        public double? PressureAltitudeFeet { get; }

        public DroneReading(DroneMessage msg)
        {
            Timestamp = msg.Timestamp;
            MacAddress = msg.MacAddress;
            MessageCounter = msg.MessageCounter;
            PressureAltitudeFeet = msg.PressureAltitudeFeet;
        }

        public void Deconstruct(out DateTime ts, out string mac, out int? counter)
        {
            ts = Timestamp;
            mac = MacAddress;
            counter = MessageCounter;
        }
    }
}
