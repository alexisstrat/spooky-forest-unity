using System.Collections;
using DG.Tweening;
using Spooky_Forest.Scripts.Manager;
using UnityEngine;

namespace Spooky_Forest.Scripts
{
    public class CameraBehaviour : MonoBehaviour
    {
        [Tooltip("Check this to automove camera")]
        public bool automove = true;
    
        [Header("Camera is following this object...")]
        [Tooltip("The target to follow")]
        public Transform target;
    
        [Header("...with this offset")]
        [Tooltip("Offset from the target")]
        public Vector3 offsetPortrait = new Vector3(1, 5.4f, -2);
        public Vector3 offsetLandscape = new Vector3(2, 5.4f, -3);

        [Header("Camera Portrait Speed Parameters")]
        public float portraitSpeed = 0.7f;
        public float portraitMaxSpeed = 1.5f;
        public float portraitSideWaysSpeed = 2f;
        public float portraitAcceleration = .2f;

        [Header("Camera Landscape Speed Parameters")] 
        [SerializeField] private float landscapeSpeed = 1;
        [SerializeField] private float landscapeMaxSpeed = 1.5f;
        [SerializeField] private float landscapeAcceleration = 0.05f;

        [Header("Camera Size")]
        [SerializeField] private int camSizePortrait = 5;
        [SerializeField] private int camSizeLandscape = 3;
    
        [Header("Portrait Camera Borders")]
        [SerializeField] private float leftPortraitX = -0.5f;
        [SerializeField] private float rightPortraitX = 4.0f;

        [Header("Landscape Camera Borders")] 
        [SerializeField] private float leftLandscapeX = -1.5f;
        [SerializeField] private float rightLandscapeX = 2.5f;
    
        [Header("Camera shake variables")]
        public float shakeDuration = .5f;
        public float shakeStrength = .1f;
        public int vibrato = 20;
        public float randomness = 90;

        [Header("Starting position of each orientation")]
        public Vector3 startPosPortrait = new Vector3(1, 5.4f, -4);
        public Vector3 startPosLandscape = new Vector3(2, 5.4f, -5);
    
        public delegate void CameraReadyDelegate();
        public event CameraReadyDelegate CameraReadyEvent;

        private float _cameraSpeed;         // The speed that the camera moves
        private float _cameraMaxSpeed;      // The maximum speed to catch up with the player
        private float _cameraSidewaysSpeed; // The speed in which moves sideways
        private float _speedAcceleration;   // The acceleration to reach the player
        private float _xBoundaryLeft;       // Left boundary that camera stops moving upon reaching that value
        private float _xBoundaryRight;      // Right boundary that camera stops moving upon reaching that value
        private Vector3 _offsetFromPlayer;  // The offset from the player
    
        private void Start()
        {
            StartCoroutine(SetCameraParameters());
        }

        /// <summary>
        /// Moves the camera if automove is set. Speeds up when player is further into
        /// the scene otherwise just moves in a constant speed
        /// </summary>
        void Update()
        {
            if(!GameManager.Get.CanPlay()) return;

            if (automove)
            {
                if (transform.position.z + Mathf.Abs(_offsetFromPlayer.z) < target.position.z)
                {
                    _cameraSpeed = Mathf.Lerp(_cameraSpeed, _cameraMaxSpeed, _speedAcceleration * Time.deltaTime);
                    transform.position = Vector3.Lerp(transform.position,
                        new Vector3(Mathf.Clamp(target.position.x + _offsetFromPlayer.x, _xBoundaryLeft, _xBoundaryRight), _offsetFromPlayer.y, target.position.z), 
                        _cameraSpeed * Time.deltaTime);
                }
                else
                {
                    _cameraSpeed = Mathf.Lerp((float) _cameraSpeed, 0.7f, _speedAcceleration*Time.deltaTime);
                    var newX = Mathf.Lerp(transform.position.x, target.position.x + _offsetFromPlayer.x,
                        _cameraSidewaysSpeed * Time.deltaTime);
                
                    transform.position = new Vector3(Mathf.Clamp(newX, _xBoundaryLeft, _xBoundaryRight), _offsetFromPlayer.y, _cameraSpeed * Time.deltaTime + transform.position.z);
                }
            }
        }

        /// <summary>
        /// Updates the camera boundaries in case of device rotation
        /// </summary>
        public void UpdateCameraPositionAndBoundaries(ScreenOrientation screenOrientation)
        {

            if (GameManager.Get.canPlay)
            {
                if (screenOrientation == ScreenOrientation.Portrait)
                {
                    SetPortraitParameters();
                }
                else
                {
                    SetLandscapeParameters();
                }
            
                transform.position = new Vector3(_offsetFromPlayer.x, transform.position.y, transform.position.z - _offsetFromPlayer.z);

            }
            else
            {
                SetCameraParameters();
            }
        }
    
        /// <summary>
        /// Shakes the Camera when player dies
        /// </summary>
        public void Shake()
        {
            GameManager.Get.mainCamera.DOShakePosition(shakeDuration, shakeStrength, vibrato, randomness,true).OnComplete(GameManager.Get.GuiGameOver);
        }

        /// <summary>
        /// On game start/restart we set the camera position and bounds depending on orientation
        /// When all set, send an event
        /// </summary>
        public IEnumerator SetCameraParameters()
        {
            yield return new WaitForEndOfFrame();
            automove = false;
            if (DeviceOrientationManager.CurrentOrientation == ScreenOrientation.Portrait)
            {
                SetPortraitParameters();
            }
            else
            {
                SetLandscapeParameters();
            }
            
            if (!GameManager.Get.canPlay && !GameManager.Get.gameOver)
            {
                transform.position = new Vector3(target.position.x + _offsetFromPlayer.x,
                    transform.position.y, target.position.z + _offsetFromPlayer.z);
            }
            // On game over
            else if (GameManager.Get.gameOver)
            {
                var xOffset = DeviceOrientationManager.CurrentOrientation == ScreenOrientation.Portrait ? -1 : 1;
                transform.position = new Vector3(Mathf.Clamp(transform.position.x + xOffset, _xBoundaryLeft, _xBoundaryRight),
                    transform.position.y, transform.position.z);
            }
            // On playing
            else
            {
                var zOffset = DeviceOrientationManager.CurrentOrientation == ScreenOrientation.Portrait ? 1 : -1;
                transform.position = new Vector3( Mathf.Clamp(_offsetFromPlayer.x + target.position.x, _xBoundaryLeft, _xBoundaryRight),
                    transform.position.y , transform.position.z + zOffset );
            }
                
            automove = true;

            CameraReady();
        }

        /// <summary>
        /// Values for portrait orientation regarding boundaries, camera size, offset and speed
        /// </summary>
        private void SetPortraitParameters()
        {
            _xBoundaryLeft = leftPortraitX;
            _xBoundaryRight = rightPortraitX;
        
            GameManager.Get.mainCamera.orthographicSize = camSizePortrait;

            _offsetFromPlayer = offsetPortrait;
        
            _speedAcceleration = portraitAcceleration;
            _cameraMaxSpeed = portraitMaxSpeed;
            _cameraSpeed = portraitSpeed;
            _cameraSidewaysSpeed = portraitSideWaysSpeed;
        }

        /// <summary>
        /// Values for landscape orientation regarding boundaries, camera size, offset and speed
        /// </summary>
        private void SetLandscapeParameters()
        {
            _xBoundaryLeft = leftLandscapeX;
            _xBoundaryRight = rightLandscapeX;
            GameManager.Get.mainCamera.orthographicSize = camSizeLandscape;

            _offsetFromPlayer = offsetLandscape;

            _speedAcceleration = landscapeAcceleration;
            _cameraMaxSpeed = landscapeMaxSpeed;
            _cameraSpeed = landscapeSpeed; ;
        }

        /// <summary>
        /// This function invokes the event to broadcast that the camera is ready
        /// </summary>
        private void CameraReady()
        {
            CameraReadyEvent?.Invoke();
        }
    }
}
