using System;
using System.Collections.Generic;
using DG.Tweening;
using Spooky_Forest.Scripts.Controller;
using Spooky_Forest.Scripts.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Spooky_Forest.Scripts.Manager
{
    public class CharacterSelectionManager : MonoBehaviour
    {
        [Header("References")] 
        public Camera charactersCamera;
        public GameObject charactersUIScreen;
        public GameObject charactersPlatform;
        public TMP_Text characterNameText;
        public TMP_Text selectButton;
        public Transform beanImage;
        
        
        [Header("List of characters sold")] 
        public CharacterItem[] characterItems;

        private readonly List<GameObject> _charactersOnPlatform = new List<GameObject>();
        
        private List<float> _anglesOnPlatform;
        private float _angle; // The angle difference between each character
        private float _circleRadius; // The circle radius for the characters to spawn on the edge
        public CharacterItem selectedCharacter; // The character that is selected for the game
        private PlayerController _playerController;

        // These layers are only for the Character Selection Camera
        private LayerMask _characterAvailableLayer; 
        private LayerMask _characterNotAvailabeLayer; // This layer is not included in the light's culling mask so that the characters not yet purchased appear dark.
        
        private int _portraitCameraField = 45;
        private int _landscapeCameraField = 55;
        
        private bool _isRotating; // when switching character this is set to true

        private void Awake()
        {
            DeviceOrientationManager.OnScreenOrientationChanged += ChangeCameraField;
            _playerController = GameManager.Get.player.GetComponent<PlayerController>();
            characterItems = _playerController.characterItems;
            _anglesOnPlatform = new List<float>();
            _circleRadius = charactersPlatform.transform.localScale.x;
            _characterAvailableLayer = LayerMask.NameToLayer("CharacterSelection");
            _characterNotAvailabeLayer = LayerMask.NameToLayer("CharacterNotAvailable");
            
            PopoulatePlatform();
            ChangeCameraField(Screen.orientation);
        }
        
        private void Update()
        {
            if (!charactersUIScreen.activeInHierarchy) return;

            if (MobileInput.Get.SwipeLeft)
            {
                _isRotating = true;
                float rotateTo = charactersPlatform.transform.rotation.eulerAngles.y + _angle;
                charactersPlatform.transform.DORotate(new Vector3(0, rotateTo , 0), 0.2f).OnComplete(UpdateInfo);
            }

            if (MobileInput.Get.SwipeRight)
            {
                _isRotating = true;
                float rotateTo = charactersPlatform.transform.rotation.eulerAngles.y - _angle;
                charactersPlatform.transform.DORotate(new Vector3(0, rotateTo, 0), 0.2f).OnComplete(UpdateInfo);
            }
        }
        
        /// <summary>
        /// When we change orientation we set the appropriate field of view of the camera to
        /// compensate on render texture
        /// </summary>
        /// <param name="screenOrientation"></param>
        private void ChangeCameraField(ScreenOrientation screenOrientation)
        {
            charactersCamera.fieldOfView = screenOrientation == ScreenOrientation.Portrait
                ? _portraitCameraField
                : _landscapeCameraField;
        }


        /// <summary>
        /// We spawn each character along the radius of a circle and add the angle position in
        /// the _anglesOnPlatform list to determine which character we are facing
        /// </summary>
        private void PopoulatePlatform()
        {
            _angle = 360f / characterItems.Length;

            for (int i = 0; i < characterItems.Length; i++)
            {
                CharacterItem ci = characterItems[i];
                
                Quaternion rotation = Quaternion.AngleAxis(i * _angle, Vector3.up);

                if (rotation.eulerAngles.y == 0)
                {
                    _anglesOnPlatform.Add(rotation.eulerAngles.y);

                }
                else
                {
                    _anglesOnPlatform.Add(360 - rotation.eulerAngles.y);
                }
                
                Vector3 direction = rotation * Vector3.forward;
                Vector3 position = charactersPlatform.transform.position + (direction * _circleRadius);
                GameObject characterModel = Instantiate(ci.model, position, rotation, charactersPlatform.transform);

                characterModel.SetActive(true);
                SetLayers(characterModel, ci);
                _charactersOnPlatform.Add(characterModel);
                characterModel.name = ci.characterName;
            }

            SaveManager.Save data = GameManager.Get.gameData;
            
            SetActiveCharacterInFront(data);
        }

        public void SetActiveCharacterInFront(SaveManager.Save data)
        {
            // Set the active character of the game as the default selection in the selection screen
            for (int i = 0; i < characterItems.Length; i++)
            {
                if (characterItems[i].characterId == data.SelectedCharacter)
                {
                    charactersPlatform.transform.rotation = Quaternion.Euler(0, _anglesOnPlatform[i], 0);
                    selectedCharacter = characterItems[i];
                    selectButton.text = "active";
                    beanImage.gameObject.SetActive(false);
                    characterNameText.text = characterItems[i].characterName;
                    break;
                }
            }
        }

        /// <summary>
        /// We check if the character is purchased and set him to the appropriate layer
        /// </summary>
        /// <param name="characterModel"></param>
        /// <param name="characterItem"></param>
        private void SetLayers(GameObject characterModel, CharacterItem characterItem)
        {
            if(GameManager.Get.gameData.UnlockedCharacters.Contains(characterItem.characterId))
            {
                SetAvailableLayer(characterModel);
            }
            else
            {
                SetNotAvailableLayer(characterModel);   
            }
        }

        /// <summary>
        /// We set the character to the available layer.
        /// Again this is done for lights.
        /// </summary>
        /// <param name="characterModel"></param>
        private void SetAvailableLayer(GameObject characterModel)
        {
            characterModel.layer = _characterAvailableLayer;
            foreach (Transform child in characterModel.GetComponentsInChildren<Transform>())
            {
                child.gameObject.layer = _characterAvailableLayer;
            }
        }

        /// <summary>
        /// We set the character to the NOT availble layer so he appears gray
        /// </summary>
        /// <param name="characterModel"></param>
        private void SetNotAvailableLayer(GameObject characterModel)
        {
            characterModel.layer = _characterNotAvailabeLayer;
            foreach (Transform child in characterModel.GetComponentsInChildren<Transform>())
            {
                child.gameObject.layer = _characterNotAvailabeLayer;
            }
        }

        /// <summary>
        /// Upon swipping and changing character we get the character and set
        /// the appropriate texts and info on UI
        /// </summary>
        void UpdateInfo()
        {
            for (int i = 0; i < _anglesOnPlatform.Count; i++)
            {
                if (Mathf.RoundToInt(_anglesOnPlatform[i] - charactersPlatform.transform.rotation.eulerAngles.y) == 0)
                {
                    selectedCharacter = characterItems[i];
                    characterNameText.text = characterItems[i].characterName;

                    if (GameManager.Get.gameData.UnlockedCharacters.Contains(characterItems[i].characterId))
                    {
                        if (GameManager.Get.gameData.SelectedCharacter == characterItems[i].characterId)
                        {
                            selectButton.text = "active";
                        }
                        else
                        {
                            selectButton.text = "select";
                        }
                        selectButton.transform.parent.GetChild(1).gameObject.SetActive(false);
                    }
                    else
                    {
                        selectButton.text = "buy for " +characterItems[i].cost;
                        beanImage.gameObject.SetActive(true);
                        selectButton.ForceMeshUpdate();
                        float beanPositionX = selectButton.transform.position.x + selectButton.preferredWidth / 2 +
                                              beanImage.GetComponent<RectTransform>().rect.width;
                        beanImage.position = new Vector3(beanPositionX,
                            beanImage.position.y, beanImage.position.z);
                            
                    }
                    break;
                }
            }

            _isRotating = false;
        }

        /// <summary>
        /// Selection of character. If we have bought it then restart the game
        /// </summary>
        public void TrySelectCharacter()
        {
            if(String.Equals(selectButton.text, "active") || _isRotating) return;

            if (GameManager.Get.gameData.UnlockedCharacters.Contains(selectedCharacter.characterId))
            {
                GameManager.Get.gameData.SelectedCharacter = selectedCharacter.characterId;
                selectButton.text = "active";
                charactersUIScreen.SetActive(false);
                GameManager.Get.PlayAgain();
            }
            else
            {
                TryBuyCharacter();
            }
        }

        /// <summary>
        /// If we press buy, check if we can buy the character and deduct the cost of his from
        /// our coins.
        /// </summary>
        private void TryBuyCharacter()
        {
            if (selectedCharacter.cost <= GameManager.Get.gameData.Coins)
            {
                GameManager.Get.BuyCharacter(selectedCharacter.characterId, selectedCharacter.cost);

                foreach (var character in _charactersOnPlatform)
                {
                    if (character.name == selectedCharacter.characterName)
                    {
                        SetAvailableLayer(character);
                        break;
                    }
                }
                
                selectButton.text = "select";
                beanImage.gameObject.SetActive(false);
            }
            else
            {
                selectButton.text = "not enough";
                selectButton.ForceMeshUpdate();
                float beanPositionX = selectButton.transform.position.x + selectButton.preferredWidth / 2 +
                                      beanImage.GetComponent<RectTransform>().rect.width * 1.5f;
                beanImage.position = new Vector3(beanPositionX,
                    beanImage.position.y, beanImage.position.z);
            }
        }

        public void SetCharactersVariables(SaveManager.Save gameData)
        {
            foreach (var characterItem in characterItems)
            {
                if (gameData.SelectedCharacter == characterItem.characterId)
                {
                    selectedCharacter = characterItem;
                }
            }
        }
    }
}
