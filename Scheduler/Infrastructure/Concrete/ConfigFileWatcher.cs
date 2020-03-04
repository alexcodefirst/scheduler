using NLog;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Scheduler.Infrastructure.Concrete
{
    public class ConfigFileWatcher
    {
        private readonly ManualResetEvent _stopEvent = new ManualResetEvent(false);
        private readonly ILogger _logger;
        private readonly object _locker = new object();
        private bool _alreadyTaskedToExit = false;

        public ConfigFileWatcher(ILogger logger)
        {
            this._logger = logger;
        }

        private void Watcher()
        {
            var watcher = new FileSystemWatcher(AppDomain.CurrentDomain.BaseDirectory)
            {
                Filter = "*.config",
                NotifyFilter = NotifyFilters.LastWrite
            };

            watcher.Changed += (sender, args) =>
            {
                lock (_locker)
                {
                    if (_alreadyTaskedToExit) return;

                    _alreadyTaskedToExit = true;

                    _logger.Info("The config file was changed, restarting service...");

                    Environment.Exit(1);
                }
            };
            watcher.EnableRaisingEvents = true;

            _stopEvent.WaitOne();
        }

        public void Start()
        {
            if (!_stopEvent.WaitOne(0))
            {
                Task.Factory.StartNew(Watcher);

                _logger.Info("The config file watcher was started");
            }
        }

        public void Stop()
        {
            if (!_stopEvent.WaitOne(0))
            {
                _stopEvent.Set();

                _logger.Info("The config file watcher was stopped");
            }
        }
    }
}