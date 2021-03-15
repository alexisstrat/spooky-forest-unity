using Spooky_Forest.Scripts.Manager;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spooky_Forest.Scripts.Spawner
{
    public class TrainSpawnerController : MovingObjectSpawnerController {

        public GameObject pumpkin = null;
        public GameObject trainPoleLight = null;
        public AudioClip pumpkinScream;

        [Range(0,1)]
        public float screamVolume = 0.1f;

        private Animator _pumpkinAnim;
        private Renderer _pumpkinRenderer;
    
    
        protected override void Awake()
        {
            base.Awake();
            initialObjectsToSpawn = Random.Range(0 ,1);
            _pumpkinAnim = pumpkin.GetComponent<Animator>();
            _pumpkinRenderer = pumpkin.transform.GetChild(0).GetComponent<Renderer>();
        }

        protected override GameObject SpawnObject()
        {
            GameObject obj = ObjectPooler.GetPooledObject();

            if (obj != null)
            {
                obj.transform.position = StartPos.position;
                obj.SetActive(true);
                obj.transform.rotation = Quaternion.Euler(obj.transform.rotation.x, _rot, 0);
            
                trainPoleLight.SetActive(true);
            
                _pumpkinAnim.Play("Pumpkin|OpenMouth");


                if (_pumpkinRenderer.isVisible)
                {
                    StartCoroutine(AudioManager.Get.PlaySfxWithVolumeCoroutine(pumpkinScream, screamVolume));
                }
            }

            return obj;
        }
    }
}
