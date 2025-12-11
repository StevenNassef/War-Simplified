using System;
using Game.Core.Abstractions;

namespace Game.Client.Runtime
{
    [Serializable]
    public class LocalPlayer : IPlayer
    {
        public string Id { get; }
        public string DisplayName { get; }

        public LocalPlayer(string id, string displayName)
        {
            Id = id;
            DisplayName = displayName;
        }
    }

}
