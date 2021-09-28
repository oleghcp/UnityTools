using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityUtilityEditor.CodeGenerating;
using UnityUtilityEditor.Configs;
using UnityUtilityEditor.Gui;
using UnityObject = UnityEngine.Object;

#if UNITY_2018_3_OR_NEWER
namespace UnityUtilityEditor.SettingsProviders
{
    internal class LayerSetSettingsProvider : SettingsProvider
    {
        private readonly string _settingsPath;

        private LayerSetConfig _config;
        private ListDrawer<LayerSetConfig.LayerMaskField> _listDrawer;
        private SerializedObject _tagManager;
        private SerializedProperty _tags;
        private SerializedProperty _layers;

        private Vector2 _scrollPos;

        public LayerSetSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
            _settingsPath = $"{AssetDatabaseExt.PROJECT_SETTINGS_FOLDER}{nameof(LayerSetConfig)}.json";

            if (File.Exists(_settingsPath))
            {
                string json = File.ReadAllText(_settingsPath);
                _config = JsonUtility.FromJson<LayerSetConfig>(json);
            }
            else
            {
                _config = new LayerSetConfig();
            }

            string assetPath = $"{AssetDatabaseExt.PROJECT_SETTINGS_FOLDER}TagManager{AssetDatabaseExt.ASSET_EXTENSION}";
            UnityObject tagManager = AssetDatabase.LoadAssetAtPath<UnityObject>(assetPath);
            _tagManager = new SerializedObject(tagManager);
            _tags = _tagManager.FindProperty("tags");
            _layers = _tagManager.FindProperty("layers");

            _listDrawer = new ListDrawer<LayerSetConfig.LayerMaskField>(_config.LayerMasks,
                                                                  ObjectNames.NicifyVariableName(nameof(_config.LayerMasks)),
                                                                  new LayerMaskFieldDrawer());
        }

        [SettingsProvider]
        private static SettingsProvider CreateMyCustomSettingsProvider()
        {
            return new LayerSetSettingsProvider($"{nameof(UnityUtility)}/Layer Set",
                                                SettingsScope.Project,
                                                new[] { "Layer", "Set", "Generate", "Class" });
        }

        public override void OnGUI(string searchContext)
        {
            _config.GenerateStaticClass = EditorGUILayout.Toggle(ObjectNames.NicifyVariableName(nameof(_config.GenerateStaticClass)), _config.GenerateStaticClass);

            if (_config.GenerateStaticClass)
            {
                _tagManager.Update();

                EditorGUILayout.Space();

                _scrollPos.y = GUILayout.BeginScrollView(_scrollPos, EditorStyles.helpBox).y;

                _config.RootFolder = EditorGUILayout.TextField(ObjectNames.NicifyVariableName(nameof(_config.RootFolder)), _config.RootFolder);
                _config.NameSpace = EditorGUILayout.TextField(ObjectNames.NicifyVariableName(nameof(_config.NameSpace)), _config.NameSpace);

                EditorGUILayout.Space();

                _config.TagFields = EditorGUILayout.Toggle(ObjectNames.NicifyVariableName(nameof(_config.TagFields)), _config.TagFields);
                DrawCollection(_config.TagFields, _tags.EnumerateArrayElements(), item => EditorGUILayout.LabelField(CreateItemString(item.stringValue)));

                _config.SortingLayerFields = EditorGUILayout.Toggle(ObjectNames.NicifyVariableName(nameof(_config.SortingLayerFields)), _config.SortingLayerFields);
                DrawCollection(_config.SortingLayerFields, SortingLayer.layers, item => EditorGUILayout.LabelField(CreateItemString(item.name)));

                _config.LayersFields = EditorGUILayout.Toggle(ObjectNames.NicifyVariableName(nameof(_config.LayersFields)), _config.LayersFields);
                DrawCollection(_config.LayersFields, _layers.EnumerateArrayElements(), drawLayer);
                void drawLayer(SerializedProperty item)
                {
                    if (item.stringValue.HasUsefulData())
                        EditorGUILayout.LabelField(CreateItemString(item.stringValue));
                }

                if (_config.LayersFields)
                {
                    var drawer = _listDrawer.ElementDrawer as LayerMaskFieldDrawer;
                    drawer.Names = _layers.EnumerateArrayElements()
                                          .Select(item => item.stringValue)
                                          .Where(item => item.HasUsefulData())
                                          .ToArray();
                    _listDrawer.Draw();
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndScrollView();
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                _config.AutoGenerate = EditorGUILayout.Toggle(_config.AutoGenerate, GUILayout.Width(13f));
                GUILayout.Label(ObjectNames.NicifyVariableName(nameof(_config.AutoGenerate)));
                GUI.enabled = !_config.AutoGenerate;
                if (GUILayout.Button("Generate", GUILayout.Width(100f)))
                    GenerateClass();
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }

            if (GUI.changed)
            {
                if (!_config.GenerateStaticClass)
                    _config = new LayerSetConfig();

                SaveAsset();
            }
        }

        private void SaveAsset()
        {
            string json = JsonUtility.ToJson(_config, true);
            File.WriteAllText(_settingsPath, json);
        }

        private void DrawCollection<T>(bool draw, IEnumerable<T> collection, Action<T> drawer)
        {
            if (!draw)
                return;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            foreach (var item in collection)
            {
                drawer(item);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string CreateItemString<T>(T item)
        {
            return $"- {item}";
        }

        private void GenerateClass()
        {
            string className = "LayerSet";
            string classText = LayerSetClassGenerator.Generate(className, _config, _tagManager);
            GeneratingTools.CreateCsFile(classText, _config.RootFolder, className, _config.NameSpace);
        }

        private class LayerMaskFieldDrawer : IListElementDrawer<LayerSetConfig.LayerMaskField>
        {
            public string[] Names;

            void IListElementDrawer<LayerSetConfig.LayerMaskField>.OnDrawElement(Rect position, ref LayerSetConfig.LayerMaskField element, bool isActive, bool isFocused)
            {
                Rect halfPos = position;
                halfPos.width = halfPos.width * 0.5f - EditorGuiUtility.StandardHorizontalSpacing;
                element.Name = EditorGUI.TextField(halfPos, element.Name);

                halfPos = position;
                halfPos.width *= 0.5f;
                halfPos.x += position.width * 0.5f;

                element.Mask = EditorGui.MaskDropDown(halfPos, element.Mask, Names);
            }

            float IListElementDrawer<LayerSetConfig.LayerMaskField>.OnElementHeight(int index)
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }
    }
}
#endif
