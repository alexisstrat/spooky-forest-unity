using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spooky_Forest.Scripts.Spawner
{
    public class StaticObjectSpawnerController : ObjectSpawnerController {

        private List<int> _spawnedStaticPositions; // List with the positions of static spawned objects
        private List<int> _spawnedBoundariesPositions;
        private int _startBoundaryXPosition = 6;
        private int _maxBoundaryXPosition = 10;
        protected List<GameObject> spawnedBlockingObjects;
        protected bool AlreadySpawned; // bool to check if we have already spawned the objects once so on enable spawn again in new positions.

        // Use this for initialization
        protected virtual void Start () {
            spawnedBlockingObjects = new List<GameObject>();
            _spawnedStaticPositions = new List<int>();
            _spawnedBoundariesPositions = new List<int>();
            SpawnBoundaries();
            SpawnBlockingObjects();
        }

        protected virtual void SpawnBlockingObjects()
        {
            int rand = Random.Range(2, 4);
            for (int i = 0; i < rand; i++)
            {
                GameObject go = ObjectPooler.GetPooledObject();
                int x = GetPosition();
                go.transform.position = new Vector3(x, leftStart.position.y, leftStart.position.z);
                go.SetActive(true);
                spawnedBlockingObjects.Add(go);
            }
            AlreadySpawned = true;
        
            if (coin != null && allowedToSpawnCoinsOnPlatform)
            {
                AddCoinToPlatform();
            }
        }

    
        /// <summary>
        /// Spawn the objects at both ends of the platform as a visual boundary
        /// </summary>
        protected virtual void SpawnBoundaries()
        {
            for (int i = 0; i < 2; i++)
            {
                GameObject go = ObjectPooler.GetPooledObject();
                go.transform.position = new Vector3(_startBoundaryXPosition, leftStart.position.y, leftStart.position.z);
                go.SetActive(true);
                _startBoundaryXPosition = -_startBoundaryXPosition;
            }

            _startBoundaryXPosition++; // 6 becomes 7
        
            for (int i = 0; i < 2; i++)
            {
                GameObject go = ObjectPooler.GetPooledObject();
                int x = GetBoundaryPosition();
                go.transform.position = new Vector3(x, leftStart.position.y, leftStart.position.z);
                go.SetActive(true);
            }
            _startBoundaryXPosition = -_startBoundaryXPosition; // reverse the x and add another one so -7 becomes -6 (x position)
            _maxBoundaryXPosition = -_maxBoundaryXPosition;
        
            for (int i = 0; i < 2; i++)
            {
                GameObject go = ObjectPooler.GetPooledObject();
                int x = GetBoundaryPosition();
                go.transform.position = new Vector3(x, leftStart.position.y, leftStart.position.z);
                go.SetActive(true);
            }
        }

        // Get the position of each item recursively so it is not pooled in the same position.x
        protected int GetPosition()
        {
            int x = (int)Random.Range(leftStart.position.x + 2, righStart.position.x - 2);// + 1;

            if (!_spawnedStaticPositions.Contains(x) && x != 0)
            {
                _spawnedStaticPositions.Add(x);
                return x;
            }

            return GetPosition();
        }

        private int GetBoundaryPosition()
        {
            int x = Random.Range(_startBoundaryXPosition, _maxBoundaryXPosition);
            if (!_spawnedBoundariesPositions.Contains(x))
            {
                _spawnedBoundariesPositions.Add(x);
                return x;
            }

            return GetBoundaryPosition();
        }

        /// <summary>
        /// Upon 2nd enable and on during lifecycle of the game
        /// spawn the blocking objects into new positions 
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (AlreadySpawned)
            {
                SpawnBlockingObjects();   
            }
        }

        /// <summary>
        /// On disable deactivate every object and clear the lists containing
        /// the gameobjects and their position
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            if (spawnedBlockingObjects == null) return;
        
            foreach (var go in spawnedBlockingObjects)
            {
                go.SetActive(false);
            }
            spawnedBlockingObjects.Clear();
            _spawnedStaticPositions.Clear();
        }
    
        /// <summary>
        /// Enables the coin on the platform in the appropriate position
        /// </summary>
        protected void AddCoinToPlatform()
        {
            int xPos = GetValidCoinPosition();
            coin.transform.position = new Vector3(xPos, coin.transform.position.y, transform.position.z);
            coin.SetActive(true);
        }

        /// <summary>
        /// Gets a position for the coin to spawn that is not on a blocking object
        /// </summary>
        /// <returns>The valid position</returns>
        int GetValidCoinPosition()
        {
            var xPos = (int)Random.Range(-5, 5);
            if (!_spawnedStaticPositions.Contains(xPos))
            {
                return xPos;
            }

            return GetValidCoinPosition();
        }
    }
}
