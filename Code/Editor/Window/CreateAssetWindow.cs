using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OlegHcp.CSharp;
using OlegHcpEditor.Engine;
using OlegHcpEditor.Utils;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace OlegHcpEditor.Window
{
    public class CreateAssetWindow : EditorWindow
    {
        private const string ANY = "Any";

        private UnityObject _targetRoot;

        private string[] _assemblyNames;
        private Dictionary<string, Type[]> _separatedTypes;
        private Dictionary<string, string[]> _separatedNames;
        private Type[] _types;
        private string[] _names;

        private int _assemblyIndex;
        private int _typeIndex;
        private string _defaultFolder;

        private void OnEnable()
        {
            maxSize = minSize = new Vector2(400f, 130f);

            _separatedTypes = new Dictionary<string, Type[]>();
            _separatedNames = new Dictionary<string, string[]>();

            foreach (Assembly assembly in AssetDatabaseExt.LoadScriptAssemblies())
            {
                Type[] types = assembly.ExportedTypes
                                       .Where(select)
                                       .ToArray();
                if (types.Length == 0)
                    continue;

                string assemblyName = assembly.GetName().Name;

                _separatedTypes[assemblyName] = types;
                _separatedNames[assemblyName] = types.Select(EditorGuiUtility.GetTypeDisplayName)
                                                     .ToArray();
            }

            _types = _separatedTypes.Values
                                    .SelectMany(item => item)
                                    .ToArray();
            _names = _separatedNames.Values
                                    .SelectMany(item => item)
                                    .ToArray();

            _assemblyNames = new string[_separatedTypes.Count + 1];
            _assemblyNames[0] = ANY;
            _separatedTypes.Keys.CopyTo(_assemblyNames, 1);

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
            if (_assemblyNames.Length == 0)
            {
                EditorGUILayout.LabelField("There is no assemblies.");
                return;
            }

            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Assembly:", GUILayout.Width(position.width * 0.5f));
            GUILayout.Label("ScriptableObject:");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            _assemblyIndex = EditorGuiLayout.DropDown(EditorPrefs.GetInt(PrefsKeys.ASSEMBLY_INDEX) % _assemblyNames.Length, _assemblyNames);
            EditorPrefs.SetInt(PrefsKeys.ASSEMBLY_INDEX, _assemblyIndex);
            string assemblyName = _assemblyNames[_assemblyIndex];
            if (_assemblyIndex == 0)
            {
                _typeIndex = EditorGuiLayout.DropDown(_typeIndex, _names);
            }
            else
            {
                _typeIndex = Math.Min(_typeIndex, _separatedTypes[assemblyName].Length - 1);
                _typeIndex = EditorGuiLayout.DropDown(_typeIndex, _separatedNames[assemblyName]);
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            _targetRoot = EditorGUILayout.ObjectField("Parent Asset", _targetRoot, typeof(UnityObject), false);

            GUILayout.Space(20f);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Create", GUILayout.Width(100f), GUILayout.Height(30f)))
                {
                    Type type = _assemblyIndex > 0 ? _separatedTypes[assemblyName][_typeIndex]
                                                   : _types[_typeIndex];
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
                        string path = EditorUtility.SaveFilePanelInProject("Save asset", type.Name, "asset", string.Empty);

                        if (path.HasUsefulData())
                            AssetDatabaseExt.CreateScriptableObjectAsset(type, path);
                    }
                }
            }
        }

        public static CreateAssetWindow Create(bool initializeParentAsset)
        {
            CreateAssetWindow window = GetWindow<CreateAssetWindow>(true, "Scriptable Objects");
            if (initializeParentAsset)
                window.SetParent(Selection.activeObject);
            return window;
        }

        private void SetParent(UnityObject parent)
        {
            _targetRoot = parent;

            if (parent.IsFolder())
                _defaultFolder = parent.GetAssetPath();
        }
    }
}
