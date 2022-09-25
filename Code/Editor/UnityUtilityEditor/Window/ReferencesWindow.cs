using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace UnityUtilityEditor.Window
{
    internal class ReferencesWindow : EditorWindow
    {
        private readonly GUILayoutOption[] _buttonOptions = new[]
        {
            GUILayout.Width(EditorGUIUtility.singleLineHeight),
            GUILayout.Height(EditorGUIUtility.singleLineHeight),
        };

        private UnityObject _target;
        private (UnityObject asset, string path)[] _objects;
        private Vector2 _scrollPosition;

        private void OnEnable()
        {
            minSize = new Vector2(250f, 200f);
        }

        public static void Create(string targetObjectGuid, IEnumerable<string> referingObjectPaths)
        {
            ReferencesWindow window = GetWindow<ReferencesWindow>(true, "References");

            window._target = AssetDatabaseExt.LoadAssetByGuid<UnityObject>(targetObjectGuid);
            window._objects = referingObjectPaths.Select(createTuple).ToArray();

            (UnityObject, string) createTuple(string path)
            {
                return (AssetDatabase.LoadAssetAtPath<UnityObject>(path), EditorGuiUtility.NicifyPathLabel(path));
            }
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

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, EditorStyles.helpBox);

            for (int i = 0; i < _objects.Length; i++)
            {
                var (asset, path) = _objects[i];

                EditorGUILayout.BeginHorizontal();
                bool clicked = GUILayout.Button(asset.GetAssetIcon(), EditorStyles.label, _buttonOptions);
                clicked |= GUILayout.Button(path, EditorStyles.label);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                if (clicked)
                    EditorGUIUtility.PingObject(asset);
            }

            EditorGUILayout.EndScrollView();
        }
    }
}
