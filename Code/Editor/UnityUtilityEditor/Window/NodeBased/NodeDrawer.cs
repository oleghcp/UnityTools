#if UNITY_2019_3_OR_NEWER
using UnityEditor;
using UnityEngine;
using UnityUtility;

namespace UnityUtilityEditor.Window.NodeBased
{
    public abstract class NodeDrawer
    {
        private const string LABEL = "{...}";

        protected virtual string ShortDrawingView => LABEL;

        public void OnHeaderGui(bool rootNode, SerializedProperty nameProperty, ref bool renaming)
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

        public void OnGui(SerializedProperty property, float width, bool enabled)
        {
            if (enabled)
            {
                OnGui(property, width);
                return;
            }

            EditorGUILayout.LabelField(ShortDrawingView);
        }

        public float GetHeight(SerializedProperty property, bool enabled)
        {
            if (enabled)
                return GetHeight(property);

            return EditorGUIUtility.singleLineHeight;
        }


        protected abstract void OnGui(SerializedProperty property, float width);
        protected abstract float GetHeight(SerializedProperty property);

        protected virtual Color GetHeaderColor(bool rootNode)
        {
            return rootNode ? Colours.Orange : Colours.Cyan;
        }
    }
}
#endif
