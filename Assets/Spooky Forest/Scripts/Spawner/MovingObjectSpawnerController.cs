using Spooky_Forest.Scripts.Mover;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spooky_Forest.Scripts.Spawner
{
    public class MovingObjectSpawnerController : ObjectSpawnerController {
    
        private bool _direction = false; // left or right
        
        protected Transform StartPos = null; // starting position

        [Header("Initial Objects Parameters")] 
        [Tooltip("Check if we want to spawn gameobjects on Start")]
        public bool spawnInitialObjects = true;
    
        [Tooltip("How many objects to spawn on Start")]
        public int initialObjectsToSpawn = 2;
    
        [Tooltip("How many distance should they have?")]
        public float differenceInPosition;

        [Header("Platform's variables")]
        public float speedMax = 4f;
        public float speedMin = 2f;
        public float delayMax = 2f;
        public float delayMin = 1.5f;

        protected float _lastTime = 0f;
        public float delayTime = 0f;
        public float speed = 0f;

        protected float _rot = 0;
        
        // Use this for initialization
        protected void Start ()
        {
            //base.Start();
            _direction = Random.value > 0.5f;

            speed = Random.Range(speedMin, speedMax);

            if (!_direction)
            {
                StartPos = leftStart;
            }
            else
            {
                differenceInPosition = -differenceInPosition;
                StartPos = righStart;
                _rot = 180;
            }
        
            SetMoverVariables();
        
            if(spawnInitialObjects)
                SpawnInitialObjects();
        }
	
        // Update is called once per frame
        void Update () {
            if (Time.time > _lastTime + delayTime)
            {
                _lastTime = Time.time;

                delayTime = Random.Range(delayMin, delayMax);
            
                SpawnObject();
            }
        }

        /// <summary>
        /// Initializes moving objects speed and "end" position
        /// </summary>
        private void SetMoverVariables()
        {
            foreach (var pooledObject in ObjectPooler.pooledObjects)
            {
                MoverBase pooledObjectMover = pooledObject.GetComponent<MoverBase>();
                pooledObjectMover.speed = speed;
                pooledObjectMover.SetMaxSpeed();
                pooledObjectMover.endPos = -StartPos.position;
                pooledObjectMover.direction = _direction;
            }
        }


        /// <summary>
        /// Actual method to get a pooled Gameobject and activate it
        /// </summary>
        /// <returns>The pooled Gameobject</returns>
        protected virtual GameObject SpawnObject()
        {
            GameObject obj = ObjectPooler.GetPooledObject();

            if (obj != null)
            {
                obj.transform.position = StartPos.position;
                obj.SetActive(true);
                obj.transform.rotation = Quaternion.Euler(obj.transform.rotation.x, _rot, 0);
            }

            return obj;
        }

        /// <summary>
        /// Spawn some gameobjects on platforms activation
        /// </summary>
        private void SpawnInitialObjects()
        {
            float k = 0;
            for (int i = 0; i < initialObjectsToSpawn; i++)
            {
                GameObject obj = SpawnObject();
                obj.transform.position = new Vector3(k, StartPos.position.y, StartPos.position.z);

                differenceInPosition = -differenceInPosition;
                k = differenceInPosition;
            }
        }

        /// <summary>
        /// Make last time "now" adding the remaining delay
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            
            _lastTime = Time.time - _lastTime;
            if (coin != null)
            {
                coin.transform.position = new Vector3(Random.Range(-5, 5), coin.transform.position.y, transform.position.z);
                coin.SetActive(true);
            }
        }

        /// <summary>
        /// Keep track of the remaining delay
        /// For example Time.time=1, _lastTime=0, delay=2
        /// _lastTime will become -1 so adding a delay of 2 will become 1 and then we can spawn the next enemy
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            _lastTime = Time.time - _lastTime;
        }
    }
}
