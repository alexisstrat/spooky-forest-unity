using System.Collections.Generic;
using Spooky_Forest.Scripts.Level_Construction;
using UnityEngine;

namespace Spooky_Forest.Scripts.Manager
{
    public class FogManager : MonoBehaviour
    {
        public delegate void FogReadyDelegate();
        public event FogReadyDelegate FogReadyEvent;
        
        private ObjectPooler _pooler;

        private int _fogPosition;
        private List<Vector3> _fogStartPos;


        private void Awake()
        {
            _fogStartPos = new List<Vector3>();
            _pooler = GetComponent<ObjectPooler>();
            _pooler.Pool();
        }

        // Start is called before the first frame update
        void Start()
        {
            foreach (var fog in _pooler.pooledObjects)
            {
                fog.transform.position = new Vector3(fog.transform.position.x, fog.transform.position.y, fog.transform.position.z + _fogPosition);
                fog.SetActive(true);
                _fogStartPos.Add(fog.transform.position);
                _fogPosition += 20;
            }
            
            FogReady();
        }

        public void ResetFog()
        {
            for (int i = 0; i < _pooler.pooledObjects.Count; i++)
            {
                _pooler.pooledObjects[i].transform.position = _fogStartPos[i];
            }
            
            FogReady();
        }

        void FogReady()
        {
            FogReadyEvent?.Invoke();
        }
    }
}
