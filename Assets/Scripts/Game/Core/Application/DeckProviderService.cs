using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Game.Core.Abstractions;
using Game.Core.Model;

#nullable enable

namespace Game.Core.Application
{
    public class DeckProviderService : IDeckProviderService
    {
        private readonly ICardApiClient _apiClient;

        private string _deckId = string.Empty;
        private int _remainingCardsInDeck;

        public DeckProviderService(ICardApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            var (deckId, remaining) = await _apiClient.CreateAndShuffleNewDeckAsync(cancellationToken);
            _deckId = deckId;
            _remainingCardsInDeck = remaining;
        }

        public async Task ReshuffleAsync(CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(_deckId)) throw new InvalidOperationException("Deck not initialized");
            var (deckId, remaining) = await _apiClient.ReshuffleDeckAsync(_deckId, cancellationToken);
            _deckId = deckId;
            _remainingCardsInDeck = remaining;
        }

        public async Task<IReadOnlyList<Card>> DrawCardsAsync(int count = 1,
            CancellationToken cancellationToken = default)
        {
            if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count), count, "Count must be greater than 0");

            var (drawnCards, remaining) = await _apiClient.DrawCardsAsync(_deckId, count, cancellationToken);

            _remainingCardsInDeck = remaining;
            return drawnCards;
        }
    }
}