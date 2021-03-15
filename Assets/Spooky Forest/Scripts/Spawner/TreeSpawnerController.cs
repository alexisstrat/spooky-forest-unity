using UnityEngine;

namespace Spooky_Forest.Scripts.Spawner
{
    public class TreeSpawnerController : StaticObjectSpawnerController 
    {
        
        protected override void SpawnBlockingObjects()
        {
            int rand = Random.Range(2, 4);
            for (int i = 0; i < rand; i++)
            {
                GameObject go = ObjectPooler.GetPooledObject();
                int x = GetPosition();
                go.transform.position = new Vector3(x, leftStart.position.y, leftStart.position.z);
            
                // Trees get a random rotation before activating
                go.transform.GetChild(0).rotation = Quaternion.Euler(0, Random.Range(-360, 360), 0);
            
                go.SetActive(true);
                spawnedBlockingObjects.Add(go);
            }
            AlreadySpawned = true;
            
            if (coin != null)
            {
                AddCoinToPlatform();
            }
        }
    }
}
