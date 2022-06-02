#if UNITY_2019_3_OR_NEWER
using System;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.Inspector;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(DrawTypenameAttribute))]
    internal class DrawTypenameDrawer : SerializeReferenceDrawer<DrawTypenameAttribute>
    {
        protected override void DrawContent(in Rect position, SerializedProperty property)
        {
            string assignedTypeName = property.managedReferenceFullTypename;
            bool nullRef = !property.HasManagedReferenceValue();
            string label = nullRef ? "Null" : ButtonLabel();

            GUI.color = nullRef ? Colours.Orange : Colours.Lime;
            GUI.Label(position, label);
            GUI.color = Colours.White;

            string ButtonLabel()
            {
                Type assignedType = EditorUtilityExt.GetTypeFromSerializedPropertyTypename(assignedTypeName);
                return ObjectNames.NicifyVariableName(assignedType.Name);
            }
        }
    }
}
#endif
