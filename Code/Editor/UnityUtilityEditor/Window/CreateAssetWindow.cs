using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace UnityUtilityEditor.Window
{
    internal class CreateAssetWindow : EditorWindow
    {
        private UnityObject _targetRoot;

        private string[] _assemblies;
        private Dictionary<string, Type[]> _types;
        private Dictionary<string, string[]> _typeNames;

        private static int _assemblyIndex;
        private int _typeIndex;
        private string _defaultFolder;

        private void OnEnable()
        {
            maxSize = minSize = new Vector2(400f, 145f);

            _types = new Dictionary<string, Type[]>();
            _typeNames = new Dictionary<string, string[]>();

            foreach (Assembly assembly in AssetDatabaseExt.LoadScriptAssemblies())
            {
                Type[] types = assembly.ExportedTypes
                                       .Where(select)
                                       .ToArray();
                if (types.Length == 0)
                    continue;

                string assemblyName = assembly.GetName().Name;

                _types[assemblyName] = types;
                _typeNames[assemblyName] = types.Select(EditorGuiUtility.GetTypeDisplayName)
                                                .ToArray();
            }

            _assemblies = _types.Keys.ToArray();

            bool select(Type type)
            {
                return type.IsSubclassOf(typeof(ScriptableObject)) &&
                       !type.IsAbstract &&
                       !type.IsSubclassOf(typeof(StateMachineBehaviour)) &&
                       !type.IsSubclassOf(typeof(Editor)) &&
                       !type.IsSubclassOf(typeof(EditorWindow));
            }
        }

        private void OnGUI()
        {
            if (_assemblies.Length == 0)
            {
                EditorGUILayout.LabelField("There is no assemblies.");
                return;
            }

            GUILayout.Space(10f);

            _assemblyIndex = EditorGuiLayout.DropDown("Assembly:", EditorPrefs.GetInt(PrefsConstants.ASSEMBLY_INDEX_KEY) % _assemblies.Length, _assemblies);

            EditorPrefs.SetInt(PrefsConstants.ASSEMBLY_INDEX_KEY, _assemblyIndex);
            string assemblyName = _assemblies[_assemblyIndex];

            GUILayout.Space(10f);

            _typeIndex = Math.Min(_typeIndex, _types[assemblyName].Length - 1);
            _typeIndex = EditorGuiLayout.DropDown("ScriptableObject:", _typeIndex, _typeNames[assemblyName]);

            EditorGUILayout.Space();

            _targetRoot = EditorGUILayout.ObjectField("Parrent Asset", _targetRoot, typeof(UnityObject), false);

            GUILayout.Space(20f);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Create", GUILayout.Width(100f), GUILayout.Height(30f)))
                {
                    Type type = _types[assemblyName][_typeIndex];

                    if (_targetRoot != null)
                    {
                        if (_defaultFolder.HasUsefulData())
                        {
                            string path = $"{_defaultFolder}/{type.Name}{AssetDatabaseExt.ASSET_EXTENSION}";

                            if (AssetDatabase.LoadAssetAtPath(path, typeof(UnityObject)) != null)
                                path = AssetDatabase.GenerateUniqueAssetPath(path);

                            AssetDatabaseExt.CreateScriptableObjectAsset(type, path);
                        }
                        else
                        {
                            AssetDatabaseExt.CreateScriptableObjectAsset(type, _targetRoot);
                        }
                    }
                    else
                    {
                        string dataPath = Application.dataPath;
                        string path = EditorUtility.SaveFilePanel("Save asset", dataPath, type.Name, "asset");

                        if (path.HasUsefulData())
                        {
                            path = AssetDatabaseExt.ASSET_FOLDER + path.Substring(dataPath.Length + 1);
                            AssetDatabaseExt.CreateScriptableObjectAsset(type, path);
                        }
                    }
                }
            }
        }

        public void SetParent(UnityObject parent)
        {
            _targetRoot = parent;

            if (parent.IsFolder())
                _defaultFolder = parent.GetAssetPath();
        }
    }
}
