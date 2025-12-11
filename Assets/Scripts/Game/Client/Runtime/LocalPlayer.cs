using System;
using Game.Core.Abstractions;
using UnityEngine;

namespace Game.Client.Runtime
{
    [Serializable]
    public class LocalPlayer : IPlayer
    {
        [SerializeField] public string id;
        [SerializeField] public string displayName;

        public string Id => id;
        public string DisplayName => displayName;

        public LocalPlayer(string id, string displayName)
        {
            this.id = id;
            this.displayName = displayName;
        }
    }

}
