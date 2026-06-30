using UnityEngine;
using UnityEngine.EventSystems;

namespace HexResourceTracker.Core
{
    internal class ResourceTrackerDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        private RectTransform _rectTransform;
        private Vector2 _startPanelPosition;
        private Vector2 _startMousePosition;

        private void Awake()
        {
            _rectTransform = transform.parent as RectTransform;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_rectTransform == null)
            {
                return;
            }

            _startPanelPosition = _rectTransform.anchoredPosition;
            _startMousePosition = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_rectTransform == null)
            {
                return;
            }

            Vector2 mouseDelta = eventData.position - _startMousePosition;
            _rectTransform.anchoredPosition = _startPanelPosition + mouseDelta;
        }
    }
}
