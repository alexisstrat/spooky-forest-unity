using System.Collections;
using DG.Tweening;
using Spooky_Forest.Scripts.Data;
using Spooky_Forest.Scripts.Manager;
using UnityEngine;

namespace Spooky_Forest.Scripts.Mover
{
    public class MoverLog : MoverBase {

        public AudioClip logClip;

        private float dippingInWater = -.05f;
        private float dippingSpeed = .2f;

        protected override void ReachedEnd()
        {
            if (Mathf.Abs(transform.position.x) >= Mathf.Abs(endPos.x))
            {
                if (coinParented != null)
                {
                    ResetCoin();
                }
                gameObject.SetActive(false);
            }
        }
        
        protected override void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Constants.Tags.Collider))
            {
                speed = MaxSpeed;
            }
        }

        private IEnumerator DipInto()
        {
            float y = transform.position.y;
            while (y > dippingInWater)
            {
                y = Mathf.Lerp(y, dippingInWater, Time.deltaTime * dippingSpeed * Time.deltaTime);
                transform.position = new Vector3(transform.position.x, y, transform.position.z);
                yield return null;
            }
            transform.position = new Vector3(transform.position.x, dippingInWater, transform.position.z);
            //StartCoroutine(DipOut());
        }
    
        private IEnumerator DipOut()
        {
            float y = transform.position.y;
            while (y < -dippingInWater)
            {
                y = Mathf.Lerp(y, -dippingInWater, Time.deltaTime * dippingSpeed);
                transform.position = new Vector3(transform.position.x, y, transform.position.z);
                yield return null;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(Constants.Tags.Collider))
            {
                speed = NormalSpeed;
            }
        }
    
        public override void AnimationOnPlayerCollision()
        {
            StartCoroutine(AudioManager.Get.PlaySfx(logClip));
            transform.DOBlendableMoveBy(new Vector3(0, dippingInWater, 0),
                dippingSpeed).OnComplete(AnimationOnPlayerExit);
        }

        public override void AnimationOnPlayerExit()
        {
            transform.DOBlendableMoveBy(new Vector3(0, -dippingInWater, 0), dippingSpeed);
        }

    }
}