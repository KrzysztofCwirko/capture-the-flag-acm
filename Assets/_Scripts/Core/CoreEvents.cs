using System;

namespace _Scripts
{
    public static class CoreEvents
    {
        #region Events

        public static Action OnPlayerKilled;
        public static Action OnPlayerReady;
        public static Action OnGameReset;
        public static Action OnPlayerHit;
        public static Action OnFlagTaken;
        public static Action OnFlagLost;
        public static Action OnFlagDelivered;

        #endregion
    }
}