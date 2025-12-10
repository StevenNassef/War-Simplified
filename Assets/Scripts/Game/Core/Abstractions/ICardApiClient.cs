using System.Threading;
using System.Threading.Tasks;
using Game.Core.Model;

namespace Game.Core.Abstractions
{
    /// <summary>
    /// API client for card decks.
    /// An abstraction to allow mocking in tests and to isolate the API client from core logic.
    /// </summary>
    /// 
    public interface ICardApiClient
    {
        /// <summary>
        /// Creates a new deck and shuffles it.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Deck Id and Remaining count</returns>
        Task<(string deckId, int remaining)> CreateAndShuffleNewDeckAsync(
            CancellationToken cancellationToken = default);

        
        /// <summary>
        /// Shuffles an existing deck.
        /// </summary>
        /// <param name="deckId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Deck Id and Remaining count</returns>
        Task<(string deckId, int remaining)> ReshuffleDeckAsync(string deckId,
            CancellationToken cancellationToken = default);

        
        /// <summary>
        /// Draws cards from a deck.
        /// </summary>
        /// <param name="deckId"></param>
        /// <param name="count"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Deck Id and Remaining count</returns>
        Task<(Card[] drawnCards, int remaining)> DrawCardsAsync(string deckId, int count,
            CancellationToken cancellationToken = default);
    }
}