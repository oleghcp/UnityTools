﻿using System;
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

        private readonly GUILayoutOption[] _iconOptions = new[]
        {
            GUILayout.Width(EditorGUIUtility.singleLineHeight),
            GUILayout.Height(EditorGUIUtility.singleLineHeight),
        };

        private readonly string[] _sizeToolbarNames = new string[] { "Bytes", "KiB", "MiB", "GiB" };
        private readonly string[] _sortWays = new string[] { "Path", "Size ▲", "Size ▼" };
        private int _sizeToolbarIndex = 2;
        private int _sortWayIndex = 2;

        private Vector2 _scrollPosition;

        private long _fileSize = 100L * 1024 * 1024;
        private Container[] _result;

        private void OnEnable()
        {
            minSize = new Vector2(600f, 200f);
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

                if (_result.HasAnyData())
                {
                    int prevSortIndex = _sortWayIndex;
                    _sortWayIndex = GUILayout.Toolbar(_sortWayIndex, _sortWays);
                    if (_sortWayIndex != prevSortIndex)
                        Sort();
                }

                GUILayout.FlexibleSpace();

                search = GUILayout.Button("Search", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(120f));
            }

            EditorGUILayout.Space();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, EditorStyles.helpBox);
            if (_result != null)
            {
                for (int i = 0; i < _result.Length; i++)
                {
                    ref Container container = ref _result[i];

                    EditorGUILayout.BeginHorizontal();
                    bool clicked = GUILayout.Button(container.Asset.GetAssetIcon(), EditorStyles.label, _iconOptions);
                    clicked |= GUILayout.Button(container.Path, EditorStyles.label);
                    GUILayout.Label(SizeToLable(container.Size, _sizeToolbarNames));
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

                    if (clicked)
                        EditorGUIUtility.PingObject(container.Asset);
                }

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
                                      .AsSequential()
                                      .Select(item => new Container(item.path, item.size))
                                      .ToArray();
            Sort();

            (string path, long size) selector(string filePath)
            {
                FileInfo info = new FileInfo(filePath);
                string assetPath = filePath.Remove(0, projectFolderPath.Length + 1);
                return (assetPath, info.Length);
            }
        }

        private void Sort()
        {
            switch (_sortWayIndex)
            {
                case 0:
                    _result.Sort(item => item.Path);
                    break;

                case 1:
                    _result.Sort(item => item.Size);
                    break;

                case 2:
                    _result.Sort(item => -item.Size);
                    break;

                default:
                    throw new UnsupportedValueException(_sortWayIndex);
            }
        }

        private string SizeToLable(long size, string[] sizeToolbarNames)
        {
            if (size <= 1024)
                return convert(size, sizeToolbarNames[0]);

            if (size <= 1024L * 1024L)
                return convert(size / 1024d, sizeToolbarNames[1]);

            if (size <= 1024L * 1024L * 1024L)
                return convert(size / 1024d / 1024d, sizeToolbarNames[2]);

            return convert(size / 1024d / 1024d / 1024d, sizeToolbarNames[3]);

            string convert(double value, string unit) => $"({value:F} {unit})";
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

        private struct Container
        {
            public string Path;
            public UnityObject Asset;
            public long Size;

            public Container(string path, long size)
            {
                Size = size;
                Asset = AssetDatabase.LoadAssetAtPath<UnityObject>(path);
                Path = EditorGuiUtility.NicifyPathLabel(path);
            }
        }
    }
}
