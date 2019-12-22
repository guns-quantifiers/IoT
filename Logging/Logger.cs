using Core.Components;
using System;

namespace Logging
{
    public class Logger : ILogger
    {
        private readonly NLog.ILogger _logger;

        public Logger(NLog.ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Warning(string message)
        {
            _logger.Warn(message);
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }
    }
}
