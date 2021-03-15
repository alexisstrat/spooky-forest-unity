using System.Collections.Generic;
using UnityEngine;

namespace Spooky_Forest.Scripts.Level_Construction
{
    public class PlatformPooler : MonoBehaviour {

        [System.Serializable]
        public class PlatformPoolItem
        {
            public GameObject objectToPool;
            public int maxAmountToPool;
            public bool shouldExpand = true;
        }

        private static PlatformPooler _instance;
    
        #region Singleton
        public static PlatformPooler Get
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(PlatformPooler)) as PlatformPooler;
                }
                return _instance;
            }
        }
        #endregion

        [Header("List of platforms")]
        public List<PlatformPoolItem> itemsToPool;
    
        [Header("List of pooled platforms for debugging")]
        public List<GameObject> pooledPlatforms;

        private void Start()
        {
            pooledPlatforms = new List<GameObject>();
            foreach (PlatformPoolItem item in itemsToPool)
            {
                for (int i = 0; i < item.maxAmountToPool; i++)
                {
                    GameObject obj = Instantiate(item.objectToPool);
                    obj.transform.parent = transform;

                    obj.SetActive(false);
                    pooledPlatforms.Add(obj);
                }
            }
        }

        public GameObject GetPooledObject(string tag)
        {
            for (int i = 0; i < pooledPlatforms.Count; i++)
            {
                if (!pooledPlatforms[i].activeInHierarchy && pooledPlatforms[i].tag ==  tag)
                {
                    return pooledPlatforms[i];
                }
            }

            // grow the pool if all objects are active and we request another one
            foreach (PlatformPoolItem item in itemsToPool)
            {
                if(item.objectToPool.tag == tag)
                {
                    if (item.shouldExpand)
                    {
                        GameObject obj = Instantiate(item.objectToPool);
                        obj.transform.parent = transform;
                        obj.SetActive(false);
                        pooledPlatforms.Add(obj);
                        return obj;
                    }
                }
            }
            return null;
        }
    }
}
