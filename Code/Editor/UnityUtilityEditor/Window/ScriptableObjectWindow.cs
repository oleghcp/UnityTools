using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace UnityUtilityEditor.Window
{
    internal class ScriptableObjectWindow : EditorWindow
    {
        private static bool m_keepOpened;
        private UnityObject m_targetRoot;

        private string[] m_assemblies;
        private Dictionary<string, string[]> m_types;

        private int m_assemblyIndex;
        private int m_typeIndex;

        private void Awake()
        {
            minSize = new Vector2(300f, 160f);
            maxSize = new Vector2(300f, 160f);

            bool select(Type type)
            {
                return type.IsSubclassOf(typeof(ScriptableObject)) &&
                       !type.IsAbstract &&
                       !type.IsSubclassOf(typeof(Editor)) &&
                       !type.IsSubclassOf(typeof(EditorWindow));
            }

            m_types = new Dictionary<string, string[]>();

            foreach (var assembly in EditorUtilityExt.GetAssemblies())
            {
                IEnumerable<Type> types = assembly.GetTypes()
                                                  .Where(select);

                if (types.HasAnyData())
                {
                    m_types[assembly.GetName().Name] = types.Select(itm => itm.Name)
                                                            .ToArray();
                }
            }

            m_assemblies = m_types.Keys.ToArray();
        }

        private void OnGUI()
        {
            if (m_assemblies.Length == 0)
            {
                EditorGUILayout.LabelField("There is no assemblies.");
                return;
            }

            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Assembly:", EditorStyles.boldLabel, GUILayout.MinWidth(10f));
            m_assemblyIndex = EditorGUILayout.Popup(m_assemblyIndex, m_assemblies);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ScriptableObject:", EditorStyles.boldLabel, GUILayout.MinWidth(10f));
            m_typeIndex = Math.Min(m_typeIndex, m_types[m_assemblies[m_assemblyIndex]].Length - 1);
            m_typeIndex = EditorGUILayout.Popup(m_typeIndex, m_types[m_assemblies[m_assemblyIndex]]);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            m_targetRoot = EditorGUILayout.ObjectField("Root Asset", m_targetRoot, typeof(UnityObject), false);

            EditorGUILayout.Space();

            if (EditorScriptUtility.DrawCenterButton("Create", 50f, 30f))
            {
                string type = m_types[m_assemblies[m_assemblyIndex]][m_typeIndex];

                if (m_targetRoot == null)
                    EditorUtilityExt.CreateScriptableObjectAsset(type);
                else
                    EditorUtilityExt.CreateScriptableObjectAsset(type, m_targetRoot);

                if (!m_keepOpened)
                    Close();
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10f);
            m_keepOpened = EditorGUILayout.Toggle(m_keepOpened, GUILayout.MaxWidth(EditorGUIUtilityExt.SmallButtonWidth));
            EditorGUILayout.LabelField("Keep opened");
            EditorGUILayout.EndHorizontal();
        }
    }
}
