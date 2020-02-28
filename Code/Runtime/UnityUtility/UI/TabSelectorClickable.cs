using UnityEngine;
using UnityEngine.EventSystems;

namespace UU.UI
{
    [DisallowMultipleComponent]
    public class TabSelectorClickable : AbstractTabSelector, IPointerClickHandler
    {
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (!Application.isMobilePlatform || eventData.pointerId == 0)
                OnClick();
        }
    }
}
