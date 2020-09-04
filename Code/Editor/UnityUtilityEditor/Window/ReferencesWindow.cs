using UnityObject = UnityEngine.Object;

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace UnityUtilityEditor.Window
{
    internal class ReferencesWindow : EditorWindow
    {
        private UnityObject m_target;
        private UnityObject[] m_objects;

        public static void Create(string targetObjectGuid, List<object> referingObjectGuids)
        {
            ReferencesWindow window = GetWindow<ReferencesWindow>("References");

            window.m_target = EditorUtilityExt.LoadAssetByGuid(targetObjectGuid);
            window.m_objects = referingObjectGuids.Select(itm => EditorUtilityExt.LoadAssetByGuid(itm.ToString())).ToArray();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(5f);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("References for", GUILayout.MaxWidth(100f));

            GUI.enabled = false;

            EditorGUILayout.ObjectField(m_target, typeof(UnityObject), false);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5f);

            for (int i = 0; i < m_objects.Length; i++)
            {
                EditorGUILayout.ObjectField(m_objects[i], typeof(UnityObject), false);
            }

            GUI.enabled = true;
        }
    }
}
