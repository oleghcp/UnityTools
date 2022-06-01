#if !UNITY_2019_2_OR_NEWER || INCLUDE_UNITY_UI
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityUtility.UI
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
#endif
