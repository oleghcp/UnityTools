using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OlegHcp;
using OlegHcp.CSharp;
using OlegHcp.Tools;
using OlegHcpEditor.CodeGenerating;
using OlegHcpEditor.Configs;
using OlegHcpEditor.Engine;
using OlegHcpEditor.Gui;
using OlegHcpEditor.Utils;
using UnityEditor;
using UnityEngine;
using static OlegHcpEditor.Configs.LayerSetConfig;

namespace OlegHcpEditor.Window
{
    public class LayerSetWindow : EditorWindow
    {
        private const string _tagFieldName = "tags";
        private static string _settingsPath = $"{AssetDatabaseExt.PROJECT_SETTINGS_FOLDER}{nameof(LayerSetConfig)}.json";

        private LayerSetConfigWrapper _configWrapper;
        private LayerSetConfig _altConfigVersion = new LayerSetConfig();

        private ListDrawer<MaskField> _listDrawer;
        private SerializedObject _tagManager;
        private SerializedProperty _tags;

        private Vector2 _scrollPos;

        private void OnEnable()
        {
            _configWrapper = GetConfig();
            _tagManager = new SerializedObject(Managers.GetTagManager());
            _tags = _tagManager.FindProperty(_tagFieldName);

            string labelName = ObjectNames.NicifyVariableName(nameof(LayerSetConfig.LayerMasks));
            _listDrawer = new ListDrawer<MaskField>(labelName, new LayerMaskFieldDrawer());
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
            if (_altConfigVersion.TagFields)
                DrawCollection(_tags.EnumerateArrayElements(), item => GUILayout.Label(CreateItemString(item.stringValue)));

            _altConfigVersion.SortingLayerFields = EditorGUILayout.Toggle(ObjectNames.NicifyVariableName(nameof(LayerSetConfig.SortingLayerFields)), config.SortingLayerFields);
            if (_altConfigVersion.SortingLayerFields)
                DrawCollection(SortingLayer.layers, item => GUILayout.Label(CreateItemString(item.name)));

            _altConfigVersion.LayerFields = EditorGUILayout.Toggle(ObjectNames.NicifyVariableName(nameof(LayerSetConfig.LayerFields)), config.LayerFields);
            if (_altConfigVersion.LayerFields)
                DrawCollection(EnumerateLayerNames(), item => GUILayout.Label(CreateItemString(item)));

            if (_altConfigVersion.LayerFields)
            {
                _altConfigVersion.MaskFieldType = (LayerMaskFieldType)EditorGUILayout.EnumPopup(ObjectNames.NicifyVariableName(nameof(LayerSetConfig.MaskFieldType)), config.MaskFieldType);
                LayerMaskFieldDrawer drawer = _listDrawer.ElementDrawer as LayerMaskFieldDrawer;
                drawer.Names = EnumerateLayerNames().ToArray();
                _altConfigVersion.LayerMasks = _listDrawer.Draw(config.LayerMasks);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndScrollView();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            bool generateClass = GUILayout.Button("Generate Class", GUILayout.Width(120f), GUILayout.Height(25f));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (GUI.changed)
            {
                Undo.RecordObject(_configWrapper, nameof(LayerSetWindow));
                Helper.Swap(ref _configWrapper.Config, ref _altConfigVersion);
                SaveAsset();
            }

            if (generateClass)
                GenerateClass(_configWrapper, _tags);
        }

        public static void Create()
        {
            LayerSetWindow window = GetWindow<LayerSetWindow>("Layer Set", true);
            window.minSize = new Vector2(350f, 500f);
        }

        public static void GenerateClass()
        {
            LayerSetConfigWrapper configWrapper = GetConfig();
            SerializedObject tagManager = new SerializedObject(Managers.GetTagManager());
            GenerateClass(configWrapper, tagManager.FindProperty(_tagFieldName));
        }

        private static IEnumerable<string> EnumerateLayerNames()
        {
            for (int i = 0; i < BitMask.SIZE; i++)
            {
                string name = LayerMask.LayerToName(i);

                if (name.HasUsefulData())
                    yield return name;
            }
        }

        private static LayerSetConfigWrapper GetConfig()
        {
            LayerSetConfigWrapper wrapper = CreateInstance<LayerSetConfigWrapper>();

            if (File.Exists(_settingsPath))
            {
                string json = File.ReadAllText(_settingsPath);
                wrapper.Config = JsonUtility.FromJson<LayerSetConfig>(json);
            }
            else
            {
                wrapper.Config = new LayerSetConfig();
            }

            return wrapper;
        }

        private static void GenerateClass(LayerSetConfigWrapper configWrapper, SerializedProperty tagArrayProperty)
        {
            LayerSetConfig config = configWrapper.Config;
            IEnumerable<string> tags = tagArrayProperty.EnumerateArrayElements()
                                                       .Select(item => item.stringValue);
            string classText = LayerSetClassGenerator.Generate(config, tags);
            GeneratingTools.CreateCsFile(classText, config.RootFolder, config.ClassName, config.Namespace);
        }

        private void SaveAsset()
        {
            string json = JsonUtility.ToJson(_configWrapper.Config, true);
            File.WriteAllText(_settingsPath, json);
        }

        private void DrawCollection<T>(IEnumerable<T> collection, Action<T> drawer)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            foreach (T item in collection)
            {
                drawer(item);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        private string CreateItemString(string item)
        {
            return $"- {item}";
        }

        #region Entities
        private class LayerMaskFieldDrawer : IListElementDrawer<MaskField>
        {
            public string[] Names;

            MaskField IListElementDrawer<MaskField>.OnDrawElement(Rect position, int index, MaskField element, bool isActive, bool isFocused)
            {
                Rect halfPos = position;
                halfPos.width = halfPos.width * 0.5f - EditorGuiUtility.StandardHorizontalSpacing;
                element.Name = EditorGUI.TextField(halfPos, element.Name);

                halfPos = position;
                halfPos.xMin += position.width * 0.5f;

                element.Mask = EditorGui.MaskDropDown(halfPos, element.Mask, Names);
                return element;
            }

            float IListElementDrawer<MaskField>.OnElementHeight(int index)
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }

        private class LayerSetConfigWrapper : ScriptableObject
        {
            public LayerSetConfig Config;
        }
        #endregion
    }
}
