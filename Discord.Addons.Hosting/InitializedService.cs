using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Addons.Hosting
{
    public abstract class InitializedService : IHostedService
    {
        private bool _initialized;
        public abstract Task InitializeAsync(CancellationToken cancellationToken);
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (_initialized) return;
            var t = InitializeAsync(cancellationToken);
            await Task.WhenAny(t, Task.Delay(Timeout.Infinite, cancellationToken));
            if (!cancellationToken.IsCancellationRequested) _initialized = true;
        }

        public virtual Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask; 
    }
}
