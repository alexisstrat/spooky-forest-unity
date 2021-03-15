using System.Collections;
using System.Collections.Generic;
using Spooky_Forest.Scripts.Data;
using Spooky_Forest.Scripts.Manager;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spooky_Forest.Scripts.Level_Construction
{
    public class LevelGenerator : MonoBehaviour
    {
        public GameObject highScorePoint;
        public Queue<GameObject> generatedPlatforms = new Queue<GameObject>();

        private GameObject lastSpawnedGameObject = null;

        private int randomRange = 0;
        private float lastPos = .5f;
        private float lastScale = 0;

        public bool spawnOnlyThese;
        [Header("Objects To Spawn")] public bool grave;
        public bool grass;
        public bool water;
        public bool rails;
        public bool spider;
        public bool skeleton;
        public bool ghost;
    
        public delegate void LevelReadyDelegate();
        public event LevelReadyDelegate LevelReadyEvent;


        private void Start()
        {
            StartCoroutine(GeneratePlatforms(GameManager.Get.levelPiecesCount, GameManager.Get.levelLoadDelay));
            SetHighScorePoint();
        }

        /// <summary>
        /// Coroutine to generate platforms. Level delay is low on first run to accommodate for
        /// objects moving with the same speed
        /// </summary>
        /// <param name="numbOfPlatforms">The number of platforms to generate</param>
        /// <param name="levelLoadDelay">The platform delay to generate a new one</param>
        /// <returns></returns>
        private IEnumerator GeneratePlatforms(int numbOfPlatforms, float levelLoadDelay)
        {
            #region Tutorial

            if (!GameManager.Get.gameData.TutorialCompleted)
            {
                lastPos += 1f;
            }

            #endregion
        
            for (int i = 0; i < numbOfPlatforms; i++)
            {
                RandomGenerator();
            
                yield return new WaitForSeconds(levelLoadDelay);
            }
            Debug.Log("Platforms created: <color=magenta><b>" + generatedPlatforms.Count + "</b></color>");
            LevelReady();
        }

        /// <summary>
        /// Setting the position of the highscoreText indicator in world
        /// We are subtracting 2 because that is the default player starting z pos.
        /// </summary>
        private void SetHighScorePoint()
        {
            int highscore = GameManager.Get.gameData.HighScore;

            if (highscore == 0)
            {
                highScorePoint.SetActive(false);
                return;
            }
        
            highScorePoint.transform.position = new Vector3( highScorePoint.transform.position.x,  highScorePoint.transform.position.y, GameManager.Get.gameData.HighScore - 2- 0.5f);
            highScorePoint.SetActive(true);
        }

        /// <summary>
        /// Randomly picks a tag to create a create a platform
        /// </summary>
        public void RandomGenerator()
        {
            randomRange = Random.Range(0, Constants._PlatformTags.Length);

            // SPAWN ONLY THESE ITEMS
            if(spawnOnlyThese)
                switch (Constants._PlatformTags[randomRange])
                {
                    case "grave":
                        if (!grave)
                        {
                            RandomGenerator();
                            return;
                        }

                        break;
                    case "grass":
                        if (!grass)
                        {
                            RandomGenerator();
                            return;
                        }

                        break;
                    case "water":
                        if (!water)
                        {
                            RandomGenerator();
                            return;
                        }

                        break;
                    case "rails":
                        if (!rails)
                        {
                            RandomGenerator();
                            return;
                        }

                        break;
                    case "spider":
                        if (!spider)
                        {
                            RandomGenerator();
                            return;
                        }

                        break;
                    case "skeleton":
                        if (!skeleton)
                        {
                            RandomGenerator();
                            return;
                        }

                        break;
                    case "ghost":
                        if (!ghost)
                        {
                            RandomGenerator();
                            return;
                        }

                        break;

                }

            GameObject obj = PlatformPooler.Get.GetPooledObject(Constants._PlatformTags[randomRange]);
            float height = Constants._PlatformHeights[randomRange];

            if (!obj.Equals(null))
            {
                CreatePlatform(obj, height);
            }
        }

        /// <summary>
        /// Creates the gameobject platform according to tag and height
        /// and sets its position.
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="height"></param>
        public void CreatePlatform(GameObject platform, float height)
        {
            lastSpawnedGameObject = platform;


            float offset = lastPos + (lastScale * 0.5f);
            offset += (platform.transform.localScale.z) * 0.5f;
            Vector3 pos = new Vector3(0, height, offset);

            platform.transform.position = pos;

            lastPos = platform.transform.position.z;
            lastScale = platform.transform.localScale.z;
        
            generatedPlatforms.Enqueue(platform);
        
            platform.SetActive(true);
            //OffsetSecondaryTexture(platform.transform);

        }

        /// <summary>
        /// The name says it all
        /// </summary>
        /// <param name="platform"></param>
        private static void OffsetSecondaryTexture(Transform platform)
        {
            if (platform.CompareTag("grass") || platform.CompareTag("spider") || platform.CompareTag("grave"))
            {
                Vector2 offset = new Vector2(0, Random.Range(-.2f, .2f));
                platform.GetChild(0).GetComponent<Renderer>().material.SetTextureOffset("_BaseMap", offset);
            }
            
        }

        public void ResetPlatforms()
        {
            lastPos = 0.5f;
            lastScale = 0;
            lastSpawnedGameObject = null;
            var i = 0;
            foreach (var platform in generatedPlatforms)
            {
                if (platform.activeInHierarchy)
                {
                    i++;
                    platform.SetActive(false);
                }
            }
            Debug.Log("Platforms Deactivated: <color=magenta><b> " + i + "</b></color>");

            generatedPlatforms.Clear();
        
            SetHighScorePoint();
            StartCoroutine(GeneratePlatforms(GameManager.Get.levelPiecesCount, GameManager.Get.levelLoadDelay));
        }
    

        void LevelReady()
        {
            LevelReadyEvent?.Invoke();
        }
        
    }
}
