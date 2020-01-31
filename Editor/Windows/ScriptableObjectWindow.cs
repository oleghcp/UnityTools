using UnityObject = UnityEngine.Object;

using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;

namespace UUEditor.Windows
{
    internal class ScriptableObjectWindow : EditorWindow
    {
        private static bool m_keepOpened;
        private UnityObject m_targetRoot;

        private string[] m_types;
        private int m_index;

        private void Awake()
        {
            minSize = new Vector2(300f, 125f);
            maxSize = new Vector2(300f, 125f);

            bool select(Type type)
            {
                bool fine = !type.IsAbstract && type.IsSubclassOf(typeof(ScriptableObject));
                fine &= !type.IsSubclassOf(typeof(Editor));
                fine &= !type.IsSubclassOf(typeof(EditorWindow));

                return fine;
            }

            Assembly[] assemblies = EditorScriptUtility.GetAssemblies();
            Type[] types = EditorScriptUtility.GetTypes(assemblies, select);
            m_types = types.Select(itm => itm.Name).ToArray();
        }

        private void OnGUI()
        {
            if (m_types.Length == 0)
            {
                EditorGUILayout.LabelField("There is no any ScriptableObject inheritor.");
            }
            else
            {
                GUILayout.Space(10f);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("ScriptableObject:", EditorStyles.boldLabel, GUILayout.MinWidth(10f));
                m_index = EditorGUILayout.Popup(m_index, m_types);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                m_targetRoot = EditorGUILayout.ObjectField("Root Asset", m_targetRoot, typeof(UnityObject), false);

                EditorGUILayout.Space();

                if (EditorScriptUtility.DrawCenterButton("Create", 50f, 30f))
                {
                    if (m_targetRoot == null)
                        EditorScriptUtility.CreateAsset(m_types[m_index]);
                    else
                        EditorScriptUtility.CreateAsset(m_types[m_index], m_targetRoot);

                    if (!m_keepOpened)
                        Close();
                }

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10f);
                m_keepOpened = EditorGUILayout.Toggle(m_keepOpened, GUILayout.MaxWidth(20f));
                EditorGUILayout.LabelField("Keep opened");
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
