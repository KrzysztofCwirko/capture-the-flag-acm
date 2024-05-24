using System.Collections;
using System.Collections.Generic;
using _Scripts.Core;
using _Scripts.Player;
using _Scripts.Utility;
using UnityEngine;

namespace _Scripts.Enemy
{
    public class EnemyBehaviourController : StaticInstance<EnemyBehaviourController>
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
            GameCore.GetEnemyByTransform = GetEnemyByTransform;
            GameCore.OnGameReset += GameReset;
            GameCore.OnPlayerReady += PlayerReady;
            GameCore.OnPlayerKilled += PlayerKilled;
            GameCore.OnFlagTaken += OnFlagTaken;

            yield return null;  //wait for PrefabPooler to finish spawning
            foreach (var enemyType in enemyTypes)
            {
                foreach (Transform spawnPosition in enemyType.parent)
                {
                    var enemy = (Enemy)PrefabPooler.Instance.Pool(enemyType.type, spawnPosition.position,  spawnPosition.rotation, enemyType.parent);
                    enemy.Init();
                    _enemies.Add(enemy);
                }
            }
        }

        private void Update()
        {
            if(GameCore.GamePaused) return;
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

        protected override void OnDestroy()
        {
            base.OnDestroy();
            GameCore.OnGameReset -= GameReset;
            GameCore.OnPlayerReady -= PlayerReady;
            GameCore.OnPlayerKilled -= PlayerKilled;
            GameCore.OnFlagTaken -= OnFlagTaken;
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
           //for example force all enemies to see the player
        }

        #endregion

        #region Sharing data

        private Enemy GetEnemyByTransform(Transform target)
        {
            return _enemies.Find(e => e.transform == target);
        }

        #endregion
    }
}