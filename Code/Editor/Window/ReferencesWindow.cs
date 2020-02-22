using UnityObject = UnityEngine.Object;

using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using System.Linq;

namespace UUEditor.Window
{
    internal class ReferencesWindow : EditorWindow
    {
        private UnityObject[] m_objects;

        public static void Create(List<string> referingObjectGuids)
        {
            ReferencesWindow window = GetWindow<ReferencesWindow>("References");
            window.m_objects = referingObjectGuids.Select(itm => EditorUtilityExt.LoadAssetByGuid(itm)).ToArray();
        }

        private void OnGUI()
        {
            GUI.enabled = false;

            for (int i = 0; i < m_objects.Length; i++)
            {
                m_objects[i] = EditorGUILayout.ObjectField(m_objects[i], typeof(UnityObject), false);
            }

            GUI.enabled = true;
        }
    }
}
