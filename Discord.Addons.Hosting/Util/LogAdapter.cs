#region License
/*
   Copyright 2021 Hawxy

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */
#endregion

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Discord.Addons.Hosting.Util
{
    internal class LogAdapter<T> where T: class
    {
        private readonly ILogger<T> _logger;
        private readonly Func<LogMessage, Exception, string> _formatter;
       
        public LogAdapter(ILogger<T> logger, IOptions<DiscordHostConfiguration> options)
        {
            _logger = logger;
            _formatter = options.Value.LogFormat;
        }
        
        public Task Log(LogMessage message)
        {
            _logger.Log(GetLogLevel(message.Severity), default, message, message.Exception, _formatter);
            return Task.CompletedTask;
        }

        private static LogLevel GetLogLevel(LogSeverity severity) 
            => severity switch
            {
                LogSeverity.Critical => LogLevel.Critical,
                LogSeverity.Error => LogLevel.Error,
                LogSeverity.Warning => LogLevel.Warning,
                LogSeverity.Info => LogLevel.Information,
                LogSeverity.Verbose => LogLevel.Debug,
                LogSeverity.Debug => LogLevel.Trace,
                _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
            };
    }
}