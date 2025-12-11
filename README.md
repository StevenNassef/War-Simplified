# War-Simplified

A Unity-based card game implementation of a simplified version of the classic "War" card game. Players compete in rounds by drawing cards, with the highest card value winning each round. The game uses the [Deck of Cards API](https://deckofcardsapi.com/) to provide authentic card deck functionality.

## Table of Contents

- [Features](#features)
- [Requirements](#requirements)
- [Getting Started](#getting-started)
- [How to Play](#how-to-play)
- [Project Structure](#project-structure)
- [Architecture](#architecture)
- [API Integration](#api-integration)
- [Testing](#testing)
- [Development](#development)

## Features

- **Simplified War Card Game**: Fast-paced card game where players compete in rounds
- **Local vs Bot**: Play against an AI opponent
- **Real-time Card Drawing**: Uses the Deck of Cards API for authentic card deck management
- **Score Tracking**: Track scores across multiple rounds
- **Clean Architecture**: Separated Core and Client layers for maintainability
- **Unit Tests**: Comprehensive test coverage for game logic
- **Modern Unity UI**: Built with TextMeshPro and Unity UI system

## Requirements

- **Unity**: Version 2022.3 LTS or later
- **Internet Connection**: Required for Deck of Cards API calls
- **Platform**: Windows, macOS, or Linux (Unity Editor)

### Unity Packages

The project uses the following Unity packages (managed via Package Manager):
- Unity Input System (1.16.0)
- Universal Render Pipeline (17.3.0)
- TextMeshPro (via UGUI)
- Unity Test Framework (1.6.0)
- NSubstitute (for mocking in tests)

## Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd War-Simplified
```

### 2. Open in Unity

1. Open Unity Hub
2. Click "Add" and select the `War-Simplified` folder
3. Select the project and click "Open"
4. Wait for Unity to import all assets and compile scripts

### 3. Open the Game Scene

1. Navigate to the game scene in the Project window
2. Ensure the scene has a `GameBootstrap` component configured
3. Press Play to start the game

### 4. Configure Game Settings

In the Unity Inspector, configure the following on the `GameBootstrap` component:
- **Game View**: Assign the GameView component
- **Players**: Assign LocalPlayer components (typically 2 players)
- **Main Player Controller**: Assign the LocalPlayerController component

## How to Play

### Game Rules

1. **Setup**: The game starts with a shuffled deck of 52 cards
2. **Rounds**: Each round, all players draw one card
3. **Winning**: The player with the highest card value wins the round
   - Card values: 2-10 (face value), Jack, Queen, King, Ace (highest)
   - Suits don't matter for comparison
4. **Scoring**: The winner of each round earns points (default: 1 point per round)
5. **Game End**: The game ends after a set number of rounds (default: 8 rounds)
6. **Winner**: The player with the highest total score wins

### Controls

- **Draw Card**: Click the draw button when it's your turn
- **Restart**: Use the restart button after the game ends

## Project Structure

```
Assets/
├── Scripts/
│   └── Game/
│       ├── Client/              # Unity-specific implementations
│       │   ├── DTOs/            # API response data transfer objects
│       │   ├── Infrastructure/  # API client implementation
│       │   ├── Runtime/         # Unity MonoBehaviour components
│       │   └── Tests/           # Client layer tests
│       └── Core/                # Platform-agnostic game logic
│           ├── Abstractions/   # Interfaces and contracts
│           ├── Application/    # Game controllers and services
│           ├── Model/         # Domain models (Card, GameState, etc.)
│           └── Tests/         # Core logic unit tests
```

## Architecture

The project follows a clean architecture pattern with clear separation of concerns:

### Core Layer
- **Abstractions**: Defines interfaces for game controllers, views, players, and services
- **Application**: Contains game logic, controllers, and game modes
- **Model**: Domain models including `Card`, `GameState`, `PlayerState`

### Client Layer
- **Infrastructure**: Unity-specific implementations (API client, logging)
- **Runtime**: Unity MonoBehaviour components (GameView, GameBootstrap, controllers)
- **DTOs**: Data transfer objects for API communication

### Key Components

- **`GameController`**: Orchestrates game flow and round management
- **`GameView`**: Unity UI implementation for displaying game state
- **`DeckProviderService`**: Manages deck operations via API
- **`SimpleWasGameMode`**: Implements the simplified War game rules
- **`DeckOfCardsApiClient`**: Unity WebRequest-based API client

## API Integration

The game integrates with the [Deck of Cards API](https://deckofcardsapi.com/) for card deck management:

- **Create Deck**: Creates and shuffles a new deck
- **Draw Cards**: Draws cards from the deck
- **Reshuffle**: Reshuffles the deck when needed

### API Endpoints Used

- `GET /api/deck/new/shuffle/` - Create and shuffle a new deck
- `GET /api/deck/{deck_id}/draw/?count={count}` - Draw cards from deck
- `GET /api/deck/{deck_id}/shuffle/` - Reshuffle existing deck

## Testing

The project includes comprehensive unit tests using Unity Test Framework and NSubstitute for mocking.

### Running Tests

1. Open the **Test Runner** window: `Window > General > Test Runner`
2. Select **EditMode** or **PlayMode** tests
3. Click **Run All** to execute all tests

### Test Structure

- **Core Tests**: Unit tests for game logic, models, and controllers
- **Client Tests**: Integration tests for API client and Unity components

### Key Test Files

- `SimpleWasGameModeTests.cs` - Game mode logic tests
- `GameControllerTests.cs` - Game controller flow tests
- `CardTests.cs` - Card model validation tests
- `DeckOfCardsApiClientTests.cs` - API client tests

## Development

### Code Style

- Follow C# coding conventions
- Use meaningful names for classes, methods, and variables
- Add XML documentation comments for public APIs
- Keep methods focused and single-purpose

### Adding New Features

1. **New Game Mode**: Implement `IGameMode` interface
2. **New Player Type**: Implement `IPlayer` and `IPlayerController` interfaces
3. **New View**: Extend `GameViewBase` or implement `IGameView`

### Dependencies

- **Unity Input System**: For player input handling
- **TextMeshPro**: For UI text rendering
- **Unity Test Framework**: For unit testing
- **NSubstitute**: For test mocking

## Future Improvements

If given more time, the following improvements would enhance the project's architecture and performance:

### UniTask Integration

Currently, the project uses standard `Task` and `Task.Yield()` for async operations. Integrating [UniTask](https://github.com/Cysharp/UniTask) would provide better performance and Unity-specific async/await support.

#### Benefits
- Zero-allocation async operations
- Better integration with Unity's lifecycle
- Cancellation token support optimized for Unity
- Improved performance in hot paths

#### Implementation Example

**Before (Current Implementation):**
```csharp
protected override async Task StartGameInternal()
{
    await Task.Yield();
    gamePanel.SetActive(true);
    mainMenuPanel.SetActive(false);
}
```

**After (With UniTask):**
```csharp
using Cysharp.Threading.Tasks;

protected override async UniTask StartGameInternal()
{
    await UniTask.Yield();
    gamePanel.SetActive(true);
    mainMenuPanel.SetActive(false);
}
```

**API Client with UniTask:**
```csharp
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

public async UniTask<(string deckId, int remaining)> CreateAndShuffleNewDeckAsync(
    CancellationToken cancellationToken = default)
{
    var url = $"{BaseUrl}/new/shuffle/?deck_count=1";
    using var request = UnityWebRequest.Get(url);
    
    await request.SendWebRequest().ToUniTask(cancellationToken: cancellationToken);
    
    if (request.result != UnityWebRequest.Result.Success)
    {
        throw new Exception($"HTTP request failed: {request.error}");
    }
    
    var response = JsonUtility.FromJson<DeckResponse>(request.downloadHandler.text);
    return (response.deck_id, response.remaining);
}
```

### Dependency Injection

Currently, dependencies are manually constructed in `GameBootstrap`. Implementing a DI container (such as [Zenject](https://github.com/modesttree/Zenject) or [VContainer](https://github.com/hadashiA/VContainer)) would improve testability and maintainability.

#### Benefits
- Loose coupling between components
- Easier unit testing with mock injection
- Centralized dependency management
- Better support for lifecycle management

#### Implementation Example with VContainer

**1. Install VContainer via Package Manager:**
```
https://github.com/hadashiA/VContainer.git?path=VContainer/Assets/VContainer
```

**2. Create Lifetime Scopes:**

```csharp
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        // Register services
        builder.Register<ICardApiClient, DeckOfCardsApiClient>(Lifetime.Singleton);
        builder.Register<IDeckProviderService, DeckProviderService>(Lifetime.Singleton);
        builder.Register<ILogger, UnityLogger>(Lifetime.Singleton);
        
        // Register game mode
        builder.Register<IGameMode>(container => 
            new SimpleWasGameMode(initialMaxRounds: 8, pointsPerRound: 1), 
            Lifetime.Singleton);
        
        // Register controllers
        builder.Register<IPlayerController>(container => 
            new BotPlayerController(3f), 
            Lifetime.Transient)
            .AsImplementedInterfaces();
        
        // Register game controller
        builder.Register<IGameController, GameController>(Lifetime.Transient);
        
        // Register Unity components
        builder.RegisterComponentInHierarchy<GameView>();
        builder.RegisterComponentInHierarchy<GameBootstrap>();
    }
}
```

**3. Update GameBootstrap to use DI:**

```csharp
using VContainer;
using VContainer.Unity;

public class GameBootstrap : MonoBehaviour, IStartable
{
    [SerializeField] private LocalPlayer[] players;
    [SerializeField] private LocalPlayerController mainPlayerController;
    
    private IGameController _gameController;
    private GameView _gameView;
    private IGameMode _gameMode;
    private GameState _gameState;
    private CancellationTokenSource _cancellationTokenSource;
    
    [Inject]
    public void Construct(
        IGameController gameController,
        GameView gameView,
        IGameMode gameMode)
    {
        _gameController = gameController;
        _gameView = gameView;
        _gameMode = gameMode;
    }
    
    public void Start()
    {
        InitializeGame();
    }
    
    public void InitializeGame()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _gameState = new GameState();
        
        var playersList = new IPlayer[players.Length];
        players.CopyTo(playersList, 0);
        
        _gameView.Initialize(playersList);
        PlayGame().Forget();
    }
    
    private async UniTaskVoid PlayGame()
    {
        await _gameController.StartGameAsync(_cancellationTokenSource.Token);
        while (_gameState.Phase != GamePhase.Finished)
        {
            await _gameController.PlayRoundAsync(_cancellationTokenSource.Token);
        }
    }
}
```

**4. Update GameController Constructor:**

```csharp
public class GameController : IGameController
{
    private readonly IDeckProviderService _deckProviderService;
    private readonly IPlayer[] _players;
    private readonly IPlayerController[] _playerControllers;
    private readonly IGameView _gameView;
    private readonly IGameMode _gameMode;
    private readonly GameState _gameState;
    private readonly ILogger _logger;
    
    [Inject]
    public GameController(
        IDeckProviderService deckProviderService,
        IPlayer[] players,
        IPlayerController[] playerControllers,
        IGameView gameView,
        IGameMode gameMode,
        GameState gameState,
        ILogger logger)
    {
        // Constructor injection handled by DI container
        _deckProviderService = deckProviderService;
        _players = players;
        _playerControllers = playerControllers;
        _gameView = gameView;
        _gameMode = gameMode;
        _gameState = gameState;
        _logger = logger;
    }
}
```

**5. Benefits in Testing:**

```csharp
[Test]
public void GameController_StartGame_InitializesDeck()
{
    // Arrange
    var mockDeckService = Substitute.For<IDeckProviderService>();
    var mockView = Substitute.For<IGameView>();
    var mockMode = Substitute.For<IGameMode>();
    var mockLogger = Substitute.For<ILogger>();
    
    var players = new[] { Substitute.For<IPlayer>() };
    var controllers = new[] { Substitute.For<IPlayerController>() };
    var gameState = new GameState();
    
    // Easy to inject mocks with DI
    var controller = new GameController(
        mockDeckService,
        players,
        controllers,
        mockView,
        mockMode,
        gameState,
        mockLogger);
    
    // Act & Assert
    // Test implementation...
}
```

#### Alternative: Manual DI Container

If preferring a lightweight solution without external packages:

```csharp
public class ServiceContainer
{
    private readonly Dictionary<Type, object> _services = new();
    
    public void Register<T>(T instance) where T : class
    {
        _services[typeof(T)] = instance;
    }
    
    public T Resolve<T>() where T : class
    {
        return _services.TryGetValue(typeof(T), out var service) 
            ? service as T 
            : throw new InvalidOperationException($"Service {typeof(T)} not registered");
    }
}
```

## Acknowledgments

- [Deck of Cards API](https://deckofcardsapi.com/) for providing the card deck service
- Unity Technologies for the Unity game engine
