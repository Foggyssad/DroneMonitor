using System;
using System.Collections;
using System.Collections.Generic;

namespace DroneMonitor.Core.Models
{
    public sealed class DroneMessageEnumerator : IEnumerator<DroneMessage>
    {
        private readonly IList<DroneMessage> _messages;
        private int _index;

        public DroneMessageEnumerator(IList<DroneMessage> messages)
        {
            _messages = messages;
            _index = -1;
        }

        public DroneMessage Current
        {
            get
            {
                if (_index < 0 || _index >= _messages.Count)
                    throw new InvalidOperationException("Enumerator is not on a valid element.");
                return _messages[_index];
            }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            _index++;
            return _index < _messages.Count;
        }

        public void Reset()
        {
            _index = -1;
        }

        public void Dispose()
        {
            // nothing to dispose
        }
    }
}