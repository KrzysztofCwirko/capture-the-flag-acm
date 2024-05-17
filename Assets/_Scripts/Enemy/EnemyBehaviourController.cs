using System;
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
            PlayerLifecycleController.OnGameReset += GameReset;
            PlayerLifecycleController.OnPlayerKilled += PlayerKilled;
            PlayerLifecycleController.OnPlayerReady += PlayerReady;

            yield return null;  //wait for PrefabPooler to finish spawning
            foreach (var enemyType in enemyTypes)
            {
                var enemy = (EnemyBase)PrefabPooler.Instance.Pool(enemyType.baseType);
                _enemies.Add(enemy);
            }
        }

        private void Update()
        {
            var playerTransform = PlayerLifecycleController.Instance.transform;
            foreach (var enemy in _enemies)
            {
                if(!enemy.gameObject.activeSelf) continue;
                var wasVisible = enemy.playerVisible;
                enemy.CheckForPlayer();

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
            PlayerLifecycleController.OnGameReset -= GameReset;
            PlayerLifecycleController.OnPlayerKilled -= PlayerKilled;
            PlayerLifecycleController.OnPlayerReady -= PlayerReady;
        }

        #endregion

        #region Enemies lifecycle
        
        private void PlayerKilled()
        {
            foreach (var enemy in _enemies)
            {
                enemy.OnPlayerDied();
            }
        }
        
        private void GameReset()
        {
            foreach (var enemy in _enemies)
            {
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

        #endregion
    }
}