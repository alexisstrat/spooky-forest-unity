using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json;
using Spooky_Forest.Scripts.Data;
using Spooky_Forest.Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Spooky_Forest.Scripts.Manager
{
    public class GuiManager : MonoBehaviour
    {
        public GameObject characterSelectionCamera;
        public ApplySafeAreaManager applySafeAreaManager;
        
        [Header("Main Info")]
        public TMP_Text distance;
        public TMP_Text coins;
        public TMP_Text highscoreText;
        public TMP_Text title;
        public GameObject titleScreen;
        public GameObject tapToStartPlaying;
        
        [Header("Miscellaneous")]
        public Image loadingImage;
        public GameObject tutorialHand;
        public SkullAnimation skull1;
        public SkullAnimation skull2;
        public TMP_Text gameOverText;
        public TMP_Text versionTextP;
        public TMP_Text versionTextL;
        public Dictionary<string, List<string>> gameOverMessages = new Dictionary<string, List<string>>();

        [Header("UI Screens")] 
        public GameObject backgroundPanel;
        public GameObject mainInfo;
        public GameObject mainButtons;
        public GameObject gameOverScreen;
        public GameObject pauseScreen;
        public GameObject characterSelectionScreen;
        public GameObject creditsPortrait;
        public GameObject creditsLandscape;

        [Header("Audio")] public AudioClip buttonUiPress;
        
        public delegate void UiDelegate();
        public event UiDelegate UiReadyEvent;

        public GameObject activeScreen;

        private Vector3 _tutHandStartPos;
        private string _highscoreKey = "highscore";

        private void Awake()
        {
            DeviceOrientationManager.OnScreenOrientationChanged += OrientationChanged;
            LoadGameOverMessages();
        }

        // Start is called before the first frame update
        void Start()
        {
            GameManager.Get.onGameOverEvent.AddListener(SetGameOverMessage);

            _tutHandStartPos = tutorialHand.transform.position;
            versionTextP.text = "v"+Application.version;
            versionTextL.text = "v"+Application.version;
            
            SetUi();
        }

        private void LoadGameOverMessages()
        {
            TextAsset targetFile = Resources.Load<TextAsset>("GameOverMessages");
            GameOverMessages items = new GameOverMessages();
            items = JsonConvert.DeserializeObject<GameOverMessages>(targetFile.text);
            
            foreach (var item in items.gameOverMessages)
            {
                gameOverMessages.Add(item.identifier, item.messages);
            }
        }

        /// <summary>
        /// Setting All UI on game start and restart
        /// </summary>
        public void SetUi()
        {
            characterSelectionCamera.SetActive(false);
            highscoreText.text = "top " + GameManager.Get.gameData.HighScore.ToString();
            highscoreText.gameObject.SetActive(true);
            coins.text = GameManager.Get.gameData.Coins.ToString();
            coins.transform.parent.GetComponent<RectTransform>().SetParent(mainInfo.transform);
            mainButtons.SetActive(true);
            tapToStartPlaying.SetActive(true);
            titleScreen.SetActive(true);
            skull1.SetStartingPosition(title, 2);
            skull2.SetStartingPosition(title, 3);
            gameOverScreen.SetActive(false);
            backgroundPanel.SetActive(false);
            loadingImage.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            loadingImage.gameObject.SetActive(true);
            var tempcolor = loadingImage.color;
            tempcolor.a = 1f;
            loadingImage.color = tempcolor;
            distance.text = "0";
            distance.transform.localScale = Vector3.one;
            gameOverText.fontSize = DeviceOrientationManager.CurrentOrientation == ScreenOrientation.Portrait ||
                                    DeviceOrientationManager.CurrentOrientation == ScreenOrientation.PortraitUpsideDown
                ? 100
                : 80;
            
            if (!GameManager.Get.gameData.TutorialCompleted)
            {
                tutorialHand.SetActive(true);
                TapHand();
            }
            else
            {
                tutorialHand.SetActive(false);
            }

            OrientationChanged(Screen.orientation);

            applySafeAreaManager.Refresh();

            OnUiReady();
        }

        /// <summary>
        /// Event fired that UI is ready
        /// </summary>
        void OnUiReady()
        {
            UiReadyEvent?.Invoke();
        }

        /// <summary>
        /// Fade the black background image
        /// </summary>
        public void FadeLoadingImage()
        {
            loadingImage.DOFade(0, .5f).OnComplete(DisableLoadingImage);
        }
        
        private void DisableLoadingImage()
        {
            loadingImage.gameObject.SetActive(false);
        }

        /// <summary>
        /// Opens pauses UI
        /// </summary>
        /// <param name="open"></param>
        public void EnablePauseScreen(bool open)
        {
            pauseScreen.SetActive(open);
        }
        
        /// <summary>
        /// We set the coins text according to our game data
        /// </summary>
        public void UpdateCoinsText()
        {
            coins.text = GameManager.Get.gameData.Coins.ToString();
        }

        /// <summary>
        /// We set the distance text according to our game data
        /// </summary>
        public void UpdateDistanceText()
        {
            distance.text = GameManager.Get.CurrentDistance.ToString();
        }

        /// <summary>
        /// Starting the game and disabling some of the UI
        /// </summary>
        public void StartPlay()
        {
            mainButtons.SetActive(false);
            titleScreen.SetActive(false);
            tapToStartPlaying.SetActive(false);
            highscoreText.gameObject.SetActive(false);
        }

        public void SwipeHand()
        {
            DOTween.Kill(tutorialHand.transform, false);
            tutorialHand.transform.DOLocalMoveX(1f, 1f).From()
                .SetLoops(-1, LoopType.Restart)
                .OnUpdate(() =>
                {
                    if (Math.Abs(tutorialHand.transform.position.x + 1) < 0.2f)
                    {
                        Debug.Log("Completed Tutorial");
                        tutorialHand.SetActive(false);
                        GameManager.Get.TutorialCompleted();
                        DOTween.Kill(tutorialHand.transform, false);
                    }
                });
        }

        public void TapHand()
        {
            DOTween.Kill(tutorialHand.transform, false);
            tutorialHand.transform.position = new Vector3(_tutHandStartPos.x, _tutHandStartPos.y, tutorialHand.transform.position.z);
            tutorialHand.transform.DOMoveY(.5f, .5f).From()
                .SetLoops(-1, LoopType.Yoyo)
                .OnUpdate(() =>
                {
                    var handPos = tutorialHand.transform.position;
                    if (Math.Abs(handPos.z) < 0.1f && Math.Abs(handPos.x) < 0.1f)
                    {
                        Debug.Log("2");
                
                        SwipeHand();
                    }
                });
        }

        /// <summary>
        /// Enable UI gameover
        /// </summary>
        public void GameOver()
        {
            gameOverScreen.SetActive(true);
            mainButtons.SetActive(true);
        }

        /// <summary>
        /// We open the passed screen. If it's the character selection screen we set the parent of the
        /// coin text to that screen so it appears in front.
        /// </summary>
        public void OpenGuiScreen(GameObject guiScreen)
        {
            StartCoroutine(AudioManager.Get.PlaySfxWithVolumeCoroutine(buttonUiPress, 2.0f));
            // Move coins into character selection screen
            if (guiScreen == characterSelectionScreen)
            {
                GameManager.Get.characterSelectionManager.SetActiveCharacterInFront(GameManager.Get.gameData);
                coins.transform.parent.GetComponent<RectTransform>().SetParent(characterSelectionScreen.transform);
                characterSelectionCamera.SetActive(true);
                coins.gameObject.SetActive(true);
            }
            
            activeScreen = guiScreen;
            backgroundPanel.SetActive(true);
            guiScreen.SetActive(true);
        }

        public void OrientationChanged(ScreenOrientation screenOrientation)
        {
            StartCoroutine(OrientationChangedRoutine(screenOrientation));
        }

        private IEnumerator OrientationChangedRoutine(ScreenOrientation screenOrientation)
        {
            yield return new WaitForSeconds(DeviceOrientationManager.ORIENTATION_CHECK_INTERVAL);
            applySafeAreaManager.Refresh();
            gameOverText.fontSize = DeviceOrientationManager.CurrentOrientation == ScreenOrientation.Portrait ||
                                    DeviceOrientationManager.CurrentOrientation == ScreenOrientation.PortraitUpsideDown
                ? 100
                : 80;
            creditsLandscape.SetActive(screenOrientation != ScreenOrientation.Portrait);
            creditsPortrait.SetActive(screenOrientation == ScreenOrientation.Portrait);
        }

        /// <summary>
        /// We close the active screen and if we opened character selection, parent the coins
        /// text where it belongs!
        /// </summary>
        public void CloseActiveScreen()
        {
            StartCoroutine(AudioManager.Get.PlaySfx(buttonUiPress));
            // Return coins to main ui
            if (coins.rectTransform.parent.parent != mainInfo.transform)
            {
                coins.gameObject.SetActive(false);
                coins.transform.parent.GetComponent<RectTransform>().SetParent(mainInfo.transform);
                characterSelectionCamera.SetActive(false);
                coins.gameObject.SetActive(true);
            }
            backgroundPanel.SetActive(false);
            activeScreen.SetActive(false);
            activeScreen = null;
        }
        
        public void SetHighscoreText(bool isHighscore = false)
        {
            if (!isHighscore)//(GameManager.Get.CurrentDistance <= GameManager.Get.gameData.HighScore)
            {
                highscoreText.gameObject.SetActive(true);
            }
            else
            {
                distance.transform.DOScale(Vector3.one * 1.5f, .2f).SetEase(Ease.OutBounce); // Scaling the distance counter to indicate a new highscoreText
                highscoreText.gameObject.SetActive(false);
            }        
        }

        public void SetGameOverMessage(string identifier)
        {
            StartCoroutine(SetGameOverMessageRoutine(identifier));
        }

        public IEnumerator SetGameOverMessageRoutine(string identifier)
        {
            yield return new WaitForEndOfFrame();
            if (GameManager.Get.isHighscore)
            {
                identifier = _highscoreKey;
            }
            
            var list = gameOverMessages[identifier];
            gameOverText.text = list[Random.Range(0, list.Count)];

            Debug.Log("Game over message for <color=cyan>"+ identifier +"</color>");

        }

        private void OnDestroy()
        {
            DeviceOrientationManager.OnScreenOrientationChanged -= OrientationChanged;
        }
    }
}
