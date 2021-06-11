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
using Discord.WebSocket;

namespace Discord.Addons.Hosting
{

    /// <summary>
    /// Configuration passed to the hosted service
    /// </summary>
    public class DiscordHostConfiguration
    {

        /// <summary>
        /// The bots token.
        /// </summary>
        public string Token { get; set; } = string.Empty;
        /// <summary>
        /// Sets a custom output format for logs coming from Discord.NET's integrated logger.
        /// </summary>
        /// <remarks>
        /// The default simply concatenates the message source with the log message.
        /// </remarks>
        public Func<LogMessage, Exception, string> LogFormat { get; set; } = (message, exception) => $"{message.Source}: {message.Message}";

        /// <inheritdoc cref="DiscordSocketConfig"/>
        public DiscordSocketConfig SocketConfig { get; set; } = new DiscordSocketConfig();
    }
}