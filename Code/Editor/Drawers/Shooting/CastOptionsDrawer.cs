#if INCLUDE_PHYSICS || INCLUDE_PHYSICS_2D
using OlegHcp.Shooting;
using OlegHcpEditor.Engine;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Drawers.Shooting
{
    [CustomPropertyDrawer(typeof(CastOptions))]
    internal class CastOptionsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.FindPropertyRelative(nameof(CastOptions.CastRadius)).Draw(position, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
#endif
