using System.Collections;
using UnityEngine;

namespace Spooky_Forest.Scripts
{
    public class MobileInput : MonoBehaviour {

        public static MobileInput Get { private set; get; }

        private bool _tap, _liftedF, _swipeLeft, _swipeRight, _swipeUp ,_swipeDown;
        private Vector2 _swipeDelta, _startTouch;
        private float DEADZONE = 80f;

        // properties
        public bool Tap => _tap;
        public bool LiftedF => _liftedF;
        public Vector2 SwipeDelta => _swipeDelta;
        public bool SwipeLeft => _swipeLeft;
        public bool SwipeRight => _swipeRight;
        public bool SwipeUp => _swipeUp;
        public bool SwipeDown => _swipeDown;

        public Queue inputs;

        private void Awake()
        {
            inputs = new Queue();
            Get = this;
        }
    
	
        // Update is called once per frame
        void Update () {

            // Reseting all the booleans
            _tap = _liftedF = _swipeLeft = _swipeRight = _swipeUp = _swipeDown = false;
            
            #region Standalone Inputs
            #if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                _tap = true;

                _startTouch = Input.mousePosition;
                inputs.Enqueue(Input.GetMouseButtonDown(0));
            }
            else if (Input.GetMouseButtonUp(0))
            {
                inputs.Dequeue();
                _liftedF = true;
                _startTouch = _swipeDelta = Vector2.zero;
            }
            #endif
            #endregion

            #region Mobile Inputs
            if (Input.touches.Length != 0)
            {
                if(Input.touches[0].phase == TouchPhase.Began)
                {
                    _tap = true;
                    _startTouch = Input.mousePosition;
                }
                else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
                {
                    _liftedF = true;
                    _startTouch = _swipeDelta = Vector2.zero;
                }

            }
            #endregion

            // Calculate distance
            _swipeDelta = Vector2.zero;
            if(_startTouch != Vector2.zero)
            {
                // Let's check with mobile
                if (Input.touches.Length != 0)
                {
                    _swipeDelta = Input.touches[0].position - _startTouch;
                }
                // Let's check with standalone
                else if (Input.GetMouseButton(0))
                {
                    _swipeDelta = (Vector2) Input.mousePosition - _startTouch;
                }
            }

            // Check if we're beyond the deadzone
            if(_swipeDelta.magnitude > DEADZONE)
            {
                // This is a confirmed swipe
                float x = _swipeDelta.x;
                float y = _swipeDelta.y;

                if(Mathf.Abs(x) > Mathf.Abs(y))
                {
                    // Left or Right
                    if (x < 0)
                        _swipeLeft = true;
                    else
                        _swipeRight = true;
                }
                else
                {
                    // Up or Down
                    if (y < 0)
                        _swipeDown = true;
                    else
                        _swipeUp = true;
                }

                _startTouch = _swipeDelta = Vector2.zero;
            }
        }
    }
}
