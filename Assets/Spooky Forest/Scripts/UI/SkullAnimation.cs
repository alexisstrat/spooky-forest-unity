using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Spooky_Forest.Scripts.UI
{
    public class SkullAnimation : MonoBehaviour {

        public RectTransform rectTransform;
        private Button button;
        private Vector3 _startPos;
        private Vector3 _startPosLandscape;

        public float yOffset = 10f;
        public float dropDuration = 1f;
        public float delay = 1f;
        public Ease ease = Ease.OutBounce;

        public float punchDuration = 1f;

        [Range(0, 10)]
        public int vibrato;

        [Range(0, 1)]
        public float elasticity;

        private Coroutine _letterPos;


        // Use this for initialization
        void Start () {
            button = GetComponent<Button>();
            rectTransform = GetComponent<RectTransform>();
        }

        public void pressedSkull()
        {
            button.interactable = false;
            rectTransform.DOPunchAnchorPos(new Vector2(0, yOffset), punchDuration, vibrato, elasticity).OnComplete(enableButton);
        }

        void enableButton()
        {
            button.interactable = true;
        }

        void PlayAnimation()
        {
            rectTransform.DOAnchorPosY(-45, dropDuration).SetEase(ease).SetDelay(delay);
        }

        public void SetStartingPosition(TMP_Text title, int character)
        {
            if(_letterPos != null)
                StopCoroutine(_letterPos);
        
            _letterPos = StartCoroutine(FindLetterPosition(title, character));
        }
    
        private IEnumerator FindLetterPosition(TMP_Text title, int character)
        {
            TMP_TextInfo textInfo;
            while (title.textInfo.characterInfo[character].bottomLeft == Vector3.zero || title.textInfo.characterInfo[character].topRight == Vector3.zero)
            {
                yield return null;
            }

            textInfo = title.textInfo;

            Vector3 bottomLeft = title.GetComponent<Transform>().TransformPoint(new Vector3(textInfo.characterInfo[character].bottomLeft.x, textInfo.characterInfo[character].descender, 0));
            Vector3 topRight = title.GetComponent<Transform>().TransformPoint(textInfo.characterInfo[character].topRight);

            Vector3 difference = (topRight - bottomLeft);
            Vector3 middlePoint = bottomLeft + difference * 0.5f;
        
            _startPos = new Vector3(middlePoint.x, Screen.height + rectTransform.rect.size.y);;
            ResetAnimation();
            PlayAnimation();
        }
    
        public void ResetAnimation()
        {
            transform.position = _startPos;
        }
    }
}
