using System;

namespace Game.Client.DTOs
{
    /// <summary>
    /// Data Transfer Objects for API responses.
    /// These are used to deserialize JSON responses from the API (Deck of Cards).
    /// </summary>
    
    [Serializable]
    internal class DeckResponse
    {
        public bool success;
        public string deck_id;
        public bool shuffled;
        public int remaining;
        public string error;
    }

    [Serializable]
    internal class DrawCardsResponse
    {
        public bool success;
        public string deck_id;
        public ApiCard[] cards;
        public int remaining;
        public string error;
    }

    [Serializable]
    internal class ApiCard
    {
        public string code;
        public string image;
        public Images images;
        public string value;
        public string suit;
    }

    [Serializable]
    internal class Images
    {
        public string svg;
        public string png;
    }
}