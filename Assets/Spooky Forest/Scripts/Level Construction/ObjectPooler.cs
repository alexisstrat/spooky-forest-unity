using System.Collections.Generic;
using UnityEngine;

namespace Spooky_Forest.Scripts.Level_Construction
{
    public class ObjectPooler : MonoBehaviour {

        [System.Serializable]
        public class PoolItem
        {
            public GameObject objectToPool;
            public int maxAmountToPool;
        }

        public List<PoolItem> itemsToPool;
        public List<GameObject> pooledObjects;
        
        /// <summary>
        /// Call this to create the objects in the list
        /// </summary>
        /// <param name="poolEverything">If set to false a random object from list will be pooled</param>
        /// <param name="shuffle">If we pool everything we can shuffle the list</param>

        public void Pool(bool poolEverything = false, bool shuffle = false)
        {
            pooledObjects = new List<GameObject>();

            if (poolEverything)
            {
                for (int i = 0; i < itemsToPool.Count; i++)
                {
                    PoolObject(i);
                }

                if (shuffle)
                {
                    ShufflePool();
                }
            }
            else
            {
                int pickedFromList = Random.Range(0, itemsToPool.Count);
                PoolObject(pickedFromList);
            }
        }

        private void PoolObject(int index)
        {
            for (int i = 0; i < itemsToPool[index].maxAmountToPool; i++)
            {
                GameObject obj = Instantiate(itemsToPool[index].objectToPool);
                obj.transform.parent = transform;
                obj.SetActive(false);
                pooledObjects.Add(obj);
            }
        }
    
        /// <summary>
        /// Gets an inactive gameobject from the list
        /// </summary>
        /// <returns>returns the pooled object</returns>
        public GameObject GetPooledObject()
        {
            for (int i = 0; i < pooledObjects.Count; i++)
            {
                if (!pooledObjects[i].activeInHierarchy)
                {
                    return pooledObjects[i];
                }
            }
        
            return null;
        }

        public void ShufflePool()
        {
            var count = pooledObjects.Count;
            var last = count - 1;
            for (int i = 0; i < last; ++i) 
            {
                var r = Random.Range(i, count);
                var tmp = pooledObjects[i];
                pooledObjects[i] = pooledObjects[r];
                pooledObjects[r] = tmp;
            }
        }
    }
}
