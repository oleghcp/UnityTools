#if UNITY_2019_3_OR_NEWER
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtilityEditor.Window.NodeBased.Stuff;

namespace UnityUtilityEditor.Window.NodeBased
{
    public class NodeDrawer
    {
        private const string LABEL = "{...}";

        internal GraphMap Map;

        protected virtual string ShortDrawingView => LABEL;

        internal void OnHeaderGui(bool rootNode, SerializedProperty nameProperty, ref bool renaming)
        {
            if (renaming)
            {
                GUILayout.BeginHorizontal();
                nameProperty.stringValue = EditorGUILayout.TextField(nameProperty.stringValue);
                if (GUILayout.Button("V", GUILayout.Width(EditorGuiUtility.SmallButtonWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                    renaming = false;
                GUILayout.EndHorizontal();
                return;
            }

            GUI.color = GetHeaderColor(rootNode);
            EditorGUILayout.LabelField(nameProperty.stringValue, EditorStylesExt.Rect);
            GUI.color = Colours.White;
        }

        internal void OnGui(SerializedProperty property, float width, bool enabled)
        {
            if (enabled)
            {
                OnGui(property, width);
                return;
            }

            EditorGUILayout.LabelField(ShortDrawingView);
        }

        internal float GetHeight(SerializedProperty property, bool enabled)
        {
            if (enabled)
                return GetHeight(property);

            return EditorGUIUtility.singleLineHeight;
        }

        protected virtual void OnGui(SerializedProperty property, float width)
        {
            EditorGUIUtility.labelWidth = width * 0.5f;
            foreach (SerializedProperty item in property.EnumerateInnerProperties())
            {
                if (!Map.IsServiceField(item))
                    EditorGUILayout.PropertyField(item, true);
            }
        }

        protected virtual float GetHeight(SerializedProperty property)
        {
            return EditorGuiUtility.GetDrawHeight(property, Map.IsServiceField);
        }

        protected virtual Color GetHeaderColor(bool rootNode)
        {
            return rootNode ? Colours.Orange : Colours.Cyan;
        }
    }
}
#endif
