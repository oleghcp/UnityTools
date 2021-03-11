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
        private const string ASSEMBLY_INDEX_KEY = "uu_csowai";

        private static bool _keepOpened;
        private UnityObject _targetRoot;

        private string[] _assemblies;
        private Dictionary<string, Type[]> _types;

        private static int _assemblyIndex;
        private int _typeIndex;
        private Rect _dropdownButtonRect;

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

            _types = new Dictionary<string, Type[]>();

            foreach (var assembly in EditorUtilityExt.GetAssemblies())
            {
                IEnumerable<Type> types = assembly.GetTypes()
                                                  .Where(select);

                if (types.HasAnyData())
                {
                    _types[assembly.GetName().Name] = types.ToArray();
                }
            }

            _assemblies = _types.Keys.ToArray();
        }

        private void OnGUI()
        {
            if (_assemblies.Length == 0)
            {
                EditorGUILayout.LabelField("There is no assemblies.");
                return;
            }

            GUILayout.Space(10f);

            _assemblyIndex = EditorGUILayout.Popup("Assembly:", EditorPrefs.GetInt(ASSEMBLY_INDEX_KEY) % _assemblies.Length, _assemblies);
            EditorPrefs.SetInt(ASSEMBLY_INDEX_KEY, _assemblyIndex);
            string assemblyName = _assemblies[_assemblyIndex];

            GUILayout.Space(10f);

            _typeIndex = Math.Min(_typeIndex, _types[assemblyName].Length - 1);

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("ScriptableObject:");

                bool btn = EditorGUILayout.DropdownButton(new GUIContent(GetDisplayName(_types[assemblyName][_typeIndex])), FocusType.Passive);

                if (Event.current.type == EventType.Repaint)
                    _dropdownButtonRect = GUILayoutUtility.GetLastRect();

                if (btn)
                    ShowMenu(assemblyName);
            }

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
                    Type type = _types[assemblyName][_typeIndex];

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

        private static string GetDisplayName(Type type)
        {
            return $"{type.Name} ({type.Namespace})";
        }

        private void ShowMenu(string assemblyName)
        {
            DropDownList list = DropDownList.Create();
            Type[] typeNames = _types[assemblyName];

            for (int i = 0; i < typeNames.Length; i++)
            {
                int index = i;
                list.AddItem(GetDisplayName(typeNames[i]), _typeIndex == i, () => _typeIndex = index);
            }


            list.ShowMenu(_dropdownButtonRect);
        }
    }
}
