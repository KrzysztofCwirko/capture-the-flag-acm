using _Scripts.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.UI
{
    public class FreezeGameUI : MonoBehaviour
    {
        private static GameObject _visible;
        
        private void OnEnable()
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
        }

        private void OnDisable()
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void ResetGame()
        {
            CoreEvents.OnGameReset?.Invoke();
            SwitchActiveSelf();
        }

        public void SwitchActiveSelf()
        {
            var o = gameObject;
            if(_visible != default && _visible != o) return;
            var targetVisibility = !o.activeSelf;
            gameObject.SetActive(targetVisibility);
            _visible = targetVisibility ? o : default;
        }
        
        public void GoToMainMenu()
        {
            SceneManager.LoadSceneAsync("Menu");
        }
    }
}