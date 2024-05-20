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
        [SerializeField] private float viewDistance;
        [SerializeField] private float viewWidth;
        
        [Header("Effects")] 
        [SerializeField] private GameObject noticedIndicator;
        [SerializeField] private GameObject escapedIndicator;

        #endregion

        #region Public properties

        [HideInInspector] public bool playerVisible;

        #endregion
        
        #region Private properties

        private (Vector3, Quaternion) _startingPositionAndRotation;

        private const float AnimationTime = .2f;

        private static int _priority;
        
        #endregion

        #region Event functions

        private void Start()
        {
            _priority += 1;
            _startingPositionAndRotation = (transform.position, transform.rotation);
            agent.avoidancePriority = _priority;
        }

        protected void OnEnable()
        {
            transform.SetPositionAndRotation(_startingPositionAndRotation.Item1, _startingPositionAndRotation.Item2);
            agent.isStopped = false;
        }
        
        #endregion
        
        #region Lifecycle

        public abstract void Idle();

        public void OnPlayerKilled()
        {
            playerVisible = false;

            if(!gameObject.activeSelf) return;
            agent.isStopped = false;
            agent.SetDestination(_startingPositionAndRotation.Item1);
        }

        public virtual void OnGameReset()
        {
            transform.SetPositionAndRotation(_startingPositionAndRotation.Item1, _startingPositionAndRotation.Item2);
        }

        #endregion

        #region Player
        
        /// <summary>
        /// Simple checking if player is in given a rect
        /// </summary>
        /// <param name="player"></param>
        public void CheckForPlayer(Transform player)
        {
            var target = transform.InverseTransformPoint(player.position);
            playerVisible = (target.z > 0f && target.z < viewDistance) 
                && Mathf.Abs(target.x) < viewWidth/2f;
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

        public void OnPlayerEscaped()
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