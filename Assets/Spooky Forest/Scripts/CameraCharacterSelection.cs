using UnityEngine;
using UnityEngine.UI;

namespace Spooky_Forest.Scripts
{
    public class CameraCharacterSelection : MonoBehaviour
    {
        public Slider scrollbar; // assign in the inspector
        public float minPosition;
        public float maxPosition;

        private Vector3 StartPos;
 
        private void Awake()
        {
            StartPos = transform.position;
            if(scrollbar != null)
            {
                scrollbar.onValueChanged.AddListener(onScroll);
            }
        }
 
        private void onScroll(float value)
        {
            transform.position = Vector3.Lerp(new Vector3(minPosition, StartPos.y, StartPos.z), new Vector3(maxPosition, StartPos.y, StartPos.z), value);
        }
    }
}
