using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Spooky_Forest.Scripts.Data;
using Spooky_Forest.Scripts.Manager;
using Spooky_Forest.Scripts.Mover;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spooky_Forest.Scripts.Controller
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Variables")]
        public float moveDistance = 1;
        public float moveTime = 0.15f;
        public float colliderDistCheck = 1;
        public Ease easeDrowning;
    
        [Header("Angled Movement Variables")]
        public float angleCheck = 1;
        public float angleCheckDist = 0.1f;
    
        [Header("Player States")]
        public bool isIdle;
        public bool isDead;
        public bool isMoving;
        public bool isWalking;
        public bool jumpStart;
        public bool isAbleToJump;
        public bool parentedToObject;

        [Header("Player Object")] 
        public CharacterItem[] characterItems;
        public List<GameObject> charactersGameObjects;
        public Animator characterAnimator;
        public GameObject character;
    
        [Header("Particle Systems")]
        public ParticleSystem gotHitParticleSystem;
        public ParticleSystem splash;

        [Header("Audio Files")] 
        public List<AudioClip> steps;
        public AudioClip audioHop;
        public AudioClip audioHit;
        public AudioClip audioSplash;
        public AudioClip bodyFall;
    
        [Space(20)]
        public float playerDistance = -2;

        private Vector3 _startPosition = new Vector3(0, 0.24f, -2f);
        private Quaternion _startRotation = Quaternion.Euler(0, 0, 0);
        private Vector3 _startCharPosition = new Vector3(0,0.006f,-2f);
        private AudioSource _audioSource;
        public Collider playerCollider;
        private bool _boatInFront;
        private bool _boatAtLeft;
        private bool _boatAtRight;
    
        public class BoatRaycastElements
        {
            public readonly float hitDistance;
            public readonly Vector3 hitPosition;
            public readonly Transform parent;

            public BoatRaycastElements(float hitDistance, Vector3 hitPosition, Transform parent)
            {
                this.hitDistance = hitDistance;
                this.hitPosition = hitPosition;
                this.parent = parent;
            }
        }
    
        public delegate void PlayerReadyDelegate();
        public event PlayerReadyDelegate PlayerReadyEvent;
    
        // Use this for initialization
        void Start ()
        {
            _audioSource = GetComponent<AudioSource>();
            playerDistance = GameManager.Get.CurrentDistance;

            CreateCharacters();
            SetPlayerParameters();
        }

        // Update is called once per frame
        void Update () {

            if (!GameManager.Get.CanPlay()) return;

            if (isDead) return;
        
            CanIdle();
            CanMove();
            IsVisible();
        }

        void CreateCharacters()
        {
            foreach (var characterItem in characterItems)
            {
                GameObject go = Instantiate(characterItem.model, transform);
                go.name = characterItem.characterName;
                go.SetActive(false);

                Instantiate(characterItem.hitParticles, go.transform);
            
                charactersGameObjects.Add(go);
            }
        }
    
        /// <summary>
        /// On game start/restart we set everything on player to default values
        /// </summary>
        public void SetPlayerParameters()
        {
            transform.parent = null;
            
            SetCharacterModel();

            gameObject.transform.position = _startPosition;
            character.transform.rotation = _startRotation;
            character.transform.position = _startCharPosition;
            
            isIdle = true;
            isDead = isAbleToJump = isMoving = isWalking = parentedToObject = false;
            
            character.SetActive(true);
            characterAnimator.Play("Character|idle");
            
            GetComponent<BoxCollider>().enabled = true;
            
            playerDistance = 0;
            splash.Stop();
            gotHitParticleSystem.Stop();
        
            PlayerReady();
        }

        public void SetCharacterModel()
        {
            foreach (var chars in charactersGameObjects)
            {
                chars.SetActive(false);
            }
        
            for (int i = 0; i < characterItems.Length; i++)
            {
                if (characterItems[i].characterId == GameManager.Get.gameData.SelectedCharacter)
                {
                    character = charactersGameObjects[i];
                
                    audioHit = characterItems[i].grunt; // Set character audio
                    gotHitParticleSystem = character.transform.GetChild(1).GetComponent<ParticleSystem>();
                
                    character.SetActive(true);       
                    characterAnimator = character.transform.GetChild(0).GetComponent<Animator>();
                    break;
                }
            }
        }

        void CanIdle()
        {
            if (MobileInput.Get.Tap)
            {
                jumpStart = true;
                if (!DOTween.IsTweening(transform))
                {
                    characterAnimator.Play("Character|jumpStart");
                }
            }

            if (MobileInput.Get.SwipeLeft) { CheckIfIdle(0, -90, 0); }
            if (MobileInput.Get.SwipeRight) { CheckIfIdle(0, 90, 0); }
            if (MobileInput.Get.SwipeDown) { CheckIfIdle(0, 180, 0); }
            if (jumpStart && MobileInput.Get.LiftedF)
            { CheckIfIdle(0, 0, 0); }
        }

        void CheckIfIdle(float x, float y, float z)
        {
            character.transform.rotation = Quaternion.Euler(x, y, z);// DORotateQuaternion(Quaternion.Euler(x, y, z), 0.2f);

            if (parentedToObject)
            {
                CheckIfCanMoveAngles();
            }
            else
            {
                CheckIfCanMoveSingleRay();
            }
            
        }

        /// <summary>
        /// Raycasting in front of the player to see if we can move
        /// </summary>
        void CheckIfCanMoveSingleRay() {
            RaycastHit hit;
            Physics.Raycast(transform.position, character.transform.forward, out hit, colliderDistCheck); 

            Debug.DrawRay(transform.position, character.transform.forward * colliderDistCheck, Color.red, 2f);

            if (hit.collider == null)
            {
                SetMove();
            }
            else
            {
                if (hit.collider.CompareTag(Constants.Tags.Collider))
                {
                    Debug.Log("Hit something with collider tag");

                    jumpStart = false;
                    isIdle = true;
                    characterAnimator.CrossFade("Character|idle", 0.1f);
                }
                else
                {
                    SetMove();
                }
            }
        }

        /// <summary>
        /// Raycasting along angles to check if we can move
        /// </summary>
        void CheckIfCanMoveAngles()
        {
            RaycastHit hit;
            RaycastHit hitLeft;
            RaycastHit hitRight;

            Physics.Raycast(transform.position, character.transform.forward, out hit, colliderDistCheck);
            Physics.Raycast(transform.position, character.transform.forward + new Vector3(angleCheck, 0, 0), out hitLeft, colliderDistCheck + angleCheckDist);
            Physics.Raycast(transform.position, character.transform.forward + new Vector3(-angleCheck, 0, 0), out hitRight, colliderDistCheck + angleCheckDist);


            if (hit.collider == null && hitLeft.collider == null && hitRight.collider == null)
            {
                SetMove();
            }
            else
            {
                if(hit.collider != null && hit.collider.CompareTag(Constants.Tags.Collider))
                {
                    Debug.Log("Hit something with collider in front\nChecking now in angles");
                    if (hitLeft.collider != null && hitLeft.collider.CompareTag(Constants.Tags.Collider))
                    {
                        Debug.Log("Hit something with collider in left\nChecking now in right");
                        if (hitRight.collider != null && hitRight.collider.CompareTag(Constants.Tags.Collider))
                        {
                            Debug.Log("Hit something with collider in right\nPassage Blocked");
                            isIdle = true;
                            characterAnimator.CrossFade("Character|idle", 0.1f);
                        }
                        else
                        {
                            Debug.Log("Can Move Right");
                        }
                    }
                    else
                    {
                        Debug.Log("Can move Left");   
                    }
                }
                else
                {
                    Debug.Log("Can move front");
                    SetMove();
                }
            }
        }
    
        void SetMove() {
            isIdle = false;
            isMoving = true;
            jumpStart = true;
        }

        void CanMove() {
            if (isMoving)
            {
                if (MobileInput.Get.SwipeDown)
                {
                    float x = GetXPosIfBoat();
                    Moving(new Vector3(x, 0.24f, transform.position.z - moveDistance));
                }
                if (MobileInput.Get.SwipeLeft)
                {
                    Moving(new Vector3(transform.position.x - moveDistance, 0.24f, transform.position.z));
                }
                if (MobileInput.Get.SwipeRight)
                {
                    Moving(new Vector3(transform.position.x + moveDistance, 0.24f, transform.position.z));
                }
                if (MobileInput.Get.LiftedF)
                {
                    float x = GetXPosIfBoat();
                    Moving(new Vector3(x, 0.24f, transform.position.z + moveDistance));
                    SetMoveForwardState();
                }
            }
        }

        /// <summary>
        /// Plays the moving animation and actually moves the player
        /// </summary>
        /// <param name="pos"></param>
        void Moving(Vector3 pos)
        {
            isIdle = jumpStart = isMoving =false;
            isWalking = true;

            if (isAbleToJump || parentedToObject)
            {
                characterAnimator.Play("Character|jump");
            }
            else
            {
                characterAnimator.CrossFade("Character|run", 0.1f);
            }
        
            if (!_boatInFront && !_boatAtLeft && !_boatAtRight)
            {
                pos.x = Mathf.RoundToInt(pos.x);
            }
        
            transform.DOMove(new Vector3(pos.x, pos.y, Mathf.Round(pos.z)), moveTime).OnComplete(MoveComplete).WaitForCompletion(true);

            int randomAudio = Random.Range(0, 5);
            PlayAudioClip(steps[randomAudio]);
        }

        /// <summary>
        /// Move is complete
        /// </summary>
        void MoveComplete() {
            var downDirection = (-character.transform.up).normalized;
            Debug.DrawRay(transform.position, downDirection * 1, Color.magenta, 1);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, downDirection, out hit))
            {
                Debug.Log("<color=magenta>Down</color> " + hit.transform.name);
                if (hit.transform.CompareTag(Constants.Tags.Water))
                {
                    DOTween.Kill(transform);
                    GotSoaked();
                }
                else if (hit.transform.CompareTag(Constants.Tags.Boat))
                {
                    parentedToObject = true;
                    transform.parent = hit.transform;
                    hit.transform.GetComponent<MoverBase>().AnimationOnPlayerCollision();
                }
                else
                {
                    UnparentPlayer();
                }
            }
            else
            {
                UnparentPlayer();
            }
            
            isWalking = isAbleToJump = _boatInFront = _boatAtLeft = _boatAtRight = false;
            isIdle = true;
        }

        private void UnparentPlayer()
        {
            if (!parentedToObject) return;
            
            parentedToObject = false;
            transform.parent = null;
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);
        }

        /// <summary>
        /// Update the distance count
        /// </summary>
        void SetMoveForwardState() {
            if (transform.position.z + 3 > playerDistance)
            {
                GameManager.Get.UpdateDistanceCount();
                playerDistance = GameManager.Get.CurrentDistance;
            }
        }

        /// <summary>
        /// We check if the player is visible to the camera,
        /// if not the game is over
        /// </summary>
        void IsVisible ()
        {
            var screenPosition = GameManager.Get.mainCamera.WorldToScreenPoint(transform.position);

            if (screenPosition.x < 0 || screenPosition.x > Screen.width ||
                screenPosition.y < 0)
            {
                if (parentedToObject)
                {
                    GotSoaked();
                }
                else
                {
                    GameManager.Get.onGameOverEvent?.Invoke("camera");
                    GotHit(true);
                }
            }
        }

        /// <summary>
        /// Everything that hit the player BUT water calls this
        /// </summary>
        /// <param name="direction"></param>
        public void GotHit(bool direction)
        {
            transform.parent = null;
            DOTween.Kill(transform);
        
            playerCollider.enabled = false;
            isDead = true;
            isIdle = jumpStart = false;

            character.transform.DORotate(new Vector3(0, 0, 0), 1f);
        
            if(direction)
                characterAnimator.Play("Character|dieLeft");
            else
            {
                characterAnimator.Play("Character|dieRight");
            }
        
            ParticleSystem.EmissionModule emissionModule = gotHitParticleSystem.emission;
            emissionModule.enabled = true;
            gotHitParticleSystem.Play();

            PlayAudioClip(audioHit);
            PlayAudioCoroutine(bodyFall);

            GameManager.Get.GameOver();
        }

        /// <summary>
        /// Called when player falls into water
        /// </summary>
        public void GotSoaked()
        {
            transform.parent = null;
            playerCollider.enabled = false;
            isDead = true;
            isIdle = false;
            jumpStart = false;
        
            ParticleSystem.EmissionModule emissionModule = splash.emission;
            emissionModule.enabled = true;
            splash.Play();

            character.transform.DOMoveY(-1.2f, 0.3f).SetEase(easeDrowning);

            PlayAudioClip(audioSplash);

            GameManager.Get.GameOver();
            GameManager.Get.onGameOverEvent?.Invoke("river");
        }

        /// <summary>
        /// Checks if the forward raycast hit a boat object collider
        /// </summary>
        private bool CheckForBoatInFront(RaycastHit hit)
        {
            if (hit.transform != null)
            {
                if (hit.transform.CompareTag(Constants.Tags.Boat))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if the 45 degree angle to the left raycast hit a boat object collider
        /// </summary>
        private bool CheckForBoatAtLeft(RaycastHit hitLeft)
        {
            if (hitLeft.transform != null)
            {
                if (hitLeft.transform.CompareTag(Constants.Tags.Boat))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if the 45 degree angle to the right raycast hit a boat object collider
        /// </summary>
        private bool CheckForBoatAtRight(RaycastHit hitRight)
        {
            if (hitRight.transform != null)
            {
                if (hitRight.transform.CompareTag(Constants.Tags.Boat))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// We check with Raycasts if there is a boat/log in front and in 45 degrees angles
        /// If yes we get the closer one and return its X position to move the player
        /// </summary>
        /// <returns>The final x position the player will move to</returns>
        float GetXPosIfBoat()
        {
            RaycastHit hit;
            RaycastHit hitLeft;
            RaycastHit hitRight;

            Physics.Raycast(transform.position, character.transform.forward, out hit, colliderDistCheck);
            Physics.Raycast(transform.position, character.transform.forward + new Vector3(angleCheck, 0, 0),  out hitLeft, colliderDistCheck + angleCheckDist);
            Physics.Raycast(transform.position, character.transform.forward + new Vector3(-angleCheck, 0, 0),out hitRight, colliderDistCheck + angleCheckDist);
        
            Debug.DrawRay(transform.position, character.transform.forward * colliderDistCheck, Color.red, 2);
            Debug.DrawRay(transform.position, (character.transform.forward + new Vector3(angleCheck, 0, 0)) * (colliderDistCheck + angleCheckDist), Color.green, 2);
            Debug.DrawRay(transform.position, (character.transform.forward + new Vector3(-angleCheck, 0, 0)) * (colliderDistCheck + angleCheckDist), Color.blue, 2);

            _boatInFront = CheckForBoatInFront(hit);
            _boatAtLeft = CheckForBoatAtLeft(hitLeft);
            _boatAtRight = CheckForBoatAtRight(hitRight);
        
            List<BoatRaycastElements> boatRaycasts = new List<BoatRaycastElements>();

            if (_boatInFront)
            {
                boatRaycasts.Add(new BoatRaycastElements(Vector3.Distance(hit.point, transform.position), hit.point, hit.transform));
            }

            if (_boatAtLeft)
            {
                boatRaycasts.Add(new BoatRaycastElements(Vector3.Distance(hitLeft.point, transform.position), hitLeft.point, hitLeft.transform));
            }

            if (_boatAtRight)
            {
                boatRaycasts.Add(new BoatRaycastElements(Vector3.Distance(hitRight.point, transform.position), hitRight.point, hitRight.transform));
            }

            if (boatRaycasts.Count > 0)
            {
                // Sort the list. The first element will be the closest point to player
                boatRaycasts = boatRaycasts.OrderBy(element => element.hitDistance).ToList();
                transform.parent = boatRaycasts[0].parent;
                isAbleToJump = true;

                var speed = boatRaycasts[0].parent.GetComponent<MoverBase>().speed;
                
                var futureBoatPos = boatRaycasts[0].hitPosition + (boatRaycasts[0].parent.forward *
                                                              (speed * moveTime));
                
                return futureBoatPos.x;
            }
        
            return transform.position.x;
        }

        void PlayAudioClip(AudioClip clip)
        {
            AudioManager.Get.PlayPlayerSfx(clip);
        }

        void PlayAudioCoroutine(AudioClip clip)
        {
            StartCoroutine(AudioManager.Get.PlaySfx(clip));
        }

        /// <summary>
        /// This function invokes the event to broadcast that the player is ready
        /// </summary>
        void PlayerReady()
        {
            PlayerReadyEvent?.Invoke();
        }
    }
}
