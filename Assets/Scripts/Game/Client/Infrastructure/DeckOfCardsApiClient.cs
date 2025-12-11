using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Game.Core.Abstractions;
using Game.Core.Model;
using Game.Client.DTOs;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Client.Infrastructure
{
    /// <summary>
    /// Unity implementation of ICardApiClient using the Deck of Cards API (https://deckofcardsapi.com/)
    /// Uses UnityWebRequest for HTTP calls.
    /// </summary>
    public class DeckOfCardsApiClient : ICardApiClient
    {
        
        private const string BaseUrl = "https://deckofcardsapi.com/api/deck";

        public async Task<(string deckId, int remaining)> CreateAndShuffleNewDeckAsync(
            CancellationToken cancellationToken = default)
        {
            var url = $"{BaseUrl}/new/shuffle/?deck_count=1";
            var response = await SendRequestAsync<DeckResponse>(url, cancellationToken);
            
            return !response.success
                ? throw new Exception($"Failed to create and shuffle deck: {response.error ?? "Unknown error"}")
                : (response.deck_id, response.remaining);
        }

        public async Task<(string deckId, int remaining)> ReshuffleDeckAsync(string deckId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(deckId) || string.IsNullOrWhiteSpace(deckId))
                throw new ArgumentException("Deck ID cannot be null or empty", nameof(deckId));

            var url = $"{BaseUrl}/{deckId}/shuffle/";
            var response = await SendRequestAsync<DeckResponse>(url, cancellationToken);

            return !response.success
                ? throw new Exception($"Failed to reshuffle deck: {response.error ?? "Unknown error"}")
                : (response.deck_id, response.remaining);
        }

        public async Task<(Card[] drawnCards, int remaining)> DrawCardsAsync(string deckId, int count,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(deckId) || string.IsNullOrWhiteSpace(deckId))
                throw new ArgumentException("Deck ID cannot be null or empty", nameof(deckId));
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count), count, "Count must be greater than 0");

            var url = $"{BaseUrl}/{deckId}/draw/?count={count}";
            var response = await SendRequestAsync<DrawCardsResponse>(url, cancellationToken);

            if (!response.success) throw new Exception($"Failed to draw cards: {response.error ?? "Unknown error"}");

            var cards = response.cards.Select(ConvertToCard).ToArray();
            return (cards, response.remaining);
        }

        private static Card ConvertToCard(ApiCard apiCard)
        {
            var suit = ParseSuit(apiCard.suit);
            var value = ParseValue(apiCard.value);
            return new Card(suit, value);
        }

        private static CardSuit ParseSuit(string suit)
        {
            return suit.ToUpperInvariant() switch
            {
                "SPADES" => CardSuit.Spades,
                "HEARTS" => CardSuit.Hearts,
                "DIAMONDS" => CardSuit.Diamonds,
                "CLUBS" => CardSuit.Clubs,
                _ => throw new ArgumentException($"Unknown suit: {suit}", nameof(suit))
            };
        }

        private static CardValue ParseValue(string value)
        {
            return value.ToUpperInvariant() switch
            {
                "2" => CardValue.Two,
                "3" => CardValue.Three,
                "4" => CardValue.Four,
                "5" => CardValue.Five,
                "6" => CardValue.Six,
                "7" => CardValue.Seven,
                "8" => CardValue.Eight,
                "9" => CardValue.Nine,
                "10" => CardValue.Ten,
                "JACK" => CardValue.Jack,
                "QUEEN" => CardValue.Queen,
                "KING" => CardValue.King,
                "ACE" => CardValue.Ace,
                _ => throw new ArgumentException($"Unknown card value: {value}", nameof(value))
            };
        }

        private async Task<T> SendRequestAsync<T>(string url, CancellationToken cancellationToken)
        {
            using var request = UnityWebRequest.Get(url);
            var operation = request.SendWebRequest();
            
            await AwaitUnityWebRequestAsync(operation, cancellationToken);

            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new Exception($"HTTP request failed: {request.error} (URL: {url})");
            }
            
            var json = request.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                throw new Exception($"Empty response received from API (URL: {url})");
            }

            try
            {
                return JsonUtility.FromJson<T>(json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to parse JSON response: {ex.Message} (URL: {url})", ex);
            }
        }

        private async Task AwaitUnityWebRequestAsync(
            UnityWebRequestAsyncOperation operation, 
            CancellationToken cancellationToken)
        {
            while (!operation.isDone)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Yield();
            }
        }
    }
}