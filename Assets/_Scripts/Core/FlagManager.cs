using System;
using UnityEngine;

namespace _Scripts.Core
{
    public class FlagManager : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private Transform flagPoint;
        [SerializeField] private Transform flag;
        [SerializeField] private Collider flagCollider;
        [SerializeField] private GameObject flagIndicator;

        #region Event functions

        private void Start()
        { 
            CoreEvents.OnFlagTaken += FlagTaken;
            CoreEvents.OnFlagLost += ResetFlag;
            CoreEvents.OnGameReset += GameReset;
            CoreEvents.OnFlagDelivered += FlagDelivered;
        }

        private void OnDestroy()
        {
            CoreEvents.OnFlagTaken -= FlagTaken;
            CoreEvents.OnFlagLost -= ResetFlag;
            CoreEvents.OnGameReset -= GameReset;
            CoreEvents.OnFlagDelivered -= FlagDelivered;
        }

        #endregion

        #region Flag lifecycle
        
        private void FlagTaken()
        {
            flagCollider.enabled = false;
            flagIndicator.SetActive(true);
        }

        private void GameReset()
        {
            ResetFlag();
        }

        private void ResetFlag()
        {
            flagIndicator.SetActive(false);
            flag.SetParent(flagPoint);
            flag.localPosition = Vector3.zero;
            flag.localRotation = Quaternion.identity;
            flagCollider.enabled = true;
        }
        
        private void FlagDelivered()
        {
            flagCollider.enabled = false;
            flagIndicator.SetActive(false);
        }

        #endregion
    }
}