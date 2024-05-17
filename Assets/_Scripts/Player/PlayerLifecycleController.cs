using System;
using _Scripts.Utility;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Player
{
    /// <summary>
    /// Control player life/ves and re/spawning
    /// </summary>
    public class PlayerLifecycleController : StaticInstance<PlayerLifecycleController>
    {
        #region Public settings

        [Header("Spawn")]
        [SerializeField] private float respawnTime;
        [SerializeField] private PlayerInput input;

        [Header("UI - HUD")] 
        [SerializeField] private GameObject[] lives;
        
        #endregion

        #region Events

        public static Action OnPlayerKilled;
        public static Action OnPlayerReady;
        public static Action OnGameReset;
        public static Action OnPlayerHit;

        #endregion
        
        #region Private properties

        private int _currentLives;
        private bool _invincible;
        private Tween _resetting;

        #endregion

        #region Event functions

        private void Start()
        {
            ResetLifecycle();

            OnPlayerHit += TakeHit;
            OnPlayerKilled += PlayerKilled;
            OnPlayerReady += PlayerReady;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnPlayerHit -= TakeHit;
            OnPlayerKilled -= PlayerKilled;
            OnPlayerReady -= PlayerReady;
        }

        #endregion

        #region Re/Spawning

        private void ResetLifecycle()
        {
            _resetting?.Kill();
            OnGameReset?.Invoke();

            _currentLives = lives.Length;
            ChangeLives(_currentLives);
            _invincible = false;
        }

        #endregion

        #region Lives
        
        private void PlayerReady()
        {
            input.ActivateInput();
        }

        private void PlayerKilled()
        {
            input.DeactivateInput();
        }

        /// <summary>
        /// Change the life count
        /// </summary>
        /// <param name="newLives">New amount of lives</param>
        private void ChangeLives(int newLives)
        {
            for (var i = 0; i < lives.Length; i++)
            {
                lives[i].SetActive(i < newLives);
            }

            if (_currentLives > newLives)
            {
                OnPlayerKilled?.Invoke();
                _invincible = true;
                
                if (newLives == 0)
                {
                    GameLost();
                }
                else
                {
                    _resetting = DOTween.Sequence().AppendInterval(respawnTime);
                    _resetting.onComplete +=() =>
                    {
                        _invincible = false;
                        OnPlayerReady?.Invoke();
                    };
                }
            }
            
            _currentLives = newLives;
        }

        private void GameLost()
        {
            ResetLifecycle();
        }
        
        private void TakeHit()
        {
            if(_invincible) return;
            ChangeLives(_currentLives-1);
        }

        #endregion
    }
}