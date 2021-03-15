using Spooky_Forest.Scripts.Mover;
using UnityEngine;

namespace Spooky_Forest.Scripts.Spawner
{
    public class BoatSpawnerController : MovingObjectSpawnerController
    {
        protected override GameObject SpawnObject()
        {
            GameObject obj = ObjectPooler.GetPooledObject();

            if (obj != null)
            {
                obj.transform.position = StartPos.position;
                obj.SetActive(true);
                obj.transform.rotation = Quaternion.Euler(obj.transform.rotation.x, _rot, 0);// obj.transform.rotation * Quaternion.Euler(0, rot, 0);
            }
            else
            {
                return null;
            }
            
            var coin = CoinManager.AddCoinToBoat();

            if (coin != null)
            {
                var offset = Random.Range(-0.5f, 0.5f);
                coin.transform.parent = obj.transform;
                coin.transform.position = new Vector3(obj.transform.position.x + offset, obj.transform.position.y, obj.transform.position.z);
                obj.GetComponent<MoverBase>().coinParented = coin;
                coin.SetActive(true);
            }

            return obj;
        }

        protected override void OnEnable()
        {
            _lastTime = Time.time - _lastTime;
        }
    }
}