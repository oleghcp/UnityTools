using UnityEditor;
using UnityEngine;
using UnityUtility.Collections;
using UnityUtilityEditor.Window.BitArrays;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(BitList))]
    internal class BitArrayDrawer : PropertyDrawer
    {
        private SimpleBitArrayMaskWindow _window;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Draw(position, label, ref _window, property);
        }

        public static void Draw<T>(Rect position, GUIContent label, ref T window, object popupParam) where T : BitArrayMaskWindow
        {
            position.size = new Vector2(position.size.x, EditorGUIUtility.singleLineHeight);
            Rect rect = EditorGUI.PrefixLabel(position, label);
            if (GUI.Button(rect, "Edit values"))
                if (window == null)
                    (window = BitArrayMaskWindow.Create<T>()).SetUp(popupParam);
                else
                    window.Focus();
        }
    }

    internal abstract class BitArrayMaskWindow : EditorWindow
    {
        public abstract void SetUp(object param);

        public static T Create<T>() where T : BitArrayMaskWindow
        {
            return GetWindow<T>(true, "Bit Array Values");
        }
    }
}
