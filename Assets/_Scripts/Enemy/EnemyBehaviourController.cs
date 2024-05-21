using System.Collections;
using System.Collections.Generic;
using _Scripts.Player;
using _Scripts.Utility;
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

        private readonly List<Enemy> _enemies = new List<Enemy>();

        #endregion
        
        #region Event functions

        private IEnumerator Start()
        {
            CoreEvents.OnGameReset += GameReset;
            CoreEvents.OnPlayerReady += PlayerReady;
            CoreEvents.OnPlayerKilled += PlayerKilled;
            CoreEvents.OnFlagTaken += OnFlagTaken;

            yield return null;  //wait for PrefabPooler to finish spawning
            foreach (var enemyType in enemyTypes)
            {
                foreach (var spawnPosition in enemyType.positions)
                {
                    var enemy = (Enemy)PrefabPooler.Instance.Pool(enemyType.type, spawnPosition.position,  spawnPosition.rotation, transform);
                    enemy.Init();
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
                enemy.TryToFindPlayer(playerTransform);

                if (wasVisible != enemy.playerVisible)
                {
                    if(enemy.playerVisible) enemy.OnPlayerNoticed();
                    else enemy.OnPlayerEscaped();
                }
                
                if (enemy.playerVisible)
                {
                    enemy.ChasePlayerLoop(playerTransform);
                }
                else
                {
                    enemy.IdleLoop();
                }
            }
        }

        private void OnDestroy()
        {
            CoreEvents.OnGameReset -= GameReset;
            CoreEvents.OnPlayerReady -= PlayerReady;
            CoreEvents.OnPlayerKilled -= PlayerKilled;
            CoreEvents.OnFlagTaken -= OnFlagTaken;
        }

        #endregion

        #region Enemies lifecycle
        
        private void GameReset()
        {
            foreach (var enemy in _enemies)
            {
                enemy.gameObject.SetActive(true);
                enemy.OnRespawn();
            }
        }
        
        private void PlayerReady()
        {
            foreach (var enemy in _enemies)
            {
                enemy.OnPlayerReady();
            }
        }
        
        private void PlayerKilled()
        {
            foreach (var enemy in _enemies)
            {
                enemy.OnPlayerKilled();
            }
        }
        
        private void OnFlagTaken()
        {
           
        }

        #endregion
    }
}