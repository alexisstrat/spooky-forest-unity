using UnityEngine;

namespace Spooky_Forest.Scripts
{
    public class TrafficLight : MonoBehaviour {

        public GameObject trainPoleLight = null;

        public void AnimationEnded()
        {
            trainPoleLight.SetActive(false);
        }
    }
}