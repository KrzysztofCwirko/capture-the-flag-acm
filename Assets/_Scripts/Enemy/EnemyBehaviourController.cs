using System.Collections;
using System.Collections.Generic;
using _Scripts.Player;
using _Scripts.World;
using UnityEngine;

namespace _Scripts.Enemy
{
    public class EnemyBehaviourController : MonoBehaviour
    {
        #region Public settings

        [SerializeField] private List<EnemySetting> enemyTypes;

        #endregion

        #region Private properties

        private readonly List<EnemyBase> _enemies = new List<EnemyBase>();

        #endregion
        
        #region Event functions

        private IEnumerator Start()
        {
            CoreEvents.OnGameReset += GameReset;
            CoreEvents.OnPlayerReady += PlayerReady;
            CoreEvents.OnPlayerKilled += PlayerKilled;

            yield return null;  //wait for PrefabPooler to finish spawning
            foreach (var enemyType in enemyTypes)
            {
                for (var e = 0; e < enemyType.count; e++)
                {
                    var enemy = (EnemyBase)PrefabPooler.Instance.Pool(enemyType.baseType, parent:transform);
                    _enemies.Add(enemy);
                }
            }
        }

        private void Update()
        {
            var playerTransform = PlayerLifecycleController.Instance.transform;
            foreach (var enemy in _enemies)
            {
                if (PlayerLifecycleController.Instance.Dead) return;
                if(!enemy.gameObject.activeSelf) continue;
                
                var wasVisible = enemy.playerVisible;
                enemy.CheckForPlayer(playerTransform);

                if (wasVisible != enemy.playerVisible)
                {
                    if(enemy.playerVisible) enemy.OnPlayerNoticed();
                    else enemy.OnPlayerEscaped();
                }
                
                if (enemy.playerVisible)
                {
                    enemy.PlayerVisible(playerTransform);
                }
                else
                {
                    enemy.Idle();
                }
            }
        }

        private void OnDestroy()
        {
            CoreEvents.OnGameReset -= GameReset;
            CoreEvents.OnPlayerReady -= PlayerReady;
            CoreEvents.OnPlayerKilled -= PlayerKilled;
        }

        #endregion

        #region Enemies lifecycle
        
        private void GameReset()
        {
            foreach (var enemy in _enemies)
            {
                enemy.gameObject.SetActive(true);
                enemy.OnGameReset();
            }
        }
        
        private void PlayerReady()
        {
            foreach (var enemy in _enemies)
            {
                enemy.OnReady();
            }
        }
        
        private void PlayerKilled()
        {
            foreach (var enemy in _enemies)
            {
                enemy.OnPlayerKilled();
            }
        }

        #endregion
    }
}