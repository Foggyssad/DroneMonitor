using System;
using System.Collections.Generic;
using DroneMonitor.Core.Interfaces;

namespace DroneMonitor.Analytics
{
    // Own generic type
    public class MetricAccumulator<T> where T : class, IDroneTrack
    {
        private readonly List<T> _items = new List<T>();

        public int Count
        {
            get { return _items.Count; }
        }

        public void Add(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _items.Add(item);
        }

        public IReadOnlyList<T> Items
        {
            get { return _items; }
        }
    }
}