using _Scripts.Core;
using DG.Tweening;
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
            GameCore.OnPlayerKilled += PlayerKilled;
        }
        
        private void OnDisable()
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            GameCore.OnPlayerKilled -= PlayerKilled;
        }
        
        private void PlayerKilled()
        {
            SwitchActiveSelf(); //Summary will pop-in anyway, OnGameLost is invoked after OnPlayerKilled
        }

        public void ResetGame()
        {
            DOTween.KillAll();
            GameCore.OnGameReset?.Invoke();
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
            DOTween.KillAll();
            SceneManager.LoadSceneAsync("Menu");
        }
    }
}