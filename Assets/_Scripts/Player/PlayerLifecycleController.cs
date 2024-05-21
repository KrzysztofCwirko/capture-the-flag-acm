using System.Collections;
using _Scripts.Utility;
using DG.Tweening;
using UnityEngine;

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

        [Header("UI - HUD")] 
        [SerializeField] private GameObject[] lives;
        
        #endregion
        
        #region Private properties

        private int _currentLives;
        public bool Dead { get; private set; }
        private Tween _resetting;

        #endregion

        #region Event functions

        private IEnumerator Start()
        {
            //wait for all enemies to start
            yield return null;
            
            CoreEvents.OnPlayerHit += TakeHit;
            CoreEvents.OnGameReset += ResetLifecycle;
            
            CoreEvents.OnGameReset?.Invoke();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            CoreEvents.OnPlayerHit -= TakeHit;
            CoreEvents.OnGameReset -= ResetLifecycle;
        }

        #endregion

        #region Re/Spawning

        private void ResetLifecycle()
        {
            _resetting?.Kill();
            _currentLives = lives.Length;
            ChangeLives(_currentLives);
            Dead = false;
        }

        #endregion

        #region Lives

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
                CoreEvents.OnFlagLost?.Invoke();
                CoreEvents.OnPlayerKilled?.Invoke();
                Dead = true;
                
                if (newLives == 0)
                {
                    GameLost();
                }
                else
                {
                    _resetting = DOTween.Sequence().AppendInterval(respawnTime);
                    _resetting.onComplete +=() =>
                    {
                        Dead = false;
                        CoreEvents.OnPlayerReady?.Invoke();
                    };
                }
            }
            
            _currentLives = newLives;
        }

        private void GameLost()
        {
            _resetting = DOTween.Sequence().AppendInterval(respawnTime);
            _resetting.onComplete +=() =>
            {
                CoreEvents.OnGameReset?.Invoke();
            };
        }
        
        private void TakeHit()
        {
            if(Dead) return;
            ChangeLives(_currentLives-1);
        }

        #endregion
    }
}