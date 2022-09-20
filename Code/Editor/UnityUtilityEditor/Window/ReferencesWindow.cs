using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace UnityUtilityEditor.Window
{
    internal class ReferencesWindow : EditorWindow
    {
        private UnityObject _target;
        private UnityObject[] _objects;
        private Vector2 _scrollPosition;

        private void OnEnable()
        {
            minSize = new Vector2(250f, 200f);
        }

        public static void Create(string targetObjectGuid, List<string> referingObjectPaths)
        {
            ReferencesWindow window = GetWindow<ReferencesWindow>(true, "References");

            window._target = AssetDatabaseExt.LoadAssetByGuid<UnityObject>(targetObjectGuid);
            window._objects = referingObjectPaths.Select(AssetDatabase.LoadAssetAtPath<UnityObject>)
                                                 .ToArray();
        }

        private void OnGUI()
        {
            GUILayout.Space(5f);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("References for", GUILayout.MaxWidth(100f));

            GUI.enabled = false;
            EditorGUILayout.ObjectField(_target, typeof(UnityObject), false);
            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5f);

            if (_objects.IsNullOrEmpty())
            {
                EditorGUILayout.LabelField("There are no references.");
                return;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            GUI.enabled = false;
            for (int i = 0; i < _objects.Length; i++)
            {
                EditorGUILayout.ObjectField(_objects[i], typeof(UnityObject), false);
            }
            GUI.enabled = true;

            EditorGUILayout.EndScrollView();
        }
    }
}
