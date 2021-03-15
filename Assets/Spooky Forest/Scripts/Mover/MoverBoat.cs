using DG.Tweening;
using Spooky_Forest.Scripts.Data;
using Spooky_Forest.Scripts.Manager;
using UnityEngine;

namespace Spooky_Forest.Scripts.Mover
{
    public class MoverBoat : MoverBase
    {
        public AudioClip boatClip;
        
        [Header("Swing Parameters")]
        [Range(0, 5)]
        public int vibrato = 5;
        [Range(0, 1)]
        public float elasticity = 1f;

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
        
        // When inside collider speed up the movement
        protected override void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Constants.Tags.Collider))
            {
                speed = MaxSpeed;
            }
        }
        
        // When outside collider, reset the movement speed
        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(Constants.Tags.Collider))
            {
                speed = NormalSpeed;
            }
        }

        // Swing rotate boat on X axis when player steps on
        public override void AnimationOnPlayerCollision()
        {
            StartCoroutine(AudioManager.Get.PlaySfx(boatClip));

            transform.DOPunchRotation(new Vector3(Rotation, 0, 0f), 2f, vibrato, elasticity);
        }
    }
}
