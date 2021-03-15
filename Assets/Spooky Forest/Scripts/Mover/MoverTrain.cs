using Spooky_Forest.Scripts.Data;
using Spooky_Forest.Scripts.Manager;
using UnityEngine;

namespace Spooky_Forest.Scripts.Mover
{
    public class MoverTrain : MoverBase
    {
        public AudioClip trainAudio;
        
        private GameObject _player;
        private bool _hitPlayer;
        private bool _audioPlayed = false;
    
        protected override void Update () {
            transform.Translate(speed * Time.deltaTime, 0, 0);

            if(!_audioPlayed)
                CheckIfInView();
            
            ReachedEnd();
        }

        private void CheckIfInView()
        {
            var screenPosition = GameManager.Get.mainCamera.WorldToScreenPoint(transform.position);

            if (screenPosition.x > 0 && screenPosition.x < Screen.width &&
                screenPosition.y > 0 && screenPosition.y < Screen.height)
            {
                StartCoroutine(AudioManager.Get.PlaySfxWithVolumeCoroutine(trainAudio, .2f));
                _audioPlayed = true;
            }
        }
        
        protected override void ReachedEnd()
        {
            if (Mathf.Abs(transform.position.x) >= Mathf.Abs(endPos.x))
            {
                if (_hitPlayer)
                {
                    _player.transform.parent = null;
                    _hitPlayer = false;
                    _player = null;
                }
                _audioPlayed = false;
                gameObject.SetActive(false);
            }
        }

        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            if (other.CompareTag(Constants.Tags.Player))
            {
                other.transform.parent = transform;
                _player = other.transform.gameObject;
                _hitPlayer = true;
            }
        }
    }
}
