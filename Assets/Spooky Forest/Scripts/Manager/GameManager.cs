using System;
using System.Collections;
using DG.Tweening;
using Spooky_Forest.Scripts.Controller;
using Spooky_Forest.Scripts.Level_Construction;
using Spooky_Forest.Scripts.Native;
using UnityEngine;
using UnityEngine.Events;

namespace Spooky_Forest.Scripts.Manager
{
    public class GameManager : MonoBehaviour
    {
        [Header("Player")] public GameObject player;

        [Header("Ambient Audio")] 
        public AudioClip ambientAudioClip;
    
        [Header("3D Camera")]
        public Camera mainCamera = null;

        [Header("Level Generator Parameters")]
        public LevelGenerator levelGenerator = null;
        public int levelPiecesCount = 30;

        [Range(0.01f, 0.5f)]
        public float levelLoadDelay = 0.2f;

        [Header("Managers")] 
        public GuiManager guiManager;
        public LevelStartManager levelStartManager;
        public SaveManager saveManager;
        public SettingsManager settingsManager;
        public AudioManager audioManager;
        public FogManager fogManager;
        public CoinManager coinManager;
        public CharacterSelectionManager characterSelectionManager;
        public PlayServicesManager playServicesManager;
    
        private int _currentDistance = 0;
        public int CurrentDistance => _currentDistance;

        [HideInInspector]
        public bool canPlay;
        [HideInInspector]
        public bool gameOver;
        [HideInInspector]
        public GameOverEvent onGameOverEvent = new GameOverEvent();

        public SaveManager.Save gameData;
    
        private bool _isPaused = false;
        private CameraBehaviour _cameraBehaviour;
        private bool _playerIsReady, _levelIsReady, _levelStartIsReady, _fogIsReady, _cameraIsReady, _uiIsReady;
        private PlayerController _playerController;
        private static GameManager _instance;
        public bool isHighscore;

        #region Singleton
        public static GameManager Get
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(GameManager)) as GameManager;
                }
                return _instance;
            }
        }
        #endregion

        private void Awake()
        {
            #region Enable or Disable logging
#if DEVELOPMENT_BUILD || UNITY_EDITOR     
            Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif
            #endregion
        
            Application.targetFrameRate = 60;
            
            _cameraBehaviour = mainCamera.GetComponent<CameraBehaviour>();
            _playerController = player.GetComponent<PlayerController>();

            SubscribeToEvents();

            gameData = saveManager.LoadSavedData();
            settingsManager.SetSettings();
            characterSelectionManager.SetCharactersVariables(gameData);
        }

        // Use this for initialization
        void Start() {
            DeviceOrientationManager.OnScreenOrientationChanged += SetCameraAndUI;
            StartCoroutine(CreateWorld());
        }
    
        /// <summary>
        /// Update only used to check for back button input to pause/resume or quit the game
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (guiManager.activeScreen != null)
                {
                    guiManager.CloseActiveScreen();
                    return;
                }
            
                if (!_isPaused && canPlay)
                {
                    PauseGame();
                }
                else if(_isPaused)
                {
                    ResumeGame();
                }
                else
                {
                    Quit();
                }
            }
        }

        /// <summary>
        /// Game Manager subscribes to events to know when everything is ready so we can start the game
        /// </summary>
        void SubscribeToEvents()
        {
            guiManager.UiReadyEvent += OnUiReady;
            levelStartManager.LevelStartEvent += OnLevelStartReady;
            _playerController.PlayerReadyEvent += OnPlayerReady;
            levelGenerator.LevelReadyEvent += OnLevelReady;
            _cameraBehaviour.CameraReadyEvent += OnCameraReady;
            fogManager.FogReadyEvent += OnFogReady;
        }

        /// <summary>
        /// Function called on each screen rotation to set the camera parameters
        /// </summary>
        /// <param name="screenOrientation">The orientation in question</param>
        private void SetCameraAndUI(ScreenOrientation screenOrientation)
        {
            Debug.Log("Orientation Change to " + screenOrientation);
            StartCoroutine(_cameraBehaviour.SetCameraParameters());
        }
    
        /// <summary>
        /// Coroutine to generate the level platforms every "levelLoadDelay"
        /// </summary>
        /// <returns></returns>
        IEnumerator CreateWorld()
        {
        
        
            while (!_playerIsReady || !_levelIsReady || !_cameraIsReady || !_uiIsReady || !_fogIsReady )
            {
                yield return null;
            }
        
            audioManager.PlayAmbient(ambientAudioClip);

            guiManager.FadeLoadingImage();

        }

        /// <summary>
        /// Called when application is soft closed on Android
        /// </summary>
        /// <param name="pauseStatus">soft closed status</param>
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                // Save the data in case app is killed
                saveManager.SaveData(gameData);
            
                // Put the game in pause state if we are playing
                if(canPlay)
                    PauseGame();
            }
        }

        public void OpenGuiScreen(GameObject guiScreen)
        {
            guiManager.OpenGuiScreen(guiScreen);
        }

        public void GoBackGui()
        {
            guiManager.CloseActiveScreen();
        }
    
        /// <summary>
        /// This pauses the game
        /// </summary>
        public void PauseGame()
        {
            canPlay = false;
            Time.timeScale = 0;
            guiManager.EnablePauseScreen(true);
            audioManager.PauseAudio();
            _isPaused = true;
        }

        /// <summary>
        /// This resumes the game
        /// </summary>
        public void ResumeGame()
        {
            guiManager.EnablePauseScreen(false);
            canPlay = true;
            Time.timeScale = 1;
            audioManager.ResumeAudio();
            _isPaused = false;
        }
    
        public void UpdateCoinCount(int value)
        {
            Debug.Log("Player picked up another coin for" + value);

            gameData.Coins += value;

            guiManager.UpdateCoinsText();
        }

        /// <summary>
        /// Updates the Distance UI, generates a new platform
        /// and removes one if it is past the camera view
        /// </summary>
        public void UpdateDistanceCount()
        {
            _currentDistance += 1;

            guiManager.UpdateDistanceText();
            
            levelGenerator.RandomGenerator();
            if(_currentDistance > levelGenerator.generatedPlatforms.Peek().transform.position.z + 10)
            {
                levelGenerator.generatedPlatforms.Peek().SetActive(false);
                levelGenerator.generatedPlatforms.Dequeue();
            }
        }

        public void TutorialCompleted()
        {
            gameData.TutorialCompleted = true;
            saveManager.SaveData(gameData);
        }

        /// <summary>
        /// Boolean to check if we are playing the game
        /// </summary>
        /// <returns>true or false (no shit sherlock)</returns>
        public bool CanPlay()
        {
            return canPlay;
        }

        /// <summary>
        /// Called with UI Button to start the game
        /// </summary>
        public void StartPlay()
        {
            isHighscore = false;
            canPlay = true;
            guiManager.StartPlay();
        }

        /// <summary>
        /// When the game is over this is called and checks for highscoreText
        /// </summary>
        public void GameOver()
        {
            Debug.Log("<color=red>Game Over</color>");
            Time.timeScale = 0.2f;
            canPlay = false;
            gameOver = true;


            _cameraBehaviour.automove = false;
            _cameraBehaviour.Shake();

            isHighscore = _currentDistance > gameData.HighScore;
            
            guiManager.SetHighscoreText(isHighscore);

            // Save data if it's a highscore
            if(isHighscore)
            {
                gameData.HighScore = _currentDistance;
                saveManager.SaveData(gameData);
            
                playServicesManager.AddScoreToLeaderboard(_currentDistance);
            }
        }

        public void ShowLeaderboard()
        {
            playServicesManager.ShowLeaderboard();
        }

        /// <summary>
        /// Activates the GameOver UI
        /// </summary>
        public void GuiGameOver()
        {
            //DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, 0.2f);
            Time.timeScale = 1f;
            guiManager.GameOver();
        }

        /// <summary>
        /// Called with UI Button and resets everything in game without reloading the scene
        /// and unloading assets but resetting each value
        /// </summary>
        public void PlayAgain()
        {   
            DOTween.KillAll();
        
            audioManager.RestartAudio();
        
            GameVariablesReset();

            StartCoroutine(CreateWorld());
        }

        /// <summary>
        /// Resets the game variables on "PLAY AGAIN" by calling each corresponding
        /// reset() function
        /// </summary>
        private void GameVariablesReset()
        {
            _playerIsReady = _levelIsReady = _levelStartIsReady = _cameraIsReady = _fogIsReady = gameOver = false;
            _currentDistance = 0;
            levelLoadDelay = 0.1f;
            guiManager.SetUi();        
            levelStartManager.SetLevelStart();
            levelGenerator.ResetPlatforms();
            _playerController.SetPlayerParameters();
            StartCoroutine(_cameraBehaviour.SetCameraParameters());
            fogManager.ResetFog();
        }
    
        public void TrySelectCharacter()
        {
            characterSelectionManager.TrySelectCharacter();
        }
    
        public void BuyCharacter(int characterId, int characterCost)
        {
            gameData.Coins -= characterCost;
            StartCoroutine(UpdateBeansUi());
            gameData.UnlockedCharacters.Add(characterId);
            saveManager.SaveData(gameData);
        }

        /// <summary>
        /// Updates the coins ui with animation
        /// </summary>
        /// <returns></returns>
        IEnumerator UpdateBeansUi()
        {
            float currentCoins = int.Parse(guiManager.coins.text);
            int initialCoins = (int) currentCoins;
        
            while (currentCoins >= gameData.Coins)
            {
                currentCoins -= (gameData.Coins + initialCoins) * Time.deltaTime;
                guiManager.coins.text = currentCoins.ToString("0");
                yield return null;
            }
        
            guiManager.coins.text = gameData.Coins.ToString();
            yield return null;
        }

        /// <summary>
        /// Called on game quit. Also saves the game data
        /// </summary>
        public void Quit()
        {
            Debug.Log("QUIIIT");
            saveManager.SaveData(gameData);
#if UNITY_ANDROID
            Application.Quit();
#elif UNITY_EDITOR
UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        public void ViewSourceCode()
        {
            // AndroidToast.ShowAndroidToast("Not yet available");
            // return;
        
            Application.OpenURL("https://github.com/alexisstrat/spooky-forest-unity");
        }

        #region Event Booleans
    
        void OnLevelStartReady()
        {
            Debug.Log("LevelStart <color=green>Ready</color>");
            _levelStartIsReady = true;
        }
    
        void OnCameraReady()
        {
            Debug.Log("Camera <color=green>Ready</color>");
            _cameraIsReady = true;
        }
    
        void OnPlayerReady()
        {
            Debug.Log("Player <color=green>Ready</color>");
            _playerIsReady = true;
        }

        void OnLevelReady()
        {
            Debug.Log("Level <color=green>Ready</color>");
            _levelIsReady = true;
        }

        void OnFogReady()
        {
            Debug.Log("Fog <color=green>Ready</color>");
            _fogIsReady = true;
        }

        void OnUiReady()
        {
            Debug.Log("UI <color=green>Ready</color>");
            _uiIsReady = true;
        }
    
        #endregion

        /// <summary>
        /// Unsubscribe from events
        /// </summary>
        private void OnDestroy()
        {
            onGameOverEvent.RemoveAllListeners();
            DeviceOrientationManager.OnScreenOrientationChanged -= SetCameraAndUI;
            guiManager.UiReadyEvent -= OnUiReady;
            _playerController.PlayerReadyEvent -= OnPlayerReady;
            levelGenerator.LevelReadyEvent -= OnLevelReady;
            _cameraBehaviour.CameraReadyEvent -= OnCameraReady;
            fogManager.FogReadyEvent -= OnFogReady;
        }
    }
    
    [Serializable]
    public class GameOverEvent : UnityEvent<string>{}
}
