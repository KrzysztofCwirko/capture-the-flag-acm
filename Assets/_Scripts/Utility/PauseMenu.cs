using System;
using UnityEngine;

namespace _Scripts.Utility
{
    public class PauseMenu : MonoBehaviour
    {
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
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}
