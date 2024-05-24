using UnityEngine;

namespace _Scripts.Utility
{
    /// <summary>
    /// A static instance is similar to a singleton
    /// </summary>
    public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }
        
        protected virtual void Awake()
        {
            Instance = this as T;
            _destroyed = false;
        }

        private bool _destroyed;
        
        protected virtual void OnApplicationQuit() {
            Instance = null;
            
            if(_destroyed) return;
            _destroyed = true;
            Destroy(gameObject);
        }

        protected virtual void OnDestroy()
        {
            _destroyed = true;
            
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}