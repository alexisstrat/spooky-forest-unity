using Spooky_Forest.Scripts.Data;
using UnityEngine;

namespace Spooky_Forest.Scripts.Mover
{
    public class MoverSkeleton : MoverBase 
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
                _animator.Play("WalkSlap");
            }
        }

        // Fired from animation WalkSlap with Event
        // ReSharper disable once UnusedMember.Local
        void DefaultAnimation(string walkAnim)
        {
            _animator.Play(walkAnim);
        }
    }
}
