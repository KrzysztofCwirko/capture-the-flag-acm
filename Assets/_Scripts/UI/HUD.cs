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
        [SerializeField] private SummaryScreen summaryScreen;

        #region Event functions

        private void Start()
        {
            summaryScreen.Init();
            playerName.text = PlayerPrefs.GetString("PlayerName", "Bestia");
            time.text = "00:00:00";
            flagIcon.color = Color.grey;
            
            GameCore.OnFlagTaken += FlagTaken;
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
        }

        #endregion
        
        private void FlagLost()
        {
            flagIcon.color = Color.grey;
        }

        private void FlagTaken()
        {
            flagIcon.color = new Color(0f, 149 / 255f, 1f);
        }
    }
}
