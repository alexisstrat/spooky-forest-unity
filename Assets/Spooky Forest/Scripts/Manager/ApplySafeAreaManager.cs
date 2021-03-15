using System.Collections;
using UnityEngine;

namespace Spooky_Forest.Scripts.Manager
{
    public class ApplySafeAreaManager : MonoBehaviour
    {
        public RectTransform[] panelsForSafeArea;
        Rect _lastSafeArea = new Rect (0, 0, 0, 0);

        private void Start()
        {
            _lastSafeArea = Screen.safeArea;
        }

        private void Update()
        {
            if(_lastSafeArea != Screen.safeArea)
                Refresh();
        }

        public void Refresh ()
        {
            StartCoroutine(ApplySafeArea (Screen.safeArea));
        }

        IEnumerator ApplySafeArea (Rect r)
        {
            _lastSafeArea = r;
            foreach (var panel in panelsForSafeArea)
            {
                // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
                Vector2 anchorMin = r.position;
                Vector2 anchorMax = r.position + r.size;
                anchorMin.x /= Screen.width;
                anchorMin.y /= Screen.height;
                anchorMax.x /= Screen.width;
                anchorMax.y /= Screen.height;
                panel.anchorMin = anchorMin;
                panel.anchorMax = anchorMax;
            }

            yield return null;
        }
    }
}
