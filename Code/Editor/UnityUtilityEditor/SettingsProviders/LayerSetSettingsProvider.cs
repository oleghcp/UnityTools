using System.Collections.Generic;
using System.IO;
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
        private SerializedObject _tagManager;
        private ListDrawer<LayerSet.LayerMaskField> _listDrawer;
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

            _listDrawer = new ListDrawer<LayerSet.LayerMaskField>(_layerSet.LayerMasks,
                                                                  ObjectNames.NicifyVariableName(nameof(_layerSet.LayerMasks)),
                                                                  new LayerMaskFieldDrawer());
        }

        [SettingsProvider]
        private static SettingsProvider CreateMyCustomSettingsProvider()
        {
            return new LayerSetSettingsProvider($"{nameof(UnityUtility)}/Later Set", SettingsScope.Project);
        }

        public override void OnGUI(string searchContext)
        {
            _layerSet.GenerateStaticClass = EditorGUILayout.Toggle(ObjectNames.NicifyVariableName(nameof(_layerSet.GenerateStaticClass)), _layerSet.GenerateStaticClass);

            if (_layerSet.GenerateStaticClass)
            {
                EditorGUILayout.Space();

                _scrollPos.y = GUILayout.BeginScrollView(_scrollPos, EditorStyles.helpBox).y;

                _layerSet.RootFolder = EditorGUILayout.TextField(ObjectNames.NicifyVariableName(nameof(_layerSet.RootFolder)), _layerSet.RootFolder);
                _layerSet.NameSpace = EditorGUILayout.TextField(ObjectNames.NicifyVariableName(nameof(_layerSet.NameSpace)), _layerSet.NameSpace);

                EditorGUILayout.Space();

                _layerSet.TagFields = EditorGUILayout.Toggle(ObjectNames.NicifyVariableName(nameof(_layerSet.TagFields)), _layerSet.TagFields);
                _layerSet.SortingLayerFields = EditorGUILayout.Toggle(ObjectNames.NicifyVariableName(nameof(_layerSet.SortingLayerFields)), _layerSet.SortingLayerFields);
                _layerSet.LayersField = EditorGUILayout.Toggle(ObjectNames.NicifyVariableName(nameof(_layerSet.LayersField)), _layerSet.LayersField);

                if (_layerSet.LayersField)
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

        private class LayerMaskFieldDrawer : IListElementDrawer<LayerSet.LayerMaskField>
        {
            void IListElementDrawer<LayerSet.LayerMaskField>.OnDrawElement(Rect position, ref LayerSet.LayerMaskField element, bool isActive, bool isFocused)
            {
                Rect halfPos = position;
                halfPos.width = halfPos.width * 0.5f - EditorGuiUtility.StandardHorizontalSpacing;
                element.Name = EditorGUI.TextField(halfPos, element.Name);

                halfPos = position;
                halfPos.width *= 0.5f;
                halfPos.x += position.width * 0.5f;
                element.Mask = EditorGUI.MaskField(halfPos, element.Mask, new[] { "Qwe", "Rty" });
            }

            float IListElementDrawer<LayerSet.LayerMaskField>.OnElementHeight(int index)
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }
    }
}
#endif
