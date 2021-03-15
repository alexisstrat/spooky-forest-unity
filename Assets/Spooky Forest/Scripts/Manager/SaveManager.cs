using System.Collections.Generic;
using UnityEngine;

namespace Spooky_Forest.Scripts.Manager
{
    public class SaveManager : MonoBehaviour
    {
        /// <summary>
        /// Class that holds the game data
        /// </summary>
        public class Save
        {
            public bool TutorialCompleted;
            public int HighScore;
            public int Coins;
            public bool Audio;
            public bool Shadows;
            public bool HighDetail;
            public bool LockRotation;
            public int SelectedCharacter;
            public List<int> UnlockedCharacters;
        }
        
        private Save _save = new Save();
        
        /// <summary>
        /// Retrieves the game data from the user preferences which are
        /// saved as json
        /// </summary>
        /// <returns>the json file with the game data</returns>
        public Save LoadSavedData()
        {
            if (PlayerPrefs.HasKey("Save"))
            {
                string saveJson = PlayerPrefs.GetString("Save");
                _save = JsonUtility.FromJson<Save>(saveJson);
            }
            else
            {
                _save.TutorialCompleted = false;
                _save.HighScore = 0;
                _save.Coins = 0;
                _save.Audio = true;
                _save.Shadows = true;
                _save.HighDetail = true;
                _save.LockRotation = false;
                _save.UnlockedCharacters = new List<int>();
                _save.UnlockedCharacters.Add(1);
                _save.SelectedCharacter = 1;
                SaveData(_save);
            }

            return _save;
        }

        /// <summary>
        /// Saves the game data into a json file and into
        /// playerprefs
        /// </summary>
        /// <param name="gameData">the actual game data</param>
        public void SaveData(Save gameData)
        { 
            string saveJson = JsonUtility.ToJson((gameData));
            PlayerPrefs.SetString("Save", saveJson);
            PlayerPrefs.Save();
        }
    }
}
