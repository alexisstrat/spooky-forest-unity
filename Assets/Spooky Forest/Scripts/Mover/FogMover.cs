using Spooky_Forest.Scripts.Manager;
using UnityEngine;

namespace Spooky_Forest.Scripts.Mover
{
    public class FogMover : MonoBehaviour
    {
        private Transform _player;

        private void Start()
        {
            _player = GameManager.Get.player.transform;
        }

        private void Update()
        {
            if (GameManager.Get.CanPlay())
            {
                UpdatePosition();
            }
        }

        public void UpdatePosition()
        {
            var pos = transform.position;
            if (_player.transform.position.z > pos.z + 20)
            {
                transform.position = new Vector3(pos.x, pos.y, pos.z + 40);
            }
        }
    }
}
