using System.Linq;
using _Scripts.Core;
using _Scripts.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Menu
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private TMP_InputField playerNameInput;
        [SerializeField] private TMP_Text highscores;

        #region Event functions

        private void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
            playerNameInput.text = GameCore.PlayerName;
            playerNameInput.onValueChanged.AddListener(UserInputChanged);

            var index = 0;
            highscores.text = string.Join("<br><br>",
                Extensions.GetAllScores()?.Take(10)
                    .Select(score =>
                    {
                        index += 1;
                        return $"{score.Item2}<br>{score.Item1.ToTime()}<br>------- {index} -------";
                    }) ??
                new[] { "No saved highscores" });
        }
        
        private void OnDestroy()
        {
            playerNameInput.onValueChanged.RemoveListener(UserInputChanged);
        }

        #endregion

        #region Player name

        private void UserInputChanged(string newName)
        {
            GameCore.PlayerName = newName;
        }

        #endregion

        public void LoadGameScene()
        {
            SceneManager.LoadSceneAsync("Game");
        }
        
        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
