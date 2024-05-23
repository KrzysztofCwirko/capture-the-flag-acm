using _Scripts.Utility;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace _Scripts.Enemy
{
    /// <summary>
    /// Class that acts as a base to inherit and customize enemy behaviour
    /// </summary>
    public abstract class Enemy : MonoBehaviour
    {
        #region Public settings

        [Header("Setup")] 
        public NavMeshAgent agent;
        [SerializeField] private float viewDistance;
        [SerializeField] private float viewWidthMultiplier;
        
        [Header("Effects")] 
        [SerializeField] private GameObject noticedIndicator;
        [SerializeField] private GameObject escapedIndicator;

#if UNITY_EDITOR
        [SerializeField] private bool debugSight;
#endif
        [SerializeField] private LineRenderer debugSightLine;

        #endregion

        #region Public properties

        [HideInInspector] public bool playerVisible;

        #endregion
        
        #region Private properties

        private (Vector3, Quaternion) _startingPositionAndRotation;

        private const float AnimationTime = .2f;
        private const float NoticeDelay = 2f;

        private float _noticedDelay;

        private static int _priority;
        
        #endregion

        #region Event functions

        private void Start()
        {
            _priority += 1;
            agent.avoidancePriority = _priority;

#if UNITY_EDITOR
            debugSightLine.positionCount = 3;
            debugSightLine.SetPositions(new[]
            {
                Vector3.zero, new Vector3(-viewDistance * viewWidthMultiplier, 0f, viewDistance),
                new Vector3(viewDistance * viewWidthMultiplier, 0f, viewDistance)
            });
#else
            Destroy(debugSightLine.gameObject);
#endif
        }

#if UNITY_EDITOR
        private void Update()
        {
            debugSightLine.gameObject.SetActive(debugSight);
        }
#endif

        private void OnValidate()
        {
            debugSightLine.gameObject.SetActive(debugSight);
            debugSightLine.positionCount = 3;
            debugSightLine.SetPositions(new[]
            {
                Vector3.zero, new Vector3(-viewDistance * viewWidthMultiplier, 0f, viewDistance),
                new Vector3(viewDistance * viewWidthMultiplier, 0f, viewDistance)
            });
        }

        #endregion
        
        #region Lifecycle
        
        public void Init()
        {
            var transform1 = transform;
            _startingPositionAndRotation = (transform1.position, transform1.rotation);
                OnInit();
        }
        protected virtual void OnInit() {}
        protected virtual void OnStartIdle() {}
        public abstract void IdleLoop();
        protected virtual void OnStartChasing() {}
        public abstract void ChasePlayerLoop(Transform player);
        
        public void TryToFindPlayer(Transform player)
        {
            var myTransform = transform;
            var target = myTransform.InverseTransformPoint(player.position);
            var visible = target.z > Mathf.Abs(target.x/viewWidthMultiplier) && target.z < viewDistance;

            if (visible)
            {
                //ray towards the player to see if there is something between
                //2.25f is an offset which represents the head level
                var position = myTransform.position.ModifyY(2.25f);
                var ray = new Ray(position, (player.position.ModifyY(2.25f) - position).normalized);

                if (Physics.Raycast(ray, out var hit, viewDistance))
                {
                    
                    if (!hit.collider.CompareTag("Player"))
                        visible = false;
                }
                else
                {
                    visible = false;
                }
            }

            //Small delay after losing Player from sight
            if (playerVisible && !visible && _noticedDelay < NoticeDelay)
            {
                _noticedDelay += Time.deltaTime;
                return;
            }

            _noticedDelay = 0f;
            playerVisible = visible;
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
            
            OnStartChasing();
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
            
            OnStartIdle();
        }
        
        public void OnPlayerReady()
        {
            if(!gameObject.activeSelf) return;
            agent.isStopped = false;
        }
        
        public virtual void OnPlayerKilled()
        {
            playerVisible = false;

            if(!gameObject.activeSelf) return;
            agent.isStopped = false;
            agent.SetDestination(_startingPositionAndRotation.Item1);
        }
        
        internal virtual void KillMe()
        {
            noticedIndicator.transform.DOKill();
            escapedIndicator.transform.DOKill();
            escapedIndicator.gameObject.SetActive(false);
            noticedIndicator.gameObject.SetActive(false);
            agent.isStopped = true;
            gameObject.SetActive(false);
        }

        public void OnGameReset()
        {
            agent.isStopped = false;
            agent.ResetPath();
            
            agent.enabled = false;
            transform.SetPositionAndRotation(_startingPositionAndRotation.Item1, _startingPositionAndRotation.Item2);
            agent.enabled = true;
            
            _noticedDelay = 0f;
            playerVisible = false;
            OnPlayerEscaped();
        }

        #endregion
    }
}