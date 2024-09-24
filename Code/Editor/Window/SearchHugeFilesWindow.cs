using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using OlegHcp;
using OlegHcp.CSharp.Collections;
using OlegHcp.IO;
using OlegHcp.Mathematics;
using OlegHcpEditor.Engine;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace OlegHcpEditor.Window
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
                    GUILayout.Label(SizeToLabel(container.Size, _sizeToolbarNames));
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
                    throw new SwitchExpressionException(_sortWayIndex);
            }
        }

        private string SizeToLabel(long size, string[] sizeToolbarNames)
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
                case 0: return EditorGUILayout.LongField(size, GUILayout.Width(WIDTH)).ClampMin(0L);
                case 1: return (long)(EditorGUILayout.DoubleField(size / 1024d, GUILayout.Width(WIDTH)) * 1024d).ClampMin(0d);
                case 2: return (long)(EditorGUILayout.DoubleField(size / 1024d / 1024d, GUILayout.Width(WIDTH)) * 1024d * 1024d).ClampMin(0d);
                case 3: return (long)(EditorGUILayout.DoubleField(size / 1024d / 1024d / 1024d, GUILayout.Width(WIDTH)) * 1024d * 1024d * 1024d).ClampMin(0d);
                default: throw new SwitchExpressionException(sizeToolbarIndex);
            }
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
                Path = path;
            }
        }
    }
}
