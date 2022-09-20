using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private (UnityObject asset, long size)[] _result;

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
                        _fileSize = Clamp(EditorGUILayout.LongField(label, _fileSize), 0L, long.MaxValue);
                        break;

                    case 1:
                        _fileSize = (long)Clamp(EditorGUILayout.DoubleField(label, _fileSize / 1024d) * 1024d, 0d, double.MaxValue);
                        break;

                    case 2:
                        _fileSize = (long)Clamp(EditorGUILayout.DoubleField(label, _fileSize / 1024d / 1024d) * 1024d * 1024d, 0d, double.MaxValue);
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
                for (int i = 0; i < _result.Length; i++)
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

                if (_result.Length == 0)
                    GUILayout.Label("No files found.");
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();

            bool search = GUILayout.Button("Search", GUILayout.Height(30f));

            if (search)
                _result = SearchFilesBySize(_fileSize);
        }

        private static (UnityObject asset, long size)[] SearchFilesBySize(long minSizeInBytes)
        {
            string projectFolderPath = PathUtility.GetParentPath(Application.dataPath);
            List<(string assetPath, long size)> foundObjects = new List<(string, long)>();

            AssetDatabaseExt.EnumerateAssetFiles("*")
                            .AsParallel()
                            .ForAll(run);

            return foundObjects.OrderByDescending(item => item.size)
                               .Select(item => (AssetDatabase.LoadAssetAtPath<UnityObject>(item.assetPath), item.size))
                               .ToArray();

            void run(string filePath)
            {
                if (Path.GetExtension(filePath) == ".meta")
                    return;

                FileInfo info = new FileInfo(filePath);

                if (info.Length >= minSizeInBytes)
                {
                    string assetPath = filePath.Remove(0, projectFolderPath.Length + 1);
                    foundObjects.Add((assetPath, info.Length));
                }
            }
        }

        private static long Clamp(long value, long min, long max)
        {
#if UNITY_2021_2_OR_NEWER
            return Math.Clamp(value, min, max);
#else
            if (value < min)
                return min;

            if (value > max)
                return max;

            return value;
#endif
        }

        private static double Clamp(double value, double min, double max)
        {
#if UNITY_2021_2_OR_NEWER
            return Math.Clamp(value, min, max);
#else
            if (value < min)
                return min;

            if (value > max)
                return max;

            return value;
#endif
        }
    }
}
