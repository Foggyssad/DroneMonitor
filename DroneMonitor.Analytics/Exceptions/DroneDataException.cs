using System;

namespace DroneMonitor.Analytics.Exceptions
{
    public class DroneDataException : Exception
    {
        public DroneDataException() { }
        public DroneDataException(string message) : base(message) { }
        public DroneDataException(string message, Exception inner) : base(message, inner) { }
    }

    public class CsvParseException : DroneDataException
    {
        public int LineNumber { get; }
        public string LineText { get; }

        public CsvParseException(int lineNumber, string lineText, string message)
            : base("CSV parse error at line " + lineNumber + ": " + message)
        {
            LineNumber = lineNumber;
            LineText = lineText;
        }
    }

    public class DataCollectionException : DroneDataException
    {
        public DataCollectionException(string message) : base(message) { }
        public DataCollectionException(string message, Exception inner) : base(message, inner) { }
    }
}