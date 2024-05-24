using _Scripts.Core;
using _Scripts.Utility;
using UnityEngine;

namespace _Scripts.Enemy.Types
{
    /// <summary>
    /// Chase Player and explode
    /// </summary>
    public class Kamikaze : Enemy
    {
        #region Public settings
        
        [SerializeField] private float chaseSpeed;
        [SerializeField] private float idleMovingRadius;
        [SerializeField] private float kaboomTolerance;
        
        #endregion

        #region Private properties

        private float _idleSpeed;

        #endregion
        
        #region Lifecycle

        protected override void OnInit()
        {
            _idleSpeed = agent.speed;
        }

        protected override void OnStartIdle()
        {
            EffectsManager.Instance.ClearSoundEffect("scream", transform);
            agent.speed = _idleSpeed;
        }

        public override void IdleLoop()
        {
            if (agent.hasPath) return;
            var newDestination = transform.position.FindRandomPositionOnNavMesh(idleMovingRadius);
            agent.SetDestination(newDestination);
        }
        
        protected override void OnStartChasing()
        {        
            var transform1 = transform;
            EffectsManager.Instance.PlaySoundEffect("scream", transform1.position, transform1, volume:15f);
            agent.speed = chaseSpeed;
        }

        public override void ChasePlayerLoop(Transform player)
        {
            agent.SetDestination(player.position);
            if (Vector3.Distance(transform.position, player.position) > kaboomTolerance) return;
            GameCore.OnPlayerHit?.Invoke();
            KillMe();
        }

        public override void OnPlayerKilled()
        {
            base.OnPlayerKilled();
            EffectsManager.Instance.ClearSoundEffect("scream", transform);
        }

        internal override void KillMe()
        {
            base.KillMe();
            EffectsManager.Instance.ClearSoundEffect("scream", transform);
            var position = transform.position;
            EffectsManager.Instance.PlaySoundEffect("Boom", position, volume:15f);
            EffectsManager.Instance.ShowParticle("Boom", position + new Vector3(0, 1.5f)); 
            GameCore.OnShakeCamera?.Invoke(position, 4f, GameCore.DefaultShakeDuration);
        }

        #endregion
    }
}