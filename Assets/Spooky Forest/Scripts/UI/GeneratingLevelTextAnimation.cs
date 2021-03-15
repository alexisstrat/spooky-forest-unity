using System;
using System.Collections;
using Spooky_Forest.Scripts.Level_Construction;
using Spooky_Forest.Scripts.Manager;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spooky_Forest.Scripts.UI
{
    public class GeneratingLevelTextAnimation : MonoBehaviour
    {
        private TMP_Text _generatingText;
        private String _defaultText = "generating level";
        private LevelGenerator _levelGenerator;
        private Coroutine _coroutine;

        void Awake()
        {
            _levelGenerator = GameManager.Get.levelGenerator;
            _levelGenerator.LevelReadyEvent += OnLevelReady;
            _generatingText = GetComponent<TMP_Text>();
        }

        /// <summary>
        /// Add dots to text to create animation
        /// In reality it does nothing...
        /// </summary>
        /// <returns></returns>
        private IEnumerator ChangeText()
        {
            yield return new WaitForSeconds(Random.Range(0.1f, 1f));

            int i = 0;
            while(i < 3)
            {
                i++;
                _generatingText.text += " . ";
                yield return new WaitForSeconds(Random.Range(0.1f, 1f));
            }

            _generatingText.text = _defaultText;
        
            yield return ChangeText();
        }

        /// <summary>
        /// On level ready event stop the coroutine and disable the text
        /// </summary>
        void OnLevelReady()
        {
            StopCoroutine(_coroutine);
            gameObject.SetActive(false);
        }
    
        private void OnEnable()
        {
            _generatingText.text = _defaultText;
            _coroutine = StartCoroutine(ChangeText());
        }
    }
}
