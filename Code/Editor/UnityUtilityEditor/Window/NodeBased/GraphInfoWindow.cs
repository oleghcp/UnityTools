using UnityEditor;
using UnityEngine;
using UnityUtility.NodeBased;

#if UNITY_2019_3_OR_NEWER
namespace UnityUtilityEditor.Window.NodeBased
{
    internal class GraphInfoWindow : EditorWindow
    {
        private GraphEditorWindow _mainWindow;
        private SerializedObject _serializedObject;

        private Vector2 _scrollPos;
        private bool _destroying;

        public static void Open(Graph graphAsset, GraphEditorWindow mainWindow)
        {
            GraphInfoWindow window = GetWindow<GraphInfoWindow>(true, "Graph Info");

            window.minSize = new Vector2(300f, 300f);
            window.SetUp(graphAsset, mainWindow);
        }

        private void SetUp(Graph graphAsset, GraphEditorWindow mainWindow)
        {
            _mainWindow = mainWindow;
            _serializedObject = new SerializedObject(graphAsset);
        }

        private void OnDestroy()
        {
            _destroying = true;

            if (_mainWindow != null)
                _mainWindow.Focus();
        }

        private void OnLostFocus()
        {
            if (_destroying)
                return;

            Close();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(2f);
            _scrollPos.y = EditorGUILayout.BeginScrollView(_scrollPos, EditorStyles.helpBox).y;
            GUIExt.DrawObjectFields(_serializedObject, IsServiceField);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space(2f);

            if (GUI.changed)
                _serializedObject.ApplyModifiedProperties();
        }

        private bool IsServiceField(SerializedProperty property)
        {
            return property.propertyPath == EditorUtilityExt.SCRIPT_FIELD;
        }
    }
}
#endif
