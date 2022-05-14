using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityObject = UnityEngine.Object;

namespace UnityUtilityEditor.Window
{
    internal class SearchHugeFilesWindow : EditorWindow
    {
        private readonly string[] _sizeToolbarNames = new string[] { "Bytes", "Kb", "Mb" };
        private int _sizeToolbarIndex = 2;

        private Vector2 _scrollPosition;

        private long _fileSize = 100L * 1024 * 1024;
        private List<(UnityObject asset, long size)> _result;

        private void OnEnable()
        {
            minSize = new Vector2(250f, 200f);
        }

        public static void Create()
        {
            GetWindow<SearchHugeFilesWindow>(true, "Search Huge Files");
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                const string label = "Min File Size";

                switch (_sizeToolbarIndex)
                {
                    case 0:
                        _fileSize = MathUtility.Clamp(EditorGUILayout.LongField(label, _fileSize), 0L, long.MaxValue);
                        break;

                    case 1:
                        _fileSize = (long)MathUtility.Clamp(EditorGUILayout.DoubleField(label, _fileSize / 1024d) * 1024d, 0d, double.MaxValue);
                        break;

                    case 2:
                        _fileSize = (long)MathUtility.Clamp(EditorGUILayout.DoubleField(label, _fileSize / 1024d / 1024d) * 1024d * 1024d, 0d, double.MaxValue);
                        break;

                    default:
                        throw new UnsupportedValueException(_sizeToolbarIndex);
                }

                _sizeToolbarIndex = EditorGUILayout.Popup(_sizeToolbarIndex, _sizeToolbarNames, GUILayout.Width(100f));
            }

            EditorGUILayout.Space();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            if (_result != null)
            {
                GUI.enabled = false;
                for (int i = 0; i < _result.Count; i++)
                {
                    string sizeLabel;
                    long size = _result[i].size;

                    if (size <= 1024)
                        sizeLabel = $"{size} {_sizeToolbarNames[0]}";
                    else if (size <= 1024 * 1024)
                        sizeLabel = $"{size / 1024d:F} {_sizeToolbarNames[1]}";
                    else
                        sizeLabel = $"{size / 1024d / 1024d:F} {_sizeToolbarNames[2]}";

                    EditorGUILayout.ObjectField(sizeLabel, _result[i].asset, typeof(UnityObject), false);
                }
                GUI.enabled = true;

                if (_result.Count == 0)
                    GUILayout.Label("No files found.");
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();

            if (GUILayout.Button("Search", GUILayout.Height(30f)))
            {
                FindHugeFiles(done);
            }

            void done(List<(UnityObject asset, long size)> list)
            {
                _result = list;
                _result.Sort(item => -item.size);
                Repaint();
            }
        }

        private void FindHugeFiles(Action<List<(UnityObject, long)>> succes)
        {
            List<(UnityObject, long)> foundObjects = new List<(UnityObject, long)>();
            IEnumerator<float> iterator = MenuItemsUtility.SearchFilesBySize(_fileSize, foundObjects);
            EditorUtilityExt.ShowProgressBarCancelable("Searching assets", "That could take a while...", iterator, () => succes(foundObjects));
        }
    }
}
