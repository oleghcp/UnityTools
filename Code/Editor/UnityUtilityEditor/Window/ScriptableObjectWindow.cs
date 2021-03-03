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
            maxSize = minSize = new Vector2(400f, 145f);

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
                    m_types[assembly.GetName().Name] = types.Select(itm => itm.FullName)
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

            m_assemblyIndex = EditorGUILayout.Popup("Assembly:", m_assemblyIndex, m_assemblies);

            GUILayout.Space(10f);

            m_typeIndex = Math.Min(m_typeIndex, m_types[m_assemblies[m_assemblyIndex]].Length - 1);
            m_typeIndex = EditorGUILayout.Popup("ScriptableObject:", m_typeIndex, m_types[m_assemblies[m_assemblyIndex]]);

            EditorGUILayout.Space();

            m_targetRoot = EditorGUILayout.ObjectField("Parrent Asset", m_targetRoot, typeof(UnityObject), false);

            EditorGUILayout.Space(20f);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(10f);

                m_keepOpened = EditorGUILayout.Toggle(m_keepOpened, GUILayout.MaxWidth(EditorGUIUtilityExt.SmallButtonWidth));
                EditorGUILayout.LabelField("Keep opened");

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Create", GUILayout.Width(100f), GUILayout.Height(30f)))
                {
                    string assemblyName = m_assemblies[m_assemblyIndex];
                    string typeName = m_types[assemblyName][m_typeIndex];
                    Type type = Type.GetType($"{typeName}, {assemblyName}");

                    string path = EditorUtility.SaveFilePanel("Save asset", Application.dataPath, type.Name + EditorUtilityExt.ASSET_EXTENSION, "asset");
                    path = EditorUtilityExt.ASSET_FOLDER + path.Substring(Application.dataPath.Length);

                    if (path.HasUsefulData())
                    {
                        if (m_targetRoot == null)
                            EditorUtilityExt.CreateScriptableObjectAsset(type, path);
                        else
                            EditorUtilityExt.CreateScriptableObjectAsset(type, m_targetRoot, path);

                        if (!m_keepOpened)
                            Close();
                    }
                }
            }
        }
    }
}
