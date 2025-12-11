using System;
using System.Threading;
using System.Threading.Tasks;
using Game.Core.Abstractions;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Client.Runtime
{
    /// <summary>
    /// Unity implementation of IPlayerController for local/human player.
    /// Waits for player input via a UI Button click.
    /// </summary>
    public class LocalPlayerController : MonoBehaviour, IPlayerController
    {
        [SerializeField, Tooltip("Reference to the Draw Button Component")]
        private Button drawButton;
        
        private TaskCompletionSource<bool> _completionSource;
        private CancellationTokenRegistration _cancellationRegistration;
        private readonly object _lockObject = new();

        private void OnEnable()
        {
            if (drawButton)
            {
                drawButton.onClick.AddListener(OnDrawButtonClicked);
                drawButton.gameObject.SetActive(false);
            }
        }

        private void OnDisable()
        {
            if (drawButton)
            {
                drawButton.onClick.RemoveListener(OnDrawButtonClicked);
            }
            
            // Cancel any pending request
            lock (_lockObject)
            {
                if (_completionSource != null)
                {
                    _completionSource.TrySetCanceled();
                    _completionSource = null;
                }
                if (_cancellationRegistration != default)
                {
                    _cancellationRegistration.Dispose();
                }
            }
        }

        private void OnDrawButtonClicked()
        {
            lock (_lockObject)
            {
                drawButton.gameObject.SetActive(false);
                
                if (_completionSource == null || _completionSource.Task.IsCompleted) return;
                
                _completionSource.TrySetResult(true);
                _completionSource = null;
                if (_cancellationRegistration != default)
                {
                    _cancellationRegistration.Dispose();
                }
            }
        }
        
        public Task RequestDrawAsync(CancellationToken cancellationToken = default)
        {
            lock (_lockObject)
            {
                // If there's already a pending request, cancel it
                if (_completionSource != null && !_completionSource.Task.IsCompleted)
                {
                    _completionSource.TrySetCanceled();
                    if (_cancellationRegistration != default)
                    {
                        _cancellationRegistration.Dispose();
                    }
                }

                // Create a new completion source
                _completionSource = new TaskCompletionSource<bool>();
                
                // Register cancellation
                if (cancellationToken.CanBeCanceled)
                {
                    _cancellationRegistration = cancellationToken.Register(CheckAndCancelCompletionSource);
                }
                
                drawButton.gameObject.SetActive(true);
            }

            return _completionSource.Task;

            void CheckAndCancelCompletionSource()
            {
                lock (_lockObject)
                {
                    if (_completionSource == null || _completionSource.Task.IsCompleted) return;
                    _completionSource.TrySetCanceled();
                    _completionSource = null;
                }
            }
        }
    }
}
