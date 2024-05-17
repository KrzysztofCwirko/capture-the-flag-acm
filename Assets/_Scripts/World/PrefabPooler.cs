using System;
using System.Collections.Generic;
using _Scripts.Utility;
using UnityEngine;

namespace _Scripts.World
{
    public class PrefabPooler : StaticInstance<PrefabPooler>
    {
        #region Public settings

        [Header("Setup")] 
        [SerializeField] private List<PoolSettings> availablePrefabs;

        #endregion

        #region Private properties

        private readonly Dictionary<MonoBehaviour, Queue<MonoBehaviour>> _spawnedPrefabs =
            new Dictionary<MonoBehaviour, Queue<MonoBehaviour>>();

        #endregion

        #region Event functions

        private void Start()
        {
            foreach (var availablePrefab in availablePrefabs)
            {
                _spawnedPrefabs.Add(availablePrefab.target, new Queue<MonoBehaviour>());

                for (var i = 0; i <availablePrefab.count; i++)
                {
                    var spawn = Instantiate(availablePrefab.target);
                    _spawnedPrefabs[availablePrefab.target].Enqueue(spawn);
                    spawn.gameObject.SetActive(false);
                }
            }
        }

        #endregion
        
        #region Pooling

        public MonoBehaviour Pool(MonoBehaviour target)
        {
            var next = _spawnedPrefabs[target].Dequeue();
            _spawnedPrefabs[target].Enqueue(next);
            next.gameObject.SetActive(true);
            return next;
        }

        #endregion
    }
}