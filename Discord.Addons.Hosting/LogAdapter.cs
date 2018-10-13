#region License
//Based on Discord.Addons.MicrosoftLogging
/*Copyright 2017, foxbot <foxbot@protonmail.com>

   Permission to use, copy, modify, and/or distribute this software for any purpose with or without fee is hereby granted, 
   provided that the above copyright notice and this permission notice appear in all copies.
   
   THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS. 
   IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE,
   DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE. 
*/
#endregion
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Discord.Addons.Hosting
{
    public class LogAdapter
    {
        private readonly ILogger _logger;
        private readonly Func<LogMessage, Exception, string> _formatter;

        /// <summary>
        /// Creates a LogAdapter to be used with a Discord client
        /// </summary>
        /// <param name="logger">The logger implementation that messages will be written to</param>
        /// <param name="formatter">
        /// A custom message formatter, should the default be inadequate.
        /// </param>
        public LogAdapter(ILogger logger, Func<LogMessage, Exception, string> formatter = null)
        {
            _logger = logger;
            _formatter = formatter ?? DefaultFormatter;
        }

        /// <summary>
        /// Convert a Discord.Net log event to an abstract log event
        /// </summary>
        /// <param name="message">The log event to be converted</param>
        /// <returns>A task for compatibility with Discord.Net's async events</returns>
        public Task Log(LogMessage message)
        {
            _logger.Log(GetLogLevel(message.Severity), default(EventId), message, message.Exception, _formatter);
            return Task.Delay(0);
        }

        private string DefaultFormatter(LogMessage message, Exception _)
            => $"{message.Source}: {message.Exception?.ToString() ?? message.Message}";

        private static LogLevel GetLogLevel(LogSeverity severity)
            => (LogLevel) (Math.Abs((int) severity - 5));
    }
}