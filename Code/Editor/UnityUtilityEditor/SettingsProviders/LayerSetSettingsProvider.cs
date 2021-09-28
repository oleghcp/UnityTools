using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityUtilityEditor.Configs;
using UnityUtilityEditor.Gui;
using UnityObject = UnityEngine.Object;

#if UNITY_2018_3_OR_NEWER
namespace UnityUtilityEditor.SettingsProviders
{
    internal class LayerSetSettingsProvider : SettingsProvider
    {
        private readonly string _settingsPath;

        private LayerSet _layerSet;
        private ListDrawer<LayerSet.LayerMaskField> _listDrawer;
        private SerializedObject _tagManager;
        private SerializedProperty _tags;
        private SerializedProperty _sortingLayers;
        private SerializedProperty _layers;

        private Vector2 _scrollPos;

        public LayerSetSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
            _settingsPath = $"{AssetDatabaseExt.PROJECT_SETTINGS_FOLDER}{nameof(LayerSet)}.json";

            if (File.Exists(_settingsPath))
            {
                string json = File.ReadAllText(_settingsPath);
                _layerSet = JsonUtility.FromJson<LayerSet>(json);
            }
            else
            {
                _layerSet = new LayerSet();
            }

            string assetPath = $"{AssetDatabaseExt.PROJECT_SETTINGS_FOLDER}TagManager{AssetDatabaseExt.ASSET_EXTENSION}";
            UnityObject tagManager = AssetDatabase.LoadAssetAtPath<UnityObject>(assetPath);
            _tagManager = new SerializedObject(tagManager);
            _tags = _tagManager.FindProperty("tags");
            _sortingLayers = _tagManager.FindProperty("m_SortingLayers");
            _layers = _tagManager.FindProperty("layers");

            _listDrawer = new ListDrawer<LayerSet.LayerMaskField>(_layerSet.LayerMasks,
                                                                  ObjectNames.NicifyVariableName(nameof(_layerSet.LayerMasks)),
                                                                  new LayerMaskFieldDrawer(_layers));
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
            _layerSet.GenerateStaticClass = EditorGUILayout.Toggle(ObjectNames.NicifyVariableName(nameof(_layerSet.GenerateStaticClass)), _layerSet.GenerateStaticClass);

            if (_layerSet.GenerateStaticClass)
            {
                _tagManager.Update();

                EditorGUILayout.Space();

                _scrollPos.y = GUILayout.BeginScrollView(_scrollPos, EditorStyles.helpBox).y;

                _layerSet.RootFolder = EditorGUILayout.TextField(ObjectNames.NicifyVariableName(nameof(_layerSet.RootFolder)), _layerSet.RootFolder);
                _layerSet.NameSpace = EditorGUILayout.TextField(ObjectNames.NicifyVariableName(nameof(_layerSet.NameSpace)), _layerSet.NameSpace);

                EditorGUILayout.Space();

                _layerSet.TagFields = EditorGUILayout.Toggle(ObjectNames.NicifyVariableName(nameof(_layerSet.TagFields)), _layerSet.TagFields);
                DrawCollection(_layerSet.TagFields, _tags.EnumerateArrayElements(), item => EditorGUILayout.LabelField(CreateItemString(item.stringValue)));

                _layerSet.SortingLayerFields = EditorGUILayout.Toggle(ObjectNames.NicifyVariableName(nameof(_layerSet.SortingLayerFields)), _layerSet.SortingLayerFields);
                DrawCollection(_layerSet.SortingLayerFields, SortingLayer.layers, item => EditorGUILayout.LabelField(CreateItemString(item.name)));

                _layerSet.LayersFields = EditorGUILayout.Toggle(ObjectNames.NicifyVariableName(nameof(_layerSet.LayersFields)), _layerSet.LayersFields);
                DrawCollection(_layerSet.LayersFields, _layers.EnumerateArrayElements(), drawLayer);
                void drawLayer(SerializedProperty item)
                {
                    if (item.stringValue.HasUsefulData())
                        EditorGUILayout.LabelField(CreateItemString(item.stringValue));
                }

                if (_layerSet.LayersFields)
                    _listDrawer.Draw();

                GUILayout.FlexibleSpace();
                GUILayout.EndScrollView();
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                _layerSet.AutoGenerate = EditorGUILayout.Toggle(_layerSet.AutoGenerate, GUILayout.Width(13f));
                GUILayout.Label(ObjectNames.NicifyVariableName(nameof(_layerSet.AutoGenerate)));
                GUI.enabled = !_layerSet.AutoGenerate;
                if (GUILayout.Button("Generate", GUILayout.Width(100f))) { }
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }

            if (GUI.changed)
            {
                if (!_layerSet.GenerateStaticClass)
                    _layerSet = new LayerSet();

                SaveAsset();
            }
        }

        private void SaveAsset()
        {
            string json = JsonUtility.ToJson(_layerSet, true);
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

        private class LayerMaskFieldDrawer : IListElementDrawer<LayerSet.LayerMaskField>
        {
            private SerializedProperty _layers;

            public LayerMaskFieldDrawer(SerializedProperty layers)
            {
                _layers = layers;
            }

            void IListElementDrawer<LayerSet.LayerMaskField>.OnDrawElement(Rect position, ref LayerSet.LayerMaskField element, bool isActive, bool isFocused)
            {
                Rect halfPos = position;
                halfPos.width = halfPos.width * 0.5f - EditorGuiUtility.StandardHorizontalSpacing;
                element.Name = EditorGUI.TextField(halfPos, element.Name);

                halfPos = position;
                halfPos.width *= 0.5f;
                halfPos.x += position.width * 0.5f;

                var names = _layers.EnumerateArrayElements()
                                   .Select(item => item.stringValue)
                                   .Where(item => item.HasUsefulData())
                                   .ToArray();

                element.Mask = EditorGui.MaskDropDown(halfPos, element.Mask, names);
            }

            float IListElementDrawer<LayerSet.LayerMaskField>.OnElementHeight(int index)
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }
    }
}
#endif
