using System;
using _Scripts.Player.Arsenal;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Player
{
    public class PlayerArsenalController : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private Weapon[] weapons;

        private int _currentWeapon;
        private bool _isFiring;

        #region Event functions

        private void Start()
        {
            _currentWeapon = 1;
            ChangeWeapon(2);
        }

        private void Update()
        {
            if(Time.timeScale <= 0.001f) return; //pause
            weapons[_currentWeapon].timeToFire += Time.deltaTime;
            
            if (!_isFiring || !weapons[_currentWeapon].CanFire())
            {           
                return;
            }

            weapons[_currentWeapon].timeToFire = 0f;
            Fire();
        }

        #endregion

        #region Aresnal Management

        private void ChangeWeapon(int target)
        {
            if(_currentWeapon == target) return;
            weapons[_currentWeapon].prefab.SetActive(false);
            _currentWeapon = target;
            weapons[_currentWeapon].prefab.SetActive(true);
        }

        #endregion

        #region Firing

        public void SetFiring(InputAction.CallbackContext context)
        {
            _isFiring = context.performed;
        }
        
        private void Fire()
        {
            weapons[_currentWeapon].prefab.transform.DOLocalMoveX(-0.1f, 1f/(weapons[_currentWeapon].fireRate*2f)).SetRelative(true).SetLoops(2, LoopType.Yoyo);

            if (weapons[_currentWeapon].isManual)
            {
                return;
            } 
        }

        #endregion
    }
}
