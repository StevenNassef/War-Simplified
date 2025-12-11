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
        private readonly Queue<(string TaskName, Func<Task> TaskFactory)> _taskQueue = new();
        private bool _isProcessingQueue;

        private void EnqueueTask(string taskName, Func<Task> taskFactory)
        {
            lock (_taskQueue)
            {
                _taskQueue.Enqueue((taskName, taskFactory));

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
                    (string TaskName, Func<Task> taskFactory) tuple;

                    lock (_taskQueue)
                    {
                        if (_taskQueue.Count == 0)
                        {
                            _isProcessingQueue = false;
                            return;
                        }

                        tuple = _taskQueue.Dequeue();
                    }

                    try
                    {
                        Debug.Log($"[GameView] Task Started: {tuple.TaskName}");
                        await tuple.taskFactory();
                        Debug.Log($"[GameView] Task Ended: {tuple.TaskName}");
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
            EnqueueTask(nameof(StartGame), StartGameInternal);
        }

        public void UpdateScores(IReadOnlyList<int> scores)
        {
            // Create a copy of the scores list to avoid issues with the list being modified
            var scoresCopy = new List<int>(scores);
            EnqueueTask(nameof(UpdateScores), () => UpdateScoresInternal(scoresCopy));
        }

        public void ShowRoundResult(int roundIndex, IReadOnlyList<Card> cards, IReadOnlyList<int> scores,
            int roundWinnerId)
        {
            // Create copies to avoid issues with collections being modified
            var cardsCopy = new List<Card>(cards);
            var scoresCopy = new List<int>(scores);
            EnqueueTask(nameof(ShowRoundResult) ,() => ShowRoundResultInternal(roundIndex, cardsCopy, scoresCopy, roundWinnerId));
        }

        public void ShowGameOver(int winnerId)
        {
            EnqueueTask(nameof(ShowGameOver), () => ShowGameOverInternal(winnerId));
        }

        public void EndGame()
        {
            EnqueueTask(nameof(EndGame), EndGameInternal);
        }
        
        #endregion

    }
}