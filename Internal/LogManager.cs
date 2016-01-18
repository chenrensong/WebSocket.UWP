
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebSocket4UWP.Internal
{
    using LogReceiver = Action<Type, LogLevel, string>;

    internal sealed class LogManager
    {
        public static readonly LogManager Instance = new LogManager();

        private readonly Dictionary<int, LogReceiver> _receivers;

        private LogManager()
        {
            _receivers = new Dictionary<int, LogReceiver>();
        }

        public void LogMessage(Type type, LogLevel logLevel, string message)
        {
            IList<LogReceiver> receivers;
            lock (_receivers)
            {
                receivers = _receivers.Values.ToList();
            }
            
            foreach (var receiver in receivers)
            {
                try
                {
                    receiver(type, logLevel, message);
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch (Exception)
                {
                    
                }
            }
        }

        public IDisposable AddReceiver(LogReceiver receiver)
        {
            if (receiver == null)
                throw new ArgumentNullException("receiver");

            lock (_receivers)
            {
                var guid = _receivers.Count + 1;
                _receivers.Add(guid, receiver);
                return new Reference(this, guid);
            }
        }

        private void Remove(int guid)
        {
            lock (_receivers)
                _receivers.Remove(guid);
        }

        struct Reference : IDisposable
        {
            private readonly LogManager _logManager;
            private int _guid;

            public Reference(LogManager logManager, int guid)
            {
                _logManager = logManager;
                _guid = guid;
            }

            public void Dispose()
            {
                if (_guid > 0)
                    _logManager.Remove(_guid);
                _guid = 0;
            }
        }
    }
}
