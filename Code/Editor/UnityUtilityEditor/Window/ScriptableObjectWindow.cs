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
        private static bool _keepOpened;
        private UnityObject _targetRoot;

        private string[] _assemblies;
        private Dictionary<string, string[]> m_types;

        private static int _assemblyIndex;
        private int _typeIndex;

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

            _assemblies = m_types.Keys.ToArray();
        }

        private void OnGUI()
        {
            if (_assemblies.Length == 0)
            {
                EditorGUILayout.LabelField("There is no assemblies.");
                return;
            }

            GUILayout.Space(10f);

            _assemblyIndex = EditorGUILayout.Popup("Assembly:", _assemblyIndex % _assemblies.Length, _assemblies);
            string assemblyName = _assemblies[_assemblyIndex];

            GUILayout.Space(10f);

            _typeIndex = Math.Min(_typeIndex, m_types[assemblyName].Length - 1);
            _typeIndex = EditorGUILayout.Popup("ScriptableObject:", _typeIndex, m_types[assemblyName]);

            EditorGUILayout.Space();

            _targetRoot = EditorGUILayout.ObjectField("Parrent Asset", _targetRoot, typeof(UnityObject), false);

            EditorGUILayout.Space(20f);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(10f);

                _keepOpened = EditorGUILayout.Toggle(_keepOpened, GUILayout.MaxWidth(EditorGUIUtilityExt.SmallButtonWidth));
                EditorGUILayout.LabelField("Keep opened");

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Create", GUILayout.Width(100f), GUILayout.Height(30f)))
                {
                    string typeName = m_types[assemblyName][_typeIndex];
                    Type type = Type.GetType($"{typeName}, {assemblyName}");

                    string path = EditorUtility.SaveFilePanel("Save asset", Application.dataPath, type.Name + EditorUtilityExt.ASSET_EXTENSION, "asset");

                    if (path.HasUsefulData())
                    {
                        path = EditorUtilityExt.ASSET_FOLDER + path.Substring(Application.dataPath.Length);

                        if (_targetRoot == null)
                            EditorUtilityExt.CreateScriptableObjectAsset(type, path);
                        else
                            EditorUtilityExt.CreateScriptableObjectAsset(type, _targetRoot, path);

                        if (!_keepOpened)
                            Close();
                    }
                }
            }
        }
    }
}
