using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Core.Abstractions;
using Game.Core.Model;
using UnityEngine;

namespace Game.Client.Runtime
{
    /// <summary>
    /// Base class for GameView implementations.
    /// </summary>
    public abstract class GameViewBase : MonoBehaviour, IGameView
    {
        public abstract void Initialize(IPlayer[] players);
        protected abstract Task StartGameInternal();
        protected abstract Task UpdateScoresInternal(IReadOnlyList<int> scores);

        protected abstract Task ShowRoundResultInternal(int roundIndex, IReadOnlyList<Card> cards,
            IReadOnlyList<int> scores,
            int roundWinnerId);

        protected abstract Task ShowGameOverInternal(int winnerId);
        protected abstract Task EndGameInternal();
        
        protected IPlayer[] Players;
        private readonly Queue<Func<Task>> _taskQueue = new();
        private bool _isProcessingQueue;

        private void EnqueueTask(Func<Task> taskFactory)
        {
            lock (_taskQueue)
            {
                _taskQueue.Enqueue(taskFactory);

                if (_isProcessingQueue) return;
                _isProcessingQueue = true;
                ProcessTaskQueueAsync();
            }
        }

        private async void ProcessTaskQueueAsync()
        {
            try
            {
                while (true)
                {
                    Func<Task> taskFactory;

                    lock (_taskQueue)
                    {
                        if (_taskQueue.Count == 0)
                        {
                            _isProcessingQueue = false;
                            return;
                        }

                        taskFactory = _taskQueue.Dequeue();
                    }

                    try
                    {
                        await taskFactory();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"[GameView] Error processing task: {ex}");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(new Exception($"[GameView] Error processing task queue: {e}", e));
            }
        }
        #region IGameView implementation
        public void StartGame()
        {
            EnqueueTask(StartGameInternal);
        }

        public void UpdateScores(IReadOnlyList<int> scores)
        {
            // Create a copy of the scores list to avoid issues with the list being modified
            var scoresCopy = new List<int>(scores);
            EnqueueTask(() => UpdateScoresInternal(scoresCopy));
        }

        public void ShowRoundResult(int roundIndex, IReadOnlyList<Card> cards, IReadOnlyList<int> scores,
            int roundWinnerId)
        {
            // Create copies to avoid issues with collections being modified
            var cardsCopy = new List<Card>(cards);
            var scoresCopy = new List<int>(scores);
            EnqueueTask(() => ShowRoundResultInternal(roundIndex, cardsCopy, scoresCopy, roundWinnerId));
        }

        public void ShowGameOver(int winnerId)
        {
            EnqueueTask(() => ShowGameOverInternal(winnerId));
        }

        public void EndGame()
        {
            EnqueueTask(EndGameInternal);
        }
        
        #endregion

    }
}