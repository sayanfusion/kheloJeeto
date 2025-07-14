using UnityEngine;
using UnityEngine.EventSystems;

namespace DevCommon
{
    public static class PointerEventDataExtensions
    {
        public static Vector3 GetWorldPointerPosition(this PointerEventData eventData)
        {
            if (eventData.pointerEnter == null)
                return Vector3.zero;

            var draggingPlane = (RectTransform) eventData.pointerEnter.transform;
            return RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingPlane,
                eventData.position, eventData.pressEventCamera, out var globalMousePosition)
                ? globalMousePosition
                : eventData.pointerEnter.transform.position;
        }

        public static Vector3 GetLocalPointerPosition(this PointerEventData eventData, RectTransform transform)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform,
                eventData.position, eventData.pressEventCamera, out var clickPosition);
            return new Vector3(clickPosition.x, clickPosition.y);
        }

        public static Vector3 GetLocalPointerPosition(this PointerEventData eventData, Transform transform)
        {
            return GetLocalPointerPosition(eventData, (RectTransform) transform);
        }
    }
}