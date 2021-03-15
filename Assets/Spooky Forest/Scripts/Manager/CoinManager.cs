using System.Collections.Generic;
using Spooky_Forest.Scripts.Level_Construction;
using UnityEngine;

namespace Spooky_Forest.Scripts.Manager
{
    public class CoinManager : MonoBehaviour
    {
        public List<GameObject> activeCoins;
        private ObjectPooler _coinPooler;

        public int coins = 0;
        public int maxCoins;

        [Range(0.0f, 1.0f)] 
        public float maxPropabilityOnPlaform;

        [Range(0.0f, 1.0f)]
        public float maxPropabilityOnBoat;

        // Start is called before the first frame update
        void Awake()
        {
            _coinPooler = GetComponent<ObjectPooler>();
            _coinPooler.Pool();
            maxCoins = _coinPooler.itemsToPool[0].maxAmountToPool;
        }

        public GameObject AddCoinToPlatform()
        {
            coins = activeCoins.Count;

            if (coins == maxCoins)
            {
                return null;
            }

            float percentage = 1 - (float)coins / maxCoins;
            percentage *=  maxPropabilityOnPlaform;

            GameObject coin = null;
            if (Random.value < percentage)
            {
                coin = _coinPooler.GetPooledObject();
            }

            return coin;
        }
        
        public GameObject AddCoinToBoat()
        {
            coins = activeCoins.Count;
            
            if(coins == maxCoins)
                return null;
            
            float percentage = 1 - (float) coins / maxCoins;
            percentage *= maxPropabilityOnBoat;

            GameObject coin = null;
            if (Random.value < percentage)
            {
                coin = _coinPooler.GetPooledObject();
            }

            if (coin != null)
            {
                coin.SetActive(true);
                Debug.Log("Spawned coin with a " + percentage * 100 + "% chance ");
            }
            
            return coin;
        }
    }
}
