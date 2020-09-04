using UnityUtility.Collections;
using UnityEditor;
using UnityEngine;
using UnityUtilityEditor.Window;

namespace UnityUtilityEditor.Drawers
{
    internal abstract class BitArrayMaskWindow : EditorWindow
    {
        public abstract void SetUp(object param);

        public static T Create<T>() where T : BitArrayMaskWindow
        {
            return GetWindow(typeof(T), true, "Bit Array Mask Values") as T;
        }
    }

    [CustomPropertyDrawer(typeof(BitArrayMask))]
    internal class BitArrayMaskDrawer : PropertyDrawer
    {
        private SimpleBitArrayMaskWindow m_win;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Draw(position, label, ref m_win, property);
        }

        internal static void Draw<T>(Rect position, GUIContent label, ref T popup, object popupParam) where T : BitArrayMaskWindow
        {
            position.size = new Vector2(position.size.x, EditorGUIUtility.singleLineHeight);
            Rect rect = EditorGUI.PrefixLabel(position, label);
            if (GUI.Button(rect, "Edit values"))
                if (popup == null)
                    (popup = BitArrayMaskWindow.Create<T>()).SetUp(popupParam);
                else
                    popup.Focus();
        }
    }
}
