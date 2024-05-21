using _Scripts.Core;
using _Scripts.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class SummaryScreen : FreezeGameUI
    {
        [Header("Setup")]
        [SerializeField] private TMP_Text resultText;
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private GameObject newHighScore;
        [SerializeField] private MaskableGraphic[] resultColors;


        #region Event functions

        public void Init()
        {
            GameCore.OnFlagDelivered += FlagDelivered;
            GameCore.OnGameLost += GameLost;
        }

        private void OnDestroy()
        {
            GameCore.OnFlagDelivered -= FlagDelivered;
            GameCore.OnGameLost -= GameLost;
        }

        #endregion
        
        private void FlagDelivered()
        {
            ShowSummary(true);
        }
        
        private void GameLost()
        {
            ShowSummary(false);
        }

        private void ShowSummary(bool won)
        {
            SwitchActiveSelf();
            resultText.text = won ? "SUCCESS" : "FAILURE";
            timeText.text = $"Time: {GameCore.GameTime.ToTime()}";
            newHighScore.SetActive(won && GameCore.GameTime > Extensions.GetTheHighestScore().Item1);
            
            foreach (var color in resultColors)
            {
                color.color = won ? new Color(0f, 149/255f,1f, .8f) : new Color(1f, 23/255f, 0f, .8f);
            }

            if (won) GameCore.GameTime.SaveScore();
        }
    }
}