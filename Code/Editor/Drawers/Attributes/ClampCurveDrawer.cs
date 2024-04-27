using OlegHcp.Inspector;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Drawers.Attributes
{
    [CustomPropertyDrawer(typeof(ClampCurveAttribute))]
    internal class ClampCurveDrawer : AttributeDrawer<ClampCurveAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorUtilityExt.GetFieldType(this) != typeof(AnimationCurve))
            {
                EditorGui.ErrorLabel(position, label, $"Use {nameof(ClampCurveAttribute)} with {nameof(AnimationCurve)}.");
                return;
            }

            EditorGUI.CurveField(position, property, attribute.Color, attribute.Bounds, label);
        }
    }
}
