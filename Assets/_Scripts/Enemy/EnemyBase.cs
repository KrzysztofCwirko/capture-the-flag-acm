using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace _Scripts.Enemy
{
    /// <summary>
    /// Class that acts as a base to inherit and customize enemy behaviour
    /// </summary>
    public abstract class EnemyBase : MonoBehaviour
    {
        #region Public settings

        [Header("Setup")] 
        public NavMeshAgent agent;

        [Header("Effects")] 
        [SerializeField] private GameObject noticedIndicator;
        [SerializeField] private GameObject escapedIndicator;

        #endregion

        #region Public properties

        [HideInInspector] public bool playerVisible;

        #endregion
        
        #region Private properties

        private (Vector3, Quaternion) _startingPositionAndRotation;

        private const float AnimationTime = .5f;
        
        #endregion

        #region Event functions

        private void Start()
        {
            _startingPositionAndRotation = (transform.position, transform.rotation);
        }

        protected void OnEnable()
        {
            agent.isStopped = false;
        }

        private void OnDisable()
        {
            transform.SetPositionAndRotation(_startingPositionAndRotation.Item1, _startingPositionAndRotation.Item2);
        }
        
        #endregion
        
        #region Lifecycle

        public abstract void Idle();

        public virtual void OnGameReset()
        {
            transform.SetPositionAndRotation(_startingPositionAndRotation.Item1, _startingPositionAndRotation.Item2);
        }

        #endregion

        #region Player
        
        public void CheckForPlayer()
        {
            playerVisible = true;
        }
        
        public void OnPlayerNoticed()
        {
            escapedIndicator.SetActive(false);
            escapedIndicator.transform.DOKill();
            
            noticedIndicator.gameObject.SetActive(true);
            noticedIndicator.transform.DOKill();
            noticedIndicator.transform.localScale = Vector3.zero;
            noticedIndicator.transform.DOScale(1f, AnimationTime).onComplete += () =>
            {
                noticedIndicator.transform.DOScale(0f, AnimationTime).SetDelay(1f).onComplete += () =>
                {
                    noticedIndicator.SetActive(false);
                };
            };
        }

        public virtual void OnPlayerEscaped()
        {
            noticedIndicator.SetActive(false);
            noticedIndicator.transform.DOKill();
            
            escapedIndicator.gameObject.SetActive(true);
            escapedIndicator.transform.DOKill();
            escapedIndicator.transform.localScale = Vector3.zero;
            escapedIndicator.transform.DOScale(1f, AnimationTime).onComplete += () =>
            {
                escapedIndicator.transform.DOScale(0f, AnimationTime).SetDelay(1f).onComplete += () =>
                {
                    escapedIndicator.SetActive(false);
                };
            };
        }

        public abstract void PlayerVisible(Transform player);

        public virtual void OnPlayerDied()
        {
            
        }

        protected void OnDeath()
        {
            playerVisible = false;
            noticedIndicator.transform.DOKill();
            escapedIndicator.transform.DOKill();
            escapedIndicator.gameObject.SetActive(false);
            noticedIndicator.gameObject.SetActive(false);
            agent.isStopped = true;
            gameObject.SetActive(false);
        }

        public void OnReady()
        {
            if(!gameObject.activeSelf) return;
            agent.isStopped = false;
        }

        #endregion
    }
}