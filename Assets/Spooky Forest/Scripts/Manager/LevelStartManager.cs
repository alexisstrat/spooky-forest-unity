using System.Collections.Generic;
using UnityEngine;

namespace Spooky_Forest.Scripts.Manager
{
    public class LevelStartManager : MonoBehaviour
    {
        [Header("Level Start Platforms")] 
        public GameObject levelStart;
        public GameObject levelStartTutorial;
        
        public List<GameObject> levelStartPlatforms;

        public delegate void LvlStartDelegate();
        public event LvlStartDelegate LevelStartEvent;

        private void Start()
        {
            SetLevelStart();
        }

        public void SetLevelStart()
        {
            bool tutorialCompleted = GameManager.Get.gameData.TutorialCompleted;
            
            levelStart.SetActive(tutorialCompleted);
            levelStartTutorial.SetActive(!tutorialCompleted);

            if (tutorialCompleted)
            {
                levelStartPlatforms.Clear();
                // Get all pieces in level start
                foreach (Transform child in levelStart.transform)
                {
                    levelStartPlatforms.Add(child.gameObject);
                }
            
                foreach (var platform in levelStartPlatforms)
                {
                    platform.SetActive(false);
                    platform.SetActive(true);
                }
            }

            OnLevelStartReady();
        }

        void OnLevelStartReady()
        {
            LevelStartEvent?.Invoke();
        }
    }
}