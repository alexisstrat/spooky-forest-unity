using UnityEngine;

namespace Spooky_Forest.Scripts.Spawner
{
    public class TutorialSpawnerController : StaticObjectSpawnerController
    {
        public int[] blockingPositions;

        protected override void SpawnBlockingObjects()
        {
            foreach (var blockingPosition in blockingPositions)
            {
                GameObject go = ObjectPooler.GetPooledObject();
                go.transform.position = new Vector3(blockingPosition, leftStart.position.y, leftStart.position.z);
                
                if(!go.name.Contains("Grave"))
                    go.transform.GetChild(0).rotation = Quaternion.Euler(0, Random.Range(-360, 360), 0);
                
                go.SetActive(true);
                spawnedBlockingObjects.Add(go);

            }
        }
    }
}