using System;

namespace _Scripts.Core
{
    public static class GameCore
    {
        #region Events

        public static Action OnPlayerKilled;
        public static Action OnPlayerReady;
        public static Action OnGameReset;
        public static Action OnPlayerHit;
        public static Action OnFlagTaken;
        public static Action OnFlagLost;
        public static Action OnFlagDelivered;
        public static Action OnGameLost;

        #endregion

        public static float GameTime { get; set; }
    }
}