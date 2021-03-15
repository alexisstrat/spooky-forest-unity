using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Spooky_Forest.Scripts.Manager
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager _instance;
        #region Singleton
        public static AudioManager Get
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(AudioManager)) as AudioManager;
                }
                return _instance;
            }
        }
        #endregion
        
        #region Fields

        private AudioSource _ambientSource;
        private AudioSource _playerSfxSource;
        private AudioSource _sfxSource;

        #endregion

        private void Awake()
        {
            AudioListener.volume = 0f;
            _ambientSource = gameObject.AddComponent<AudioSource>();
            _playerSfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource = gameObject.AddComponent<AudioSource>();

            _ambientSource.volume = 0;
            _ambientSource.loop = true;
        }

        public void AudioOnOff(bool on)
        {
            if (on)
            {
                AudioListener.volume = 1f;
                _ambientSource.Play();
            }
            else
            {
                AudioListener.volume = 0;
                _ambientSource.Stop();
                _sfxSource.Stop();
                _playerSfxSource.Stop();
            }
        }

        public void PauseAudio()
        {
            AudioListener.pause = true;
            _ambientSource.Pause();
            _sfxSource.Pause();
            _playerSfxSource.Pause();
        }

        public void ResumeAudio()
        {
            AudioListener.pause = false;
            _ambientSource.UnPause();
            _sfxSource.UnPause();
            _playerSfxSource.UnPause();
        }

        public void PlayAmbient(AudioClip ambientClip)
        {
            _ambientSource.clip = ambientClip;
            _ambientSource.Play();
            TurnUpVolume(_ambientSource);
        }
        
        public void RestartAudio()
        {
            _ambientSource.volume = 0;
            _ambientSource.Stop();
            _sfxSource.Stop();
            _playerSfxSource.Stop();
        }

        public void PlayPlayerSfx(AudioClip playerSfxClip)
        {
            _playerSfxSource.PlayOneShot(playerSfxClip);
        }
        
        public IEnumerator PlaySfx(AudioClip sfxClip)
        {
            _sfxSource.PlayOneShot(sfxClip);
            yield return null;
        }

        public void PlaySfxWithVolume(AudioClip sfxClip, float volume)
        {
            _sfxSource.PlayOneShot(sfxClip, volume);
        }

        public IEnumerator PlaySfxWithVolumeCoroutine(AudioClip sfxClip, float volume)
        {
            _sfxSource.PlayOneShot(sfxClip, volume);
            yield return null;
        }

        public void TurnUpVolume(AudioSource audioSource)
        {
            DOTween.To(() => audioSource.volume, x=> audioSource.volume = x, 0.5f, 0.5f);
        }
        
        public void TurnDownVolume(AudioSource audioSource)
        {
            DOTween.To(() => audioSource.volume, x=> audioSource.volume = x, 0f, 0.5f);
        }
        
    }
}
