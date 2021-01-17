using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace UnityUtilityEditor.Window
{
    internal class ReferencesWindow : EditorWindow
    {
        private UnityObject m_target;
        private UnityObject[] m_objects;
        private Vector2 _scrollPosition;

        public static void Create(string targetObjectGuid, List<object> referingObjectGuids)
        {
            ReferencesWindow window = GetWindow<ReferencesWindow>(true, "References");

            window.m_target = EditorUtilityExt.LoadAssetByGuid(targetObjectGuid);
            window.m_objects = referingObjectGuids.Select(itm => EditorUtilityExt.LoadAssetByGuid(itm.ToString())).ToArray();
            window.minSize = new Vector2(250f, 200f);
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(5f);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("References for", GUILayout.MaxWidth(100f));

            GUI.enabled = false;
            EditorGUILayout.ObjectField(m_target, typeof(UnityObject), false);
            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5f);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            GUI.enabled = false;
            for (int i = 0; i < m_objects.Length; i++)
            {
                EditorGUILayout.ObjectField(m_objects[i], typeof(UnityObject), false);
            }
            GUI.enabled = true;

            EditorGUILayout.EndScrollView();
        }
    }
}
