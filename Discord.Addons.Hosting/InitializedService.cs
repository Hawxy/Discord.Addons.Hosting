using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Addons.Hosting
{
    /// <summary>
    /// Base class for implementing an <see cref="IHostedService"/> with one-time setup requirements.
    /// </summary>
    public abstract class InitializedService : IHostedService
    {
        private bool _initialized;

        /// <summary>
        /// This method is called when the <see cref="IHostedService"/> starts for the first time.
        /// </summary>
        /// <param name="cancellationToken">Triggered when <see cref="IHostedService"/> is stopped during startup.</param>
        public abstract Task InitializeAsync(CancellationToken cancellationToken);
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (_initialized) return;
            await InitializeAsync(cancellationToken);
            if (!cancellationToken.IsCancellationRequested) _initialized = true;
        }

        public virtual Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask; 
    }
}
