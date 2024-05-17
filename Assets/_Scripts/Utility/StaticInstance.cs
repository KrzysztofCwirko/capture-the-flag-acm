using UnityEngine;

namespace _Scripts.Utility
{
    /// <summary>
    /// A static instance is similar to a singleton, but instead of destroying any new
    /// instances, it overrides the current instance. This is handy for resetting the state
    /// and saves you doing it manually
    /// </summary>
    public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; set; }
        
        protected virtual void Awake()
        {
            Instance = this as T;
            Destroyed = false;
        }

        protected bool Destroyed;
        
        protected virtual void OnApplicationQuit() {
            Instance = null;
            
            if(Destroyed) return;
            Destroyed = true;
            Destroy(gameObject);
        }

        protected virtual void OnDestroy()
        {
            Destroyed = true;
            
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }

    /// <summary>
    /// This transforms the static instance into a basic singleton. This will destroy any new
    /// versions created, leaving the original instance intact
    /// </summary>
    public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour {
        protected override void Awake() {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            base.Awake();
        }
    }

    /// <summary>
    /// Finally we have a persistent version of the singleton. This will survive through scene
    /// loads. Perfect for system classes which require stateful, persistent data. Or audio sources
    /// where music plays through loading screens, etc
    /// </summary>
    public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        protected override void Awake() {
            base.Awake();
            DontDestroyOnLoad(!root ? transform.root.gameObject : root);
        }
    }
}