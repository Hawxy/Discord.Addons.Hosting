#region License
/*
   Copyright 2019 Hawxy

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
    public class DiscordHostConfiguration
    {
        public readonly string Token;
        public readonly Func<LogMessage, Exception, string> LogFormat;
        public readonly DiscordSocketConfig SocketConfig;
        internal DiscordHostConfiguration(string token, Func<LogMessage, Exception, string> logFormat, DiscordSocketConfig socketConfig)
        {
            Token = token;
            LogFormat = logFormat;
            SocketConfig = socketConfig;
        }
     
    }

    //This builder is a tad overkill but it provides a bit of additional flexibility and might come in handy in a future release
    public class DiscordHostConfigurationBuilder : IDiscordHostConfigurationBuilder
    {
        private string _token = string.Empty;
        private Func<LogMessage, Exception, string> _logFormat = (message, exception) => $"{message.Source}: {message.Message}";
        private DiscordSocketConfig _socketConfig = new DiscordSocketConfig();

        public IDiscordHostConfigurationBuilder SetDiscordConfiguration(DiscordSocketConfig config)
        {
            _socketConfig = config;
            return this;
        }

        public IDiscordHostConfigurationBuilder SetToken(string token)
        {
            TokenUtils.ValidateToken(TokenType.Bot, token);
           _token = token;
            return this;
        }

        public IDiscordHostConfigurationBuilder SetCustomLogFormat(Func<LogMessage, Exception, string> formatter)
        {
            _logFormat = formatter;
            return this;
        }

        public DiscordHostConfiguration Build() => new DiscordHostConfiguration(_token, _logFormat, _socketConfig);

    }

    public interface IDiscordHostConfigurationBuilder
    {
        IDiscordHostConfigurationBuilder SetDiscordConfiguration(DiscordSocketConfig config);
        IDiscordHostConfigurationBuilder SetToken(string token);
        IDiscordHostConfigurationBuilder SetCustomLogFormat(Func<LogMessage, Exception, string> formatter);
        DiscordHostConfiguration Build();
    }
}