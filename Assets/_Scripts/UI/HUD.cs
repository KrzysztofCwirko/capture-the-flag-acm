using _Scripts.Core;
using _Scripts.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class HUD : MonoBehaviour
    {
        [Header("Setup")] 
        [SerializeField] private TMP_Text playerName;
        [SerializeField] private TMP_Text time;
        [SerializeField] private MaskableGraphic flagIcon;
        [SerializeField] private MaskableGraphic flagHolder;
        [SerializeField] private SummaryScreen summaryScreen;

        #region Event functions

        private void Start()
        {
            summaryScreen.Init();
            playerName.text = GameCore.PlayerName;
            time.text = "00:00:00";
 
            FlagLost();
            GameCore.OnFlagTaken += FlagTaken;
            GameCore.OnGameReset += FlagLost;
            GameCore.OnFlagLost += FlagLost;
        }

        private void Update()
        {
            time.text = GameCore.GameTime.ToTime();
        }

        private void OnDestroy()
        {
            GameCore.OnFlagTaken -= FlagTaken;
            GameCore.OnFlagLost -= FlagLost;
            GameCore.OnGameReset -= FlagLost;
        }

        #endregion
        
        private void FlagLost()
        {
            flagIcon.color = Color.grey;
            flagHolder.color = Color.white;
        }

        private void FlagTaken()
        {
            flagIcon.color = Color.white;
            flagHolder.color =  Color.red;
        }
    }
}
