using Spooky_Forest.Scripts.Manager;
using Spooky_Forest.Scripts.Utility;
using UnityEditor;
using UnityEngine;

namespace Spooky_Forest.Scripts.Editor
{
    public class GameUtilities : EditorWindow
    {
        private GameManager _gameManager;
        
        [MenuItem("Spooky Forest/Game Utilities")]
        private static void ShowWindow()
        {
            var window = GetWindow<GameUtilities>();
            window.titleContent = new GUIContent("Game Utilities");
            window.Show();
        }

        private void OnGUI()
        {
            if (_gameManager == null)
            {
                _gameManager = FindObjectOfType<GameManager>();
            }

            DrawCameraFunctions();

            DrawGameFunctions();
            
            GUILayout.BeginVertical("GroupBox");;
            GUILayout.Label("Editor", EditorStyles.whiteLabel);
            if (GUILayout.Button("Clear PlayerPrefs"))
            {
                Debug.Log("Cleared PlayerPrefs");
                PlayerPrefs.DeleteKey("Save");
            }
            GUILayout.EndVertical();
        }

        private void DrawCameraFunctions()
        {
            GUILayout.BeginVertical("GroupBox");;
            EditorGUILayout.LabelField("Camera Orientation", EditorStyles.whiteLabel);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Portrait"))
            {
                if (!GameView.SizeExists(GameView.GetCurrentGroupType(), 1080, 2246))
                {
                    GameView.AddCustomSize(GameView.GameViewSizeType.FixedResolution, GameView.GetGameViewSizeGroupTypeByCurrentBuild(), 1080, 2246, "Custom Portrait");
                }
                
                GameView.SetSize(GameView.FindSize(GameView.GetCurrentGroupType(), 1080, 2246));
                GameView.UpdateZoomAreaAndParent();
                
                
                DeviceOrientationManager.CurrentOrientation = ScreenOrientation.Portrait;
                _gameManager.mainCamera.GetComponent<CameraBehaviour>().SetCameraParameters();

            }

            if (GUILayout.Button("Landscape"))
            {
                if (!GameView.SizeExists(GameView.GetCurrentGroupType(), 2246, 1080))
                {
                    GameView.AddCustomSize(GameView.GameViewSizeType.FixedResolution, GameView.GetGameViewSizeGroupTypeByCurrentBuild(), 2246, 1080, "Custom Landscape");
                }
                
                GameView.SetSize(GameView.FindSize(GameView.GetCurrentGroupType(), 2246, 1080));
                GameView.UpdateZoomAreaAndParent();

                DeviceOrientationManager.CurrentOrientation = ScreenOrientation.Landscape;
                _gameManager.mainCamera.GetComponent<CameraBehaviour>().SetCameraParameters();
            }


            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void DrawGameFunctions()
        {   
            GUILayout.BeginVertical("GroupBox");;
            GUILayout.Label("Game Specifics", EditorStyles.whiteLabel);

            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            if (GUILayout.Button("Add 20 Beans"))
            {
                _gameManager.UpdateCoinCount(20);
            }
            
            if (GUILayout.Button("Restart Game"))
            {
                _gameManager.PlayAgain();
            }
            EditorGUI.EndDisabledGroup();
            
            GUILayout.EndVertical();
        }
    }
}