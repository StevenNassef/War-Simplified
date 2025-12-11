using System.Threading;
using System.Threading.Tasks;
using Game.Core.Abstractions;

namespace Game.Client.Runtime
{
    /// <summary>
    /// Unity implementation of IPlayerController for bot/AI player.
    /// Completes draw requests automatically after a random delay (1-3 seconds).
    /// </summary>
    public class BotPlayerController : IPlayerController
    {
        private const float MinDelaySeconds = 1f;
        private const float MaxDelaySeconds = 3f;
        
        private readonly System.Random _random;

        public BotPlayerController()
        {
            _random = new System.Random();
        }

        public BotPlayerController(int seed)
        {
            _random = new System.Random(seed);
        }

        public async Task RequestDrawAsync(CancellationToken cancellationToken = default)
        {
            // Generate a random delay between MinDelaySeconds and MaxDelaySeconds
            var delaySeconds = (float)(_random.NextDouble() * (MaxDelaySeconds - MinDelaySeconds) + MinDelaySeconds);
            var delayMilliseconds = (int)(delaySeconds * 1000);

            try
            {
                await Task.Delay(delayMilliseconds, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                // Cancellation is expected and should be propagated
                throw;
            }
        }
    }
}
