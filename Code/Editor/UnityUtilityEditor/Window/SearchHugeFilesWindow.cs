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
        private const float WIDTH = 100f;

        private readonly string[] _sizeToolbarNames = new string[] { "Bytes", "KiB", "MiB", "GiB" };
        private int _sizeToolbarIndex = 2;

        private Vector2 _scrollPosition;

        private long _fileSize = 100L * 1024 * 1024;
        private (UnityObject asset, string folder, long size)[] _result;

        private void OnEnable()
        {
            minSize = new Vector2(450f, 200f);
        }

        public static void Create()
        {
            GetWindow<SearchHugeFilesWindow>(true, "Search Huge Files");
        }

        private void OnGUI()
        {
            bool search;

            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Min File Size: ");
                _fileSize = SizeField(_fileSize, _sizeToolbarIndex);
                _sizeToolbarIndex = EditorGUILayout.Popup(_sizeToolbarIndex, _sizeToolbarNames, GUILayout.Width(80f));
                GUILayout.FlexibleSpace();
                search = GUILayout.Button("Search", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(150f));
            }

            EditorGUILayout.Space();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            if (_result != null)
            {
                GUI.enabled = false;
                for (int i = 0; i < _result.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    GUILayout.Label(SizeToLable(_result[i].size, _sizeToolbarNames), GUILayout.Width(100f));
                    GUILayout.Label(_result[i].folder);
                    EditorGUILayout.ObjectField(_result[i].asset, typeof(UnityObject), false);
                    GUILayout.FlexibleSpace();

                    EditorGUILayout.EndHorizontal();
                }
                GUI.enabled = true;

                if (_result.Length == 0)
                    GUILayout.Label("No files found.");
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();

            if (search)
                SearchFilesBySize(_fileSize);
        }

        private void SearchFilesBySize(long minSizeInBytes)
        {
            string projectFolderPath = PathUtility.GetParentPath(Application.dataPath);

            _result = AssetDatabaseExt.EnumerateAssetFiles("*")
                                      .AsParallel()
                                      .Where(item => Path.GetExtension(item) != ".meta")
                                      .Select(selector)
                                      .Where(item => item.size >= minSizeInBytes)
                                      .OrderByDescending(item => item.size)
                                      .AsSequential()
                                      .Select(createTuple)
                                      .ToArray();

            (string path, long size) selector(string filePath)
            {
                FileInfo info = new FileInfo(filePath);
                string assetPath = filePath.Remove(0, projectFolderPath.Length + 1);
                return (assetPath, info.Length);
            }

            (UnityObject, string, long) createTuple((string path, long size) item)
            {
                const string slash = " ∕ ";
                string prettyPath = PathUtility.GetParentPath(item.path).Replace("/", slash).Replace("\\", slash) + " ∕";
                return (AssetDatabase.LoadAssetAtPath<UnityObject>(item.path), prettyPath, item.size);
            }
        }

        private string SizeToLable(long size, string[] sizeToolbarNames)
        {
            if (size <= 1024)
                return $"{size} {sizeToolbarNames[0]}";

            if (size <= 1024L * 1024L)
                return $"{size / 1024d:F} {sizeToolbarNames[1]}";

            if (size <= 1024L * 1024L * 1024L)
                return $"{size / 1024d / 1024d:F} {sizeToolbarNames[2]}";

            return $"{size / 1024d / 1024d / 1024d:F} {sizeToolbarNames[3]}";
        }

        private static long SizeField(long size, int sizeToolbarIndex)
        {
            switch (sizeToolbarIndex)
            {
                case 0: return Clamp(EditorGUILayout.LongField(size, GUILayout.Width(WIDTH)), 0L, long.MaxValue);
                case 1: return (long)Clamp(EditorGUILayout.DoubleField(size / 1024d, GUILayout.Width(WIDTH)) * 1024d, 0d, double.MaxValue);
                case 2: return (long)Clamp(EditorGUILayout.DoubleField(size / 1024d / 1024d, GUILayout.Width(WIDTH)) * 1024d * 1024d, 0d, double.MaxValue);
                case 3: return (long)Clamp(EditorGUILayout.DoubleField(size / 1024d / 1024d / 1024d, GUILayout.Width(WIDTH)) * 1024d * 1024d * 1024d, 0d, double.MaxValue);
                default: throw new UnsupportedValueException(sizeToolbarIndex);
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
