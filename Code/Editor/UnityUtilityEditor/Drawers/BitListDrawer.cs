using UnityEditor;
using UnityEngine;
using UnityUtility.Collections;
using UnityUtility.Inspector;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(BitList))]
    internal class BitListDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGui.ErrorLabel(position, label, $"Use {nameof(DrawFlagsAttribute)}.");
        }
    }
}
