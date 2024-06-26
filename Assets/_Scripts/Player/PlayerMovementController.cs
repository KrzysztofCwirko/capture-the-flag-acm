using _Scripts.Core;
using _Scripts.Utility;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace _Scripts.Player
{
    /// <summary>
    /// Control player movement
    /// </summary>
    public class PlayerMovementController : MonoBehaviour
    {
        #region Editor properties

        [Header("Setup")]
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Transform playerCamera;
        [SerializeField] private Transform groundedCheck;
        [SerializeField] private Transform flagHolder;
        
        [Header("Settings")]
        [SerializeField] private float movingSpeed;
        [SerializeField] private float lookSpeed;
        [SerializeField] private float jumpForce;
        [SerializeField] private float runSpeedMultiplier;
        
        [Header("Death")]
        [SerializeField] private Volume volume;
        
        #endregion

        #region Private properties
        
        private Vector3 _currentMoveVector;
        private float _currentMoveSpeedMultiplier = 1f;
        
        private Vector3 _currentLookVector;
        private Vector3 _currentRotation;
        
        private float _currentGravityForce;
        private bool _isGrounded;

        #endregion
        
        #region Event functions

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            GameCore.OnPlayerReady += ActivateInputAndResetPosition;
            GameCore.OnGameReset += GameReset;
            GameCore.OnPlayerKilled += DeactivateInput;
            GameCore.OnShakeCamera += ShakeCamera;
        }

        private void Update()
        {
            if(Time.timeScale <= 0.001f) return;    //paused
            GameCore.GameTime += Time.deltaTime;
            _isGrounded = IsGrounded();
            ResolveRotation();
            ResolveMovement();
            ResolveGravity();
        }

        private void OnDestroy()
        {
            GameCore.OnPlayerReady -= ActivateInputAndResetPosition;
            GameCore.OnGameReset -= GameReset;
            GameCore.OnPlayerKilled -= DeactivateInput;
            GameCore.OnShakeCamera -= ShakeCamera;
        }

        #endregion

        #region Resolve Controlls

        private void ResolveRotation()
        {
            playerCamera.localRotation = Quaternion.Euler(_currentRotation.OnlyX());
            transform.eulerAngles = _currentRotation.OnlyY();
        }

        private void ResolveMovement()
        {
            _currentMoveVector.y = _currentGravityForce;
            characterController.Move( transform.rotation * _currentMoveVector * (_currentMoveSpeedMultiplier * movingSpeed * Time.deltaTime));
        }

        private void ResolveGravity()
        {
            if (_isGrounded)
            {
                _currentGravityForce = 0f;
                return;
            }

            _currentGravityForce += Physics.gravity.y * Time.deltaTime;
        }

        #endregion


        #region Input

        public void OnMove(InputAction.CallbackContext context)
        {
            if(context.started) return;
            var input = context.ReadValue<Vector2>();
            _currentMoveVector = new Vector3(input.x, 0f, input.y);
        }
        
        public void OnLook(InputAction.CallbackContext context)
        {
            if(!context.started) return;
            var input = context.ReadValue<Vector2>();
            _currentLookVector = new Vector3(-input.y, input.x);
            _currentRotation += _currentLookVector * (Time.deltaTime * lookSpeed);
            _currentRotation = _currentRotation.ClampX(-75, 75);
        }

        public void OnJump(InputAction.CallbackContext context)
        {   
            if(!_isGrounded || !context.started) return;
            _currentGravityForce += jumpForce;
            EffectsManager.Instance.PlaySoundEffect("jump", transform.position);
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            _currentMoveSpeedMultiplier = context.canceled ? 1f : runSpeedMultiplier;
        }

        #endregion

        #region Is Grounded Fix

        private bool IsGrounded()
        {
            if (_currentGravityForce > 0.001f) return false;

            if (!Physics.SphereCast(transform.position + characterController.center, characterController.radius,
                    Vector3.down, out var hit, characterController.height / 2f + 1f)) return false;
            return hit.distance < groundedCheck.localPosition.y;
        }

        #endregion

        #region Lifecycle

        /// <summary>
        /// Move player to the spawnPoint
        /// </summary>
        private void ActivateInputAndResetPosition()
        {
            if(volume.profile.TryGet(out FilmGrain grain))
            {
                grain.active = false;
            }
            
            transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            playerInput.ActivateInput();
            enabled = true;
            characterController.enabled = true;
        }
        
        private void GameReset()
        {
            DeactivateInput();
            ActivateInputAndResetPosition();
        }
        
        private void DeactivateInput()
        {
            if(volume.profile.TryGet(out FilmGrain grain))
            {
                grain.active = true;
            }
            
            playerInput.DeactivateInput();
            characterController.enabled = false;
            enabled = false;
            _isGrounded = true;
            _currentLookVector = Vector3.zero;
            _currentMoveVector = Vector3.zero;
            _currentGravityForce = 0f;
            _currentMoveSpeedMultiplier = 1f;
        }
        
        #endregion

        #region Flag

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if(!hit.collider.CompareTag("Flag")) return;

            if (flagHolder.childCount == 1)
            {
                //already has the Flag
                GameCore.OnFlagDelivered?.Invoke();
                return;
            }
            
            GameCore.OnFlagTaken?.Invoke();
            hit.transform.SetParent(flagHolder);
            hit.transform.localPosition = Vector3.zero;
            hit.transform.localRotation = Quaternion.identity;
        }

        #endregion

        #region Camera shake

        private void ShakeCamera(Vector3 source, float strength, float duration)
        {
            const float maxStrengthDistance = 3f;
            var distance = Mathf.Max(Vector3.Distance(source, transform.position), maxStrengthDistance);
            var percent = maxStrengthDistance / distance;
            playerCamera.DOKill(true);
            playerCamera.DOShakePosition(duration, strength * percent);
        }

        #endregion
    }
}
