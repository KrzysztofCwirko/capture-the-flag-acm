using System;
using _Scripts.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Player
{
    /// <summary>
    /// Control player movement
    /// </summary>
    public class PlayerMovementController : MonoBehaviour
    {
        #region Editor properties

        [Header("Setup")]
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Transform groundedCheck;
        
        [Header("Settings")]
        [SerializeField] private float movingSpeed;
        [SerializeField] private float lookSpeed;
        [SerializeField] private float jumpForce;
        [SerializeField] private float runSpeedMultiplier;
        
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
            PlayerLifecycleController.OnPlayerReady += ResetPosition;
            PlayerLifecycleController.OnGameReset += ResetPosition;
            PlayerLifecycleController.OnPlayerKilled += PlayerKilled;
        }
        
        private void Update()
        {
            _isGrounded = IsGrounded();
            ResolveRotation();
            ResolveMovement();
            ResolveGravity();
        }

        private void OnDestroy()
        {
            PlayerLifecycleController.OnPlayerReady -= ResetPosition;
            PlayerLifecycleController.OnGameReset -= ResetPosition;
            PlayerLifecycleController.OnPlayerKilled -= PlayerKilled;
        }

        #endregion

        #region Resolve Controlls

        private void ResolveRotation()
        {
            playerCamera.transform.localRotation = Quaternion.Euler(_currentRotation.OnlyX());
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
            _currentRotation = _currentRotation.ClampX(-45, 45);
        }

        public void OnJump(InputAction.CallbackContext context)
        {   
            if(!_isGrounded || !context.started) return;
            _currentGravityForce += jumpForce;
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
            
            var ray = new Ray(groundedCheck.position, groundedCheck.up);
            if (!Physics.SphereCast(ray, .75f, out var hit, 2f)) return false;
            return hit.distance < 0.4f;
        }

        #endregion

        #region Lifecycle

        /// <summary>
        /// Move player to the spawnPoint
        /// </summary>
        private void ResetPosition()
        {
            transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            enabled = true;
        }
        
        private void PlayerKilled()
        {
            _isGrounded = true;
            _currentLookVector = Vector3.zero;
            _currentMoveVector = Vector3.zero;
            _currentGravityForce = 0f;
            _currentMoveSpeedMultiplier = 1f;
            enabled = false;
        }

        #endregion
    }
}
