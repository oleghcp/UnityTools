using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    public class ExtendedButton : Button
    {
        private readonly ExtendedClickedEvent _onClick = new ExtendedClickedEvent();

        public UnityEvent<PointerEventData> OnClickDetailed => _onClick;

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            _onClick.Invoke(eventData);
        }

        private class ExtendedClickedEvent : UnityEvent<PointerEventData> { }
    }
}
