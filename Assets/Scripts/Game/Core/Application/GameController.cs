using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Game.Core.Abstractions;
using Game.Core.Model;

namespace Game.Core.Application
{
    public class GameController : IGameController   
    {
        private readonly IDeckProviderService _deckProviderService;
        private readonly IPlayer[] _players;
        private readonly IPlayerController[] _playerControllers;
        private readonly IGameView _gameView;
        private readonly IGameMode _gameMode;
        private readonly GameState _gameState;
        private readonly ILogger _logger;

        private readonly int _playerCount;
        
        public GameController(IDeckProviderService deckProviderService, IPlayer[] players, IPlayerController[] playerControllers, IGameView gameView, IGameMode gameMode, GameState gameState, ILogger logger)
        {
            if (players.Length != playerControllers.Length)
                throw new System.ArgumentException("Number of players and player controllers must be equal");

            _deckProviderService = deckProviderService;
            _players = players;
            _playerControllers = playerControllers;
            _gameView = gameView;
            _gameMode = gameMode;
            _gameState = gameState;
            _playerCount = players.Length;
            _logger = logger;
        }
        
        public async Task StartGameAsync(CancellationToken cancellationToken = default)
        {
            if (_gameState.Phase == GamePhase.Finished)
            {
                _logger.LogWarning("[GameController] Game already finished");
                return;
            }
            
            _gameMode.ConfigureNewGame(_gameState, _players);
            await _deckProviderService.InitializeAsync(cancellationToken);
            _gameView.StartGame();
            _gameView.UpdateScores(_gameState.PlayerScores.ToList());
        }

        public async Task PlayRoundAsync(CancellationToken cancellationToken = default)
        {
            if (_gameState.Phase != GamePhase.Playing)
            {
                _logger.LogWarning("[GameController] Game not started yet");
                return;
            }

            if (_gameState.Phase == GamePhase.Finished)
            {
                _logger.LogWarning("[GameController] Game already finished");
                return;
            }
            
            var drawTask = _playerControllers.Select(c => c.RequestDrawAsync(cancellationToken));
            await Task.WhenAll(drawTask);
            cancellationToken.ThrowIfCancellationRequested();
            
            await PlayRoundInternalAsync(cancellationToken);
        }

        private async Task PlayRoundInternalAsync(CancellationToken cancellationToken)
        {
            var drawnCards = await _deckProviderService.DrawCardsAsync(_playerCount, cancellationToken);

            var roundWinnerIndex = -1;
            
            if (drawnCards == null)
            {
                // TODO: Handle error
                _logger.LogError("[GameController] Failed to draw cards");
                return;
            }
            
            roundWinnerIndex = _gameMode.EvaluateRoundOutcome(_gameState, _players, drawnCards);

            _gameView.ShowRoundResult(_gameState.CurrentRound, drawnCards, _gameState.PlayerScores.ToList(), roundWinnerIndex);
            _gameView.UpdateScores(_gameState.PlayerScores.ToList());

            CheckWinnerAndEndGame();
        }

        private void CheckWinnerAndEndGame(bool force = false)
        {
            if (_gameMode.TryResolveGameWinner(_gameState, _players, out var winnerIndex))
            {
                EndGame(winnerIndex);
            } 
            else if (force)
            {
                EndGame();
            }
        }

        private void EndGame(int winnerIndex = -1)
        {
            if (_gameState.Phase == GamePhase.Finished)
            {
                _logger.LogWarning("[GameController] Game already finished");
                return;
            }
            
            _gameState.Phase = GamePhase.Finished;
            _gameView.ShowGameOver(winnerIndex);
        }
    }
}
