using System;
using UnityEngine;

namespace CardWar.API
{
    public class GameLogger
    {
        private readonly string _tag;

        public GameLogger(string tag)
        {
            _tag = tag;
        }

        public void Log(string message) => Debug.Log($"[{_tag}] {message}");
        public void LogWarning(string message) => Debug.LogWarning($"[{_tag}] {message}");
        public void LogError(string message) => Debug.LogError($"[{_tag}] {message}");
        public void LogException(Exception exception) => Debug.LogException(exception);
    }
}
