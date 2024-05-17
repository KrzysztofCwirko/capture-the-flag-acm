using _Scripts.Player;
using UnityEngine;

namespace _Scripts.Enemy.Types
{
    /// <summary>
    /// Chase Player and explode
    /// </summary>
    public class Kamikaze : EnemyBase
    {
        #region Public settings

        [SerializeField] private float idleMovingRadius;
        [SerializeField] private float kaboomTolerance;
        
        #endregion
        
        #region Lifecycle

        public override void Idle()
        {
            if (agent.hasPath) return;
            var newDestination = Random.onUnitSphere * idleMovingRadius;
            agent.SetDestination(newDestination);
        }

        #endregion
        
        #region Player

        public override void PlayerVisible(Transform player)
        {
            agent.SetDestination(player.position);
            if (Vector3.Distance(transform.position, player.position) > kaboomTolerance) return;
            PlayerLifecycleController.OnPlayerHit?.Invoke();
            OnDeath();
        }
        
        public override void OnPlayerEscaped()
        {
            base.OnPlayerEscaped();
        }

        public override void OnPlayerDied()
        {
            base.OnPlayerDied();
        }

        #endregion
    }
}