using System;
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
        private readonly float _minDelaySeconds = 1f;
        private readonly float _maxDelaySeconds = 3f;
        
        private readonly bool _constantDelay = false;
        
        private readonly Random _random;

        public BotPlayerController()
        {
            _random = new Random();
        }

        public BotPlayerController(int seed)
        {
            _random = new Random(seed);
        }

        public BotPlayerController(float minDelaySeconds, float maxDelaySeconds)
        {
            _minDelaySeconds = minDelaySeconds;
            _maxDelaySeconds = maxDelaySeconds;
            _random = new Random();
        }

        public BotPlayerController(float constantDelaySeconds)
        {
            _constantDelay = true;
            _minDelaySeconds = constantDelaySeconds;
            _maxDelaySeconds = constantDelaySeconds;
        }
        
        public async Task RequestDrawAsync(CancellationToken cancellationToken = default)
        {
            // Generate a random delay between MinDelaySeconds and MaxDelaySeconds
            var delaySeconds = _minDelaySeconds;
            
            if (!_constantDelay)
            {
                delaySeconds = (float)(_random.NextDouble() * (_maxDelaySeconds - _minDelaySeconds) + _minDelaySeconds);    
            }
            
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
