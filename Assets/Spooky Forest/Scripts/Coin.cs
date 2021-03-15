using DG.Tweening;
using Spooky_Forest.Scripts.Data;
using Spooky_Forest.Scripts.Manager;
using UnityEngine;

namespace Spooky_Forest.Scripts
{
    public class Coin : MonoBehaviour {

        public int coinValue = 1;
        public AudioClip audioClip = null;
        private CoinManager _coinManager;
    
        private void Awake()
        {
            _coinManager = GameManager.Get.coinManager;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag(Constants.Tags.Player))
            {
                StartCoroutine(AudioManager.Get.PlaySfx(audioClip));
                GameManager.Get.UpdateCoinCount(coinValue);

                transform.DOScale(0, .2f).OnComplete(Deactivate);
            }
        }

        private void Deactivate()
        {
            transform.gameObject.SetActive(false);
            transform.localScale = Vector3.one;
        }

        private void OnEnable()
        {
            _coinManager.activeCoins.Add(gameObject);
        }

        private void OnDisable()
        {
            _coinManager.activeCoins.Remove(gameObject);
        }
    }
}
