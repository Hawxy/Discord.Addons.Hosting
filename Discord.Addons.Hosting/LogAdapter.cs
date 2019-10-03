using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Discord.Addons.Hosting
{
    internal class LogAdapter<T> where T: class
    {
        private readonly ILogger _logger;
        private readonly Func<LogMessage, Exception, string> _formatter;
       
        public LogAdapter(ILoggerFactory loggerFactory, Func<LogMessage, Exception, string> formatter = null)
        {
            _logger = loggerFactory.CreateLogger(typeof(T));
            _formatter = formatter ?? DefaultFormatter;
        }
        
        public Task Log(LogMessage message)
        {
            _logger.Log(GetLogLevel(message.Severity), default, message, message.Exception, _formatter);
            return Task.CompletedTask;
        }

        private string DefaultFormatter(LogMessage message, Exception _)
            => $"{message.Source}: {message.Message}";

        private static LogLevel GetLogLevel(LogSeverity severity)
            => (LogLevel) Math.Abs((int) severity - 5);
    }
}