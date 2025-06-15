#if INCLUDE_ADDRESSABLES && INCLUDE_NEWTONSOFT_JSON
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using OlegHcp;
using OlegHcp.CSharp;
using OlegHcp.CSharp.Collections;
using OlegHcpEditor.Engine;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace OlegHcpEditor.Window
{
    public class AddressablesAnalysisResultsWindow : EditorWindow
    {
        private readonly GUILayoutOption[] _buttonOptions = new[]
        {
            GUILayout.Width(10f),
            GUILayout.Height(EditorGUIUtility.singleLineHeight),
        };

        private readonly GUILayoutOption[] _iconOptions = new[]
        {
            GUILayout.Width(EditorGUIUtility.singleLineHeight),
            GUILayout.Height(EditorGUIUtility.singleLineHeight),
        };

        private Container[] _results;
        private Vector2 _scrollPosition;
        private int _selected = -1;
        private Texture2D _selectedLine;
        private Texture2D _selectedLineNotFocused;
        private bool _focused;

        private void OnFocus()
        {
            _focused = true;
        }

        private void OnLostFocus()
        {
            _focused = false;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (_results.HasAnyData())
                GUILayout.Label($"Duplicated Assets: {_results.Length}");

            GUILayout.FlexibleSpace();

            if (_results.HasAnyData())
            {
                if (GUILayout.Button("Clear", GUILayout.Width(70f)))
                {
                    _results = null;
                    _selected = -1;
                    _scrollPosition = default;
                }
            }

            bool open = GUILayout.Button("Import Results", GUILayout.Width(110f));
            EditorGUILayout.EndHorizontal();

            if (open)
            {
                string path = EditorUtility.OpenFilePanel("Import analysis results from json", string.Empty, "json");

                if (path.IsNullOrEmpty())
                    return;

                try
                {
                    Deserialize(File.ReadAllText(path));
                }
                catch (Exception)
                {
                    Debug.LogWarning("Bad file.");
                    return;
                }
            }

            if (_results.IsNullOrEmpty())
                return;

            EditorGUILayout.Space();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, EditorStyles.helpBox);

            for (int i = 0; i < _results.Length; i++)
            {
                ref Container container = ref _results[i];

                GUIStyle lineStyle = _selected == i ? GetSelectedLineStyle() : EditorStyles.label;

                EditorGUILayout.BeginHorizontal();
                container.Expanded = EditorGUILayout.Toggle(container.Expanded, EditorStyles.foldout, _buttonOptions);
                bool clicked = GUILayout.Button(container.Asset.GetAssetIcon(), EditorStyles.label, _iconOptions);
                clicked |= GUILayout.Button($"{container.Path} ({container.Bundles.Length})", lineStyle);
                EditorGUILayout.EndHorizontal();

                if (container.Expanded)
                {
                    foreach (string bundle in container.Bundles)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(30f);
                        GUILayout.Label(bundle);
                        EditorGUILayout.EndHorizontal();
                    }
                }

                if (clicked)
                {
                    _selected = i;
                    EditorGUIUtility.PingObject(container.Asset);
                }
            }
            EditorGUILayout.EndScrollView();
        }

        public static void Create()
        {
#if INCLUDE_ADDRESSABLES && INCLUDE_NEWTONSOFT_JSON
            GetWindow<AddressablesAnalysisResultsWindow>(false, "Analysis Results", true);
#else
            GetWindow<AddressablesAnalysisInfo>(true, "Analysis Results", true);
#endif
        }

        private void Deserialize(string json)
        {
            IEnumerable<(string asset, string bundle)> resultsQuery = JObject.Parse(json)
                                                                             .GetValue("m_RuleToResults")
                                                                             .First(item => (string)item["RuleName"] == "Check Duplicate Bundle Dependencies")
                                                                             .SelectToken("Results")
                                                                             .AsParallel()
                                                                             .Select(item => (string)item["m_ResultName"])
                                                                             .Select(item => item.Split(':'))
                                                                             .Select(item => (item[2], item[1]))
                                                                             .AsSequential();

            Dictionary<string, HashSet<string>> organizedResults = new Dictionary<string, HashSet<string>>();
            foreach (var (asset, bundle) in resultsQuery)
            {
                if (!organizedResults.TryGetValue(asset, out var bundles))
                    bundles = organizedResults.Place(asset, new HashSet<string>());

                bundles.Add(bundle);
            }

            _results = organizedResults.Select(item => new Container(item.Key, item.Value.OrderBy(i => i).ToArray()))
                                       .OrderBy(item => item.Path)
                                       .ToArray();
        }

        private GUIStyle GetSelectedLineStyle()
        {
            if (_selectedLine == null)
            {
                const float max = 255f;
                _selectedLine = makeTexture(new Color(44f / max, 92f / max, 134f / max));
                _selectedLineNotFocused = makeTexture(new Color(77f / max, 77f / max, 77f / max));
            }

            GUIStyle selectedLineStyle = new GUIStyle(EditorStyles.label);
            selectedLineStyle.normal.background = _focused ? _selectedLine : _selectedLineNotFocused;
            selectedLineStyle.normal.textColor = Colours.White;

            return selectedLineStyle;

            Texture2D makeTexture(in Color32 color)
            {
                const int size = 4;
                Color32[] pix = new Color32[size * size];

                for (int i = 0; i < pix.Length; i++)
                {
                    pix[i] = color;
                }

                Texture2D result = new Texture2D(size, size);
                result.SetPixels32(pix);
                result.Apply();

                return result;
            }
        }

        private struct Container
        {
            public string Path;
            public string[] Bundles;
            public bool Expanded;
            public UnityObject Asset;

            public Container(string path, string[] bundles)
            {
                Asset = AssetDatabase.LoadAssetAtPath<UnityObject>(path);
                Path = path;
                Bundles = bundles;
                Expanded = false;
            }
        }
    }
}
#endif
