using System.Collections.Generic;
using _Scripts.Utility;
using UnityEngine;

namespace _Scripts.Core
{
    public class PrefabPooler : StaticInstance<PrefabPooler>
    {
        #region Public settings

        [Header("Setup")] 
        [SerializeField] private List<PoolSettings> availablePrefabs;

        #endregion

        #region Private properties

        private readonly Dictionary<Behaviour, Queue<Behaviour>> _spawnedPrefabs =
            new Dictionary<Behaviour, Queue<Behaviour>>();

        #endregion

        #region Event functions

        private void Start()
        {
            foreach (var availablePrefab in availablePrefabs)
            {
                _spawnedPrefabs.Add(availablePrefab.target, new Queue<Behaviour>());

                for (var i = 0; i < availablePrefab.count; i++)
                {
                    var spawn = Instantiate(availablePrefab.target, transform);
                    _spawnedPrefabs[availablePrefab.target].Enqueue(spawn);
                    spawn.gameObject.SetActive(false);
                }
            }
        }

        #endregion
        
        #region Pooling

        public Behaviour Pool(Behaviour target, Vector3 position = new Vector3(), Quaternion rotation = new Quaternion(), Transform parent = null)
        {
            var next = _spawnedPrefabs[target].Dequeue();
            _spawnedPrefabs[target].Enqueue(next);
            next.transform.SetPositionAndRotation(position, rotation);
            next.transform.SetParent(parent);
            next.gameObject.SetActive(true);
            return next;
        }

        #endregion
    }
}