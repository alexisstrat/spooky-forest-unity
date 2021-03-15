using Spooky_Forest.Scripts.Controller;
using UnityEngine;

namespace Spooky_Forest.Scripts
{
    public class KillBox : MonoBehaviour {

        // check for trigger
        // if player, send message get hit

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<PlayerController>().GotSoaked();
            }
        }
    }
}
