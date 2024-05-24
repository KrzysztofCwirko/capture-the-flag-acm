using System;
using UnityEngine;

namespace _Scripts.Core
{
    public static class GameCore
    {
        #region Events

        public static Action OnPlayerKilled { get; set; }
        public static Action OnPlayerReady { get; set; }
        public static Action OnGameReset { get; set; }
        public static Action OnPlayerHit { get; set; }
        public static Action OnFlagTaken { get; set; }
        public static Action OnFlagLost { get; set; }
        public static Action OnFlagDelivered { get; set; }
        public static Action OnGameLost { get; set; }
        /// <summary>
        /// Shake source position, shake strength, shake duration
        /// </summary>
        public static Action<Vector3, float, float> OnShakeCamera { get; set; }

        #endregion

        public static float GameTime { get; set; }
        public static string PlayerName
        {
            get => PlayerPrefs.GetString("PlayerName", "No name");
            set => PlayerPrefs.SetString("PlayerName", string.IsNullOrEmpty(value) ? "No name" : value);
        }
        public static Func<Transform, Enemy.Enemy> GetEnemyByTransform { get; set; }
        public const float DefaultShakeDuration = 0.3f;
        public static bool GamePaused => Time.timeScale < 0.001f;
    }
}