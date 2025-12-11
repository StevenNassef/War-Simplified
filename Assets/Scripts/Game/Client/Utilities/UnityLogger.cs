using System;
using UnityEngine;
using ILogger = Game.Core.Abstractions.ILogger;
using Object = UnityEngine.Object;

namespace Game.Client.Utilities
{
    public class UnityLogger : ILogger
    {
        public void Log(object message, object context = null)
        {
            if (context is Object unityObject)
            {
                Debug.Log(message, unityObject);
            }
            else
            {
                Debug.Log(message);    
            }
        }

        public void LogError(object message, object context = null)
        {
            if (context is Object unityObject)
            {
                Debug.LogError(message, unityObject);
            }
            else
            {
                Debug.LogError(message);    
            }
        }

        public void LogWarning(string message, object context = null)
        {
            if (context is Object unityObject)
            {
                Debug.LogWarning(message, unityObject);
            }
            else
            {
                Debug.LogWarning(message);    
            }
        }

        public void LogException(Exception exception, object context = null)
        {
            if (context is Object unityObject)
            {
                Debug.LogException(exception, unityObject);
            }
            else
            {
                Debug.LogException(exception);    
            }
        }
    }
}
