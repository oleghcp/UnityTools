#if UNITY_2019_3_OR_NEWER
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityUtility.NodeBased;

namespace UnityUtilityEditor.Window.NodeBased.Stuff
{
    internal class GraphSidePanel
    {
        public const float WIDTH = 300f;

        private GraphEditorWindow _window;
        private HashSet<string> _ignoredFields;
        private Vector2 _scrollPos;

        public GraphSidePanel(GraphEditorWindow window)
        {
            _window = window;

            _ignoredFields = new HashSet<string>
            {
                EditorUtilityExt.SCRIPT_FIELD,
                RawGraph.NodesFieldName,
                RawGraph.RootNodeFieldName,
                RawGraph.IdGeneratorFieldName,
                RawGraph.WidthFieldName,
                RawGraph.CommonNodeFieldName,
            };
        }

        public void Draw(in Rect position)
        {
            GUILayout.BeginArea(position);
            _scrollPos.y = EditorGUILayout.BeginScrollView(_scrollPos, EditorStyles.helpBox).y;

            EditorGUILayout.LabelField(_window.SerializedGraph.GraphAsset.name, EditorStyles.boldLabel);

            EditorGUILayout.Space();

            foreach (SerializedProperty item in _window.SerializedGraph.SerializedObject.EnumerateProperties())
            {
                if (!_ignoredFields.Contains(item.name))
                    EditorGUILayout.PropertyField(item, true);
            }

            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }
    }
}
#endif
