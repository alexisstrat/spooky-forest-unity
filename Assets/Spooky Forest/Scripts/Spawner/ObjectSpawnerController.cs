using Spooky_Forest.Scripts.Level_Construction;
using Spooky_Forest.Scripts.Manager;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spooky_Forest.Scripts.Spawner
{
    [RequireComponent(typeof(ObjectPooler))]
    public class ObjectSpawnerController : MonoBehaviour
    {
        [Header("Main Settings")] 
        
        public bool poolEverythingOnList;
        public bool shuffleListAfterPooling;
        public bool allowedToSpawnCoinsOnPlatform;

        [Tooltip("Drag here the leftstart from object's hierarchy")]
        public Transform leftStart = null;
    
        [Tooltip("Drag here the rightstart from object's hierarchy")]
        public Transform righStart = null;
        
        protected ObjectPooler ObjectPooler; // gameObject's pooler
        protected CoinManager CoinManager;
        protected GameObject coin;
        
        private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");

        protected virtual void Awake()
        {
            ObjectPooler = GetComponent<ObjectPooler>();
            CoinManager = GameManager.Get.coinManager;
            
            ObjectPooler.Pool(poolEverythingOnList, shuffleListAfterPooling);
        }

        protected virtual void OnEnable()
        {
            if (!allowedToSpawnCoinsOnPlatform) return;
            OffsetSecondaryTexture();
            coin = CoinManager.AddCoinToPlatform();
        }

        protected virtual void OnDisable()
        {
            if (!allowedToSpawnCoinsOnPlatform) return;

            if (coin != null && CoinManager != null)
            {
                coin.transform.parent = CoinManager.transform;
                coin.SetActive(false);
                coin = null;
            }
        }
    
        private void OffsetSecondaryTexture()
        {
            if (transform.CompareTag("grass") || transform.CompareTag("spider") || transform.CompareTag("grave"))
            {
                Material platformOverlay = transform.GetChild(0).GetComponent<Renderer>().material;
            
                Vector2 offset = platformOverlay.GetTextureOffset(BaseMap);
                offset.y = Random.Range(-.2f, .2f);
            
                platformOverlay.SetTextureOffset(BaseMap, offset);
            }
            
        }
    }
}
