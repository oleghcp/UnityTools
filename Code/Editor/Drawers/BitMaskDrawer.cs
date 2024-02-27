using UnityEditor;
using UnityEngine;
using OlegHcp;
using OlegHcp.Collections;
using OlegHcp.Inspector;
using OlegHcpEditor.Engine;

namespace OlegHcpEditor.Drawers
{
    [CustomPropertyDrawer(typeof(BitList))]
    [CustomPropertyDrawer(typeof(IntMask))]
    internal class BitMaskDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGui.ErrorLabel(position, label, $"Use {nameof(DrawFlagsAttribute)}.");
        }
    }
}
