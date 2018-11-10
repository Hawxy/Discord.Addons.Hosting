using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// Based on ReliabilityService by Foxbot
namespace Discord.Addons.Hosting.Reliability
{
    internal class ReliableDiscordHost : IDisposable
    {
        private static readonly TimeSpan _timeout = TimeSpan.FromSeconds(30);

        private readonly DiscordSocketClient _discord;
        private readonly ILogger _logger;
        private readonly IHost _host;
        private CancellationTokenSource _cts;

        public ReliableDiscordHost(DiscordSocketClient discord, ILogger logger, IHost host)
        {
            _cts = new CancellationTokenSource();
            _discord = discord;
            _logger = logger;
            _host = host;
            _logger.LogInformation("Using Discord.Net Reliability service - Host will attempt to restart after a 30 second disconnect");

            _discord.Connected += ConnectedAsync;
            _discord.Disconnected += DisconnectedAsync;
        }

        private Task ConnectedAsync()
        {
            _logger.LogDebug("Discord client reconnected, resetting cancel token...");
            _cts.Cancel();
            _cts = new CancellationTokenSource();
            _logger.LogDebug("Discord client reconnected, cancel token reset.");

            return Task.CompletedTask;
        }

        private Task DisconnectedAsync(Exception _e)
        {
            _logger.LogInformation("Discord client disconnected, starting timeout task...");
            _ = Task.Delay(_timeout, _cts.Token).ContinueWith(async _ =>
            {
                _logger.LogDebug("Timeout expired, continuing to check client state...");
                await CheckStateAsync();
            });

            return Task.CompletedTask;
        }

        private async Task CheckStateAsync()
        {
            // Client reconnected, no need to reset
            if (_discord.ConnectionState == ConnectionState.Connected)
            {
                _logger.LogInformation("Discord client recovered");
                return;
            }

            _logger.LogCritical("Client did not reconnect in time, restarting host");
            await _host.StopAsync();
            await _host.StartAsync();
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _logger.LogInformation("Disposing Reliability Service");
                _discord.Connected -= ConnectedAsync;
                _discord.Disconnected -= DisconnectedAsync;
                _cts?.Cancel();
                _cts?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ReliableDiscordHost()
        {
            Dispose(false);
        }
    }
}