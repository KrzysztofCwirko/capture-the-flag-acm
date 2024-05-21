using System;
using _Scripts.Core;
using _Scripts.Player;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

        private void Start()
        {
            CoreEvents.OnFlagDelivered += FlagDelivered;
            CoreEvents.OnGameLost += GameLost;
            
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            CoreEvents.OnFlagDelivered -= FlagDelivered;
            CoreEvents.OnGameLost -= GameLost;
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
            var timeSpan = TimeSpan.FromSeconds(CoreEvents.GameTime);
            timeText.text = $"Time: {timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";

            foreach (var color in resultColors)
            {
                color.color = won ? new Color(0f, 149/255f,1f, .8f) : new Color(1f, 23/255f, 0f, .8f);
            }
        }
    }
}