using System;
using _Scripts.Core;
using _Scripts.Player.Arsenal;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace _Scripts.Player
{
    public class PlayerArsenalController : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private Weapon[] weapons;
        [SerializeField] private Transform firingRoot;

        private int _currentWeapon;
        private bool _isFiring;

        private Weapon CurrentWeapon => weapons[_currentWeapon];

        #region Event functions

        private void Start()
        {
            _currentWeapon = 1;
            ChangeWeapon(0);

            GameCore.OnPlayerKilled += PlayerKilled;
            GameCore.OnPlayerReady += PlayerReady;
        }

        private void Update()
        {
            if(Time.timeScale <= 0.001f) return; //pause
            CurrentWeapon.TimeToFire += Time.deltaTime;
            
            if (!_isFiring || !CurrentWeapon.CanFire())
            {           
                return;
            }

            CurrentWeapon.TimeToFire = 0f;
            Fire();
        }

        private void OnDestroy()
        {
            GameCore.OnPlayerKilled -= PlayerKilled;
            GameCore.OnPlayerReady -= PlayerReady;
        }

        #endregion

        #region Aresnal Management

        public void SetToFirst(InputAction.CallbackContext context)
        {
            if(!context.started) return;
            ChangeWeapon(0);
        }
        
        public void SetToSecond(InputAction.CallbackContext context)
        {
            if(!context.started) return;
            ChangeWeapon(1);
        }
        
        public void SetToThird(InputAction.CallbackContext context)
        {
            if(!context.started) return;
            ChangeWeapon(2);
        }

        private void ChangeWeapon(int target)
        {
            if(_currentWeapon == target) return;
            CurrentWeapon.prefab.SetActive(false);
            _currentWeapon = target;
            CurrentWeapon.prefab.SetActive(true);
        }

        #endregion

        #region Firing

        public void SetFiring(InputAction.CallbackContext context)
        {
            _isFiring = context.performed;
        }
        
        private void Fire()
        {
            var oneDirectionDuration = 1f / (CurrentWeapon.fireRate * 2f);
            DOTween.Sequence().Append(CurrentWeapon.prefab.transform
                .DOLocalMove(CurrentWeapon.moveOnAttack, oneDirectionDuration
                    )
                .SetRelative(true).SetLoops(2, LoopType.Yoyo)).Join(CurrentWeapon.prefab.transform
                .DOLocalRotate(CurrentWeapon.rotateOnAttack,
                    oneDirectionDuration)
                .SetRelative(true).SetLoops(2, LoopType.Yoyo));
            

            var position = firingRoot.position;
            EffectsManager.Instance.PlaySoundEffect(CurrentWeapon.shootingClipName, position);
            
            // GameCore.OnShakeCamera?.Invoke(position, .2f, oneDirectionDuration*2f);
            EffectsManager.Instance.ShowParticle("Shoot",
                firingRoot.InverseTransformPoint(CurrentWeapon.shootingParticlePosition.position) + Random.insideUnitSphere/10f, 3);
            
            var ray = new Ray(position, firingRoot.forward);
            if (!Physics.Raycast(ray, out var hit, CurrentWeapon.maxDistance))
            {
                EffectsManager.Instance.ShowParticle("BulletMiss", ray.origin + ray.direction*CurrentWeapon.maxDistance);
                return;
            }
            EffectsManager.Instance.ShowParticle("BulletHit", hit.point + hit.normal * 0.01f, rotation:Quaternion.LookRotation(-hit.normal).eulerAngles);
            if(!hit.collider.CompareTag("Enemy")) return;
            //colliders on enemies are their direct children
            var enemyHit = GameCore.GetEnemyByTransform(hit.transform.parent);
            enemyHit.KillMe();
        }

        #endregion

        #region Lifecycle

        private void PlayerReady()
        {
           CurrentWeapon.prefab.SetActive(true);
        }

        private void PlayerKilled()
        {
            CurrentWeapon.prefab.SetActive(false);
        }

        #endregion
    }
}
