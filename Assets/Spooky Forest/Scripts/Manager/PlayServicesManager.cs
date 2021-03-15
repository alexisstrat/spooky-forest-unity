using System;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Spooky_Forest.Scripts.Native;
using UnityEngine;

namespace Spooky_Forest.Scripts.Manager
{
    public class PlayServicesManager : MonoBehaviour
    {
        private const string LeaderboardId = GPGSIds.leaderboard_spooky_forest_leaderboard;
        
        // Start is called before the first frame update
        void Start()
        {
            InitializePlayServices();
        }

        void InitializePlayServices()
        {
            try
            {
                PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
                PlayGamesPlatform.InitializeInstance(config);
                PlayGamesPlatform.DebugLogEnabled = true;
                PlayGamesPlatform.Activate();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        void AuthenticateUser()
        {
            PlayGamesPlatform.Instance.localUser.Authenticate(success =>
            {
                if (success)
                {
                    ShowLeaderboard();
                    GetScores();
                }
                else
                {
                    AndroidToast.ShowAndroidToast("Error");
                    Debug.Log("Authentication Failed");
                }
            });
        }

        public void AddScoreToLeaderboard(int highscore)
        {
            if(!HasInternet()) return;
            
            if (Social.localUser.authenticated)
            {
                Social.ReportScore(highscore, LeaderboardId, success => { });
            }
        }

        public void ShowLeaderboard()
        {
            if (!HasInternet())
            {
                AndroidToast.ShowAndroidToast("Network Access is required for Leaderboard");
                return;
            }
            
            if (Social.localUser.authenticated)
            {
                StartCoroutine(ShowLeaderboardRoutine());
            }
            else
            {
                AuthenticateUser();
            }
        }

        private IEnumerator ShowLeaderboardRoutine()
        {
            yield return new WaitForEndOfFrame();
            PlayGamesPlatform.Instance.ShowLeaderboardUI (LeaderboardId);
        }

        private void GetScores()
        {
            PlayGamesPlatform.Instance.LoadScores(LeaderboardId, LeaderboardStart.PlayerCentered,
            1, LeaderboardCollection.Public, LeaderboardTimeSpan.AllTime, data =>
            {
                CheckHighScore(data);
                Debug.Log (data.Valid);
                Debug.Log (data.Id);
                Debug.Log (data.PlayerScore.userID);
                Debug.Log (data.PlayerScore.formattedValue);
            });
        }

        private void CheckHighScore(LeaderboardScoreData data)
        {
            int playerScoreValue = (int) data.PlayerScore.value;
            if (GameManager.Get.gameData.HighScore <= playerScoreValue)
            {
                GameManager.Get.gameData.HighScore = playerScoreValue;
                GameManager.Get.guiManager.highscoreText.text = "top " +  playerScoreValue;
            }
            else
            {
                AddScoreToLeaderboard(GameManager.Get.gameData.HighScore);
            }
        }

        private bool HasInternet()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }
    }
}
