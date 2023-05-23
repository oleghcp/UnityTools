using UnityEditor;
using UnityEngine;
using UnityUtility.CSharp;
using UnityObject = UnityEngine.Object;

namespace Window
{
    public class FindAssetByGuidWindow : EditorWindow
    {
        private string _guid;

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("GUID:");
            _guid = EditorGUILayout.TextField(GUIContent.none, _guid);
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (_guid.IsNullOrWhiteSpace())
                return;

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            bool pressed = GUILayout.Button("Find", GUILayout.Width(200f));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (pressed)
            {
                string path = AssetDatabase.GUIDToAssetPath(_guid);

                if (path.HasUsefulData())
                    EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(path, typeof(UnityObject)));
                else
                    EditorUtility.DisplayDialog("Asset Search", "There are no assets with such GUID.", "Ok");
            }
        }
    }
}

