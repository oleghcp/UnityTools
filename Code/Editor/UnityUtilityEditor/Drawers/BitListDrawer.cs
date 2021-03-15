using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.Collections;

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
