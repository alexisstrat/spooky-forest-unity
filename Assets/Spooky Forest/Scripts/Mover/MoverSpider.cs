using Spooky_Forest.Scripts.Data;
using UnityEngine;

namespace Spooky_Forest.Scripts.Mover
{
    public class MoverSpider : MoverBase
    {
        private Animator _animator;

        protected override void Start()
        {
            _animator = GetComponent<Animator>();   
        }
        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            if (other.CompareTag(Constants.Tags.Player))
            {
                speed *= 3f;
                _animator.SetBool("Bite", true);
                DefaultAnimation();
            }
        }

        void DefaultAnimation()
        {
            speed /= 3f;
            _animator.SetBool("Bite", false);
        }
    }
}
