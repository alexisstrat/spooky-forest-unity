using Spooky_Forest.Scripts.Controller;
using UnityEngine;

namespace Spooky_Forest.Scripts
{
    public class AnimatorController : MonoBehaviour {

        public PlayerController playerController = null;
        private Animator animator = null;
    
        //animator booleans
        private static readonly int Jump = Animator.StringToHash("jump");
        private static readonly int Move = Animator.StringToHash("move");
        private static readonly int JumpStart = Animator.StringToHash("jumpStart");

        // Use this for initialization
        void Start () {
            animator = GetComponent<Animator>();
        }
	
        // Update is called once per frame
        void Update () {
            
            if (playerController.jumpStart)
            {
                animator.SetBool("jumpStart", true);
            }
        
            if (playerController.isWalking)
            {
                if (playerController.isAbleToJump || playerController.parentedToObject)
                {
                    animator.SetBool(Jump, true);
                }
                else
                {
                    animator.SetBool(Move, true);
                }
                animator.SetBool(JumpStart, false);
            }
            else
            {
                animator.SetBool("move", false);
                animator.SetBool("jumpStart", false);
            }
            
        }
    }
}
