using Spooky_Forest.Scripts.Controller;
using Spooky_Forest.Scripts.Data;
using Spooky_Forest.Scripts.Manager;
using UnityEngine;
namespace Spooky_Forest.Scripts.Mover
{
    public class MoverBase : MonoBehaviour {

        [Header("Object's Variables. These are set automatically")]
        public float speed = 1f;
        protected float NormalSpeed;
        protected float MaxSpeed;
        public bool direction = false;
        public Vector3 endPos = Vector3.zero;
        protected int Rotation = 10; // used for boat rotation
        public GameObject coinParented;
    
        // Use this for initialization
        protected virtual void Start ()
        {
            NormalSpeed = speed;
            if (direction)
                Rotation = -Rotation;
        }
	
        // Update is called once per frame
        protected virtual void Update () {
            transform.Translate(speed * Time.deltaTime, 0, 0);

            ReachedEnd();
        }

        /// <summary>
        /// Deactivates gameobject if it reaches end position
        /// </summary>
        protected virtual void ReachedEnd()
        {
            if (Mathf.Abs(transform.position.x) >= Mathf.Abs(endPos.x))
            {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Set the coin parent again to coin manager, deactivate it and
        /// set the reference of the mover to nothing
        /// </summary>
        protected void ResetCoin()
        {
            coinParented.transform.parent = GameManager.Get.coinManager.transform;
            coinParented.SetActive(false);
            coinParented = null;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Constants.Tags.Player))
            {
                Debug.Log("Mover trigger ENTER player");
                
                GameManager.Get.onGameOverEvent?.Invoke(gameObject.tag);
                
                other.GetComponent<PlayerController>().GotHit(direction);
            }
        }

        public void SetMaxSpeed()
        {
            MaxSpeed = 5 * speed;
        }
    
        public virtual void AnimationOnPlayerCollision()
        {
        
        }
    
        public virtual void AnimationOnPlayerExit(){}
    }
}
