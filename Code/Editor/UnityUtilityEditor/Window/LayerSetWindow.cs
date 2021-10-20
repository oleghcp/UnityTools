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
using UnityUtilityTools;

namespace UnityUtilityEditor.Window
{
    internal class LayerSetWindow : EditorWindow
    {
        private string _settingsPath;

        private LayerSetConfigWrapper _configWrapper;
        private LayerSetConfig _altConfigVersion = new LayerSetConfig();

        private ListDrawer<LayerSetConfig.MaskField> _listDrawer;
        private SerializedObject _tagManager;
        private SerializedProperty _tags;
        private SerializedProperty _layers;

        private Vector2 _scrollPos;

        public static void CreateWindow()
        {
            LayerSetWindow window = GetWindow<LayerSetWindow>(true, "Layer Set");
            window.minSize = new Vector2(350f, 500f);
        }

        private void OnEnable()
        {
            _settingsPath = $"{AssetDatabaseExt.PROJECT_SETTINGS_FOLDER}{nameof(LayerSetConfig)}.json";

            _configWrapper = CreateInstance<LayerSetConfigWrapper>();

            if (File.Exists(_settingsPath))
            {
                string json = File.ReadAllText(_settingsPath);
                _configWrapper.Config = JsonUtility.FromJson<LayerSetConfig>(json);
            }
            else
            {
                _configWrapper.Config = new LayerSetConfig();
            }

            _tagManager = new SerializedObject(Managers.GetTagManager());
            _tags = _tagManager.FindProperty("tags");
            _layers = _tagManager.FindProperty("layers");

            _listDrawer = new ListDrawer<LayerSetConfig.MaskField>(ObjectNames.NicifyVariableName(nameof(LayerSetConfig.LayerMasks)),
                                                                   new LayerMaskFieldDrawer());
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();

            if (GUILayout.Button("Open Tag Manager", GUILayout.Height(25f)))
                Selection.activeObject = _tagManager.targetObject;

            _tagManager.Update();

            LayerSetConfig config = _configWrapper.Config;

            EditorGUILayout.Space();

            _scrollPos.y = GUILayout.BeginScrollView(_scrollPos).y;

            _altConfigVersion.ClassName = EditorGUILayout.TextField(ObjectNames.NicifyVariableName(nameof(LayerSetConfig.ClassName)), config.ClassName);
            _altConfigVersion.Namespace = EditorGUILayout.TextField(ObjectNames.NicifyVariableName(nameof(LayerSetConfig.Namespace)), config.Namespace);
            _altConfigVersion.RootFolder = EditorGUILayout.TextField(ObjectNames.NicifyVariableName(nameof(LayerSetConfig.RootFolder)), config.RootFolder);

            EditorGUILayout.Space();

            _altConfigVersion.TagFields = EditorGUILayout.Toggle(ObjectNames.NicifyVariableName(nameof(LayerSetConfig.TagFields)), config.TagFields);
            DrawCollection(_altConfigVersion.TagFields, _tags.EnumerateArrayElements(), item => EditorGUILayout.LabelField(CreateItemString(item.stringValue)));

            _altConfigVersion.SortingLayerFields = EditorGUILayout.Toggle(ObjectNames.NicifyVariableName(nameof(LayerSetConfig.SortingLayerFields)), config.SortingLayerFields);
            DrawCollection(_altConfigVersion.SortingLayerFields, SortingLayer.layers, item => EditorGUILayout.LabelField(CreateItemString(item.name)));

            _altConfigVersion.LayerFields = EditorGUILayout.Toggle(ObjectNames.NicifyVariableName(nameof(LayerSetConfig.LayerFields)), config.LayerFields);
            DrawCollection(_altConfigVersion.LayerFields, _layers.EnumerateArrayElements(), drawLayer);
            void drawLayer(SerializedProperty item)
            {
                if (item.stringValue.HasUsefulData())
                    EditorGUILayout.LabelField(CreateItemString(item.stringValue));
            }

            if (_altConfigVersion.LayerFields)
            {
                _altConfigVersion.MaskFieldType = (LayerSetConfig.LayerMaskFieldType)EditorGUILayout.EnumPopup(ObjectNames.NicifyVariableName(nameof(LayerSetConfig.MaskFieldType)), config.MaskFieldType);
                var drawer = _listDrawer.ElementDrawer as LayerMaskFieldDrawer;
                drawer.Names = _layers.EnumerateArrayElements()
                                      .Select(item => item.stringValue)
                                      .ToArray();

                _altConfigVersion.LayerMasks = _listDrawer.Draw(config.LayerMasks);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndScrollView();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Generate Class", GUILayout.Width(120f), GUILayout.Height(25f)))
                GenerateClass();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (GUI.changed)
            {
                Undo.RecordObject(_configWrapper, nameof(LayerSetWindow));
                Helper.Swap(ref _configWrapper.Config, ref _altConfigVersion);
                SaveAsset();
            }
        }

        public void GenerateClass()
        {
            LayerSetConfig config = _configWrapper.Config;
            string classText = LayerSetClassGenerator.Generate(config, _tagManager);
            GeneratingTools.CreateCsFile(classText, config.RootFolder, config.ClassName, config.Namespace);
        }

        private void SaveAsset()
        {
            string json = JsonUtility.ToJson(_configWrapper.Config, true);
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

        private class LayerMaskFieldDrawer : IListElementDrawer<LayerSetConfig.MaskField>
        {
            public string[] Names;

            LayerSetConfig.MaskField IListElementDrawer<LayerSetConfig.MaskField>.OnDrawElement(Rect position, LayerSetConfig.MaskField element, bool isActive, bool isFocused)
            {
                Rect halfPos = position;
                halfPos.width = halfPos.width * 0.5f - EditorGuiUtility.StandardHorizontalSpacing;
                element.Name = EditorGUI.TextField(halfPos, element.Name);

                halfPos = position;
                halfPos.width *= 0.5f;
                halfPos.x += position.width * 0.5f;

                element.Mask = EditorGui.MaskDropDown(halfPos, element.Mask, Names);
                return element;
            }

            float IListElementDrawer<LayerSetConfig.MaskField>.OnElementHeight(int index)
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }

        private class LayerSetConfigWrapper : ScriptableObject
        {
            public LayerSetConfig Config;
        }
    }
}
