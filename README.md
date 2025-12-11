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


## Acknowledgments

- [Deck of Cards API](https://deckofcardsapi.com/) for providing the card deck service
- Unity Technologies for the Unity game engine
