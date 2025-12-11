using System.Threading;
using System.Threading.Tasks;
using Game.Client.Infrastructure;
using Game.Client.Utilities;
using Game.Core.Abstractions;
using Game.Core.Application;
using Game.Core.Application.GameModes;
using Game.Core.Model;
using UnityEngine;
using ILogger = Game.Core.Abstractions.ILogger;

namespace Game.Client.Runtime
{
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Game Player")] [SerializeField]
        private GameView gameView;
        
        [SerializeField] private LocalPlayer[] players;
        [SerializeField] private LocalPlayerController mainPlayerController;

        private IGameController _gameController;
        private CancellationTokenSource _cancellationTokenSource;
        private GameState _gameState;

        private ICardApiClient _apiClient;
        private ILogger _logger;

        private IPlayerController _botPlayerController;
        private IGameMode _gameMode;

        private void Start()
        {
            _logger = new UnityLogger();
            _apiClient = new DeckOfCardsApiClient();
            _botPlayerController = new BotPlayerController(3f);
            _gameMode = new SimpleWasGameMode();
        }

        public void InitializeGame()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _gameState = new GameState();

            var playersList = new IPlayer[players.Length];
            players.CopyTo(playersList, 0);

            gameView.Initialize(playersList);
            _gameController = new GameController(new DeckProviderService(_apiClient), playersList,
                new[] { mainPlayerController, _botPlayerController }, gameView, _gameMode, _gameState, _logger);
            PlayGame().GetAwaiter();
        }

        public async Task PlayGame()
        {
            await _gameController.StartGameAsync(_cancellationTokenSource.Token);
            while (_gameState.Phase != GamePhase.Finished)
            {
                await _gameController.PlayRoundAsync(_cancellationTokenSource.Token);
            }
        }
    }
}